using AudioGeraImagem.Domain.Entities;
using AudioGeraImagem.Domain.Messages;
using AudioGeraImagemWorker.Domain.Enums;
using AudioGeraImagemWorker.Domain.Factories;
using AudioGeraImagemWorker.Domain.Interfaces;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
using AudioGeraImagemWorker.Domain.Utility;
using Microsoft.Extensions.Logging;

namespace AudioGeraImagemWorker.Domain.Services
{
    public class ComandoManager : IComandoManager
    {
        private readonly HttpHelper _httpHelper;
        private readonly IComandoRepository _comandoRepository;
        private readonly IErroManager _erroManager;
        private readonly IBucketManager _bucketManager;
        private readonly IOpenAIVendor _openAIVendor;
        private readonly ILogger<ComandoManager> _logger;
        private readonly string _className = typeof(ComandoManager).Name;

        public ComandoManager(
            HttpHelper httpHelper,
            IComandoRepository comandoRepository,
            IErroManager erroManager,
            IBucketManager bucketManager,
            IOpenAIVendor openAIVendor,
            ILogger<ComandoManager> logger)
        {
            _httpHelper = httpHelper;
            _comandoRepository = comandoRepository;
            _erroManager = erroManager;
            _bucketManager = bucketManager;
            _openAIVendor = openAIVendor;
            _logger = logger;
        }

        public async Task ProcessarComando(ComandoMessage mensagem)
        {
            var comando = await _comandoRepository.Obter(mensagem.ComandoId);

            if(comando is null)
            {
                _logger.LogWarning($"[{_className}] - [ProcessarComando] => Mensagem descartada, pois o comando de id '{mensagem.ComandoId}' não existe.");
            }
            else
            {
                if(comando.ProcessamentosComandos.Last()?.Estado is EstadoComando.Recebido)
                    await AtualizarProcessamentoComando(comando);

                await ExecutarComando(comando, mensagem.Payload);
            }
        }

        public async Task ReprocessarComando(RetentativaComandoMessage mensagem)
        {
            var comando = await _comandoRepository.Obter(mensagem.ComandoId);

            if (comando is null)
            {
                _logger.LogWarning($"[{_className}] - [ReprocessarComando] => Mensagem descartada, pois o comando de id '{mensagem.ComandoId}' não existe.");
            }

            await AtualizarComando(comando, mensagem.UltimoEstado);
            await ExecutarComando(comando, mensagem.Payload);
        }

        private async Task ExecutarComando(Comando comando, byte[] payload = null)
        {
            var ultimoProcessamento = comando.ProcessamentosComandos.LastOrDefault();

            try
            {
                switch (ultimoProcessamento.Estado)
                {
                    case EstadoComando.SalvandoAudio:
                        await SalvarAudio(comando, payload);
                        break;

                    case EstadoComando.GerandoTexto:
                        await GerarTexto(comando);
                        break;

                    case EstadoComando.GerandoImagem:
                        await GerarImagem(comando);
                        break;

                    case EstadoComando.SalvadoImagem:
                        await SalvarImagem(comando);
                        break;

                    case EstadoComando.Finalizado:
                        await Finalizar(comando);
                        return;
                }

                await AtualizarProcessamentoComando(comando);
                await ExecutarComando(comando, payload);
            }
            catch (Exception ex)
            {
                ultimoProcessamento.MensagemErro = ex.Message;
                await _erroManager.TratarErro(comando, ultimoProcessamento.Estado, payload);
                await _comandoRepository.Atualizar(comando);
            }
        }

        private async Task AtualizarProcessamentoComando(Comando comando)
        {
            var ultimoProcessamento = comando.ProcessamentosComandos.LastOrDefault();

            EstadoComando novoEstadoComando = default;

            switch (ultimoProcessamento.Estado)
            {
                case EstadoComando.Recebido:
                    novoEstadoComando = EstadoComando.SalvandoAudio;
                    break;

                case EstadoComando.SalvandoAudio:
                    novoEstadoComando = EstadoComando.GerandoTexto;
                    break;

                case EstadoComando.GerandoTexto:
                    novoEstadoComando = EstadoComando.GerandoImagem;
                    break;

                case EstadoComando.GerandoImagem:
                    novoEstadoComando = EstadoComando.SalvadoImagem;
                    break;

                case EstadoComando.SalvadoImagem:
                    novoEstadoComando = EstadoComando.Finalizado;
                    break;
            }

            await AtualizarComando(comando, novoEstadoComando);
        }

        private async Task AtualizarComando(Comando comando, EstadoComando estadoComando)
        {
            var novoProcessamentoComando = ProcessamentoComandoFactory.Novo(estadoComando);
            comando.ProcessamentosComandos.Add(novoProcessamentoComando);
            comando.InstanteAtualizacao = novoProcessamentoComando.InstanteCriacao;

            await _comandoRepository.Atualizar(comando);
        }

        #region [ Tratamentos dos Estados dos Comandos ]

        // 1. Estado Recebido >> Salvando Audio
        private async Task SalvarAudio(Comando comando, byte[] payload = null)
        {
            try
            {
                var fileName = string.Concat("audios/", comando.Id.ToString(), ".mp3");
                comando.UrlAudio = await _bucketManager.ArmazenarObjeto(payload, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [SalvarAudio] => Exception.: {ex.Message}");
                throw;
            }   
        }

        // 2. Salvando Audio >> Gerando Texto
        private async Task GerarTexto(Comando comando)
        {
            try
            {
                var bytes = await _httpHelper.GetBytes(comando.UrlAudio);
                comando.Transcricao = await _openAIVendor.GerarTranscricao(bytes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [GerarTexto] => Exception.: {ex.Message}");
                throw;
            }
        }

        // 3. Salvando Texto >> Gerando Imagem
        private async Task GerarImagem(Comando comando)
        {
            try
            {
                comando.UrlImagem = await _openAIVendor.GerarImagem(comando.Transcricao);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [GerarImagem] => Exception.: {ex.Message}");
                throw;
            }
        }

        // 4. Gerando Imagem >> Salvado Imagem
        private async Task SalvarImagem(Comando comando)
        {
            try
            {
                var bytes = await _httpHelper.GetBytes(comando.UrlImagem);
                var fileName = string.Concat("imagens/", comando.Id.ToString(), ".jpeg");
                var urlImagem = await _bucketManager.ArmazenarObjeto(bytes, fileName);
                comando.UrlImagem = urlImagem;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [SalvarImagem] => Exception.: {ex.Message}");
                throw;
            }
        }

        // 5. Salvando Imagem >> Finalizado
        private async Task Finalizar(Comando comando)
        {
            try
            {
                await _comandoRepository.Atualizar(comando);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [Finalizar] => Exception.: {ex.Message}");
                throw;
            }
        }

        #endregion
    }
}