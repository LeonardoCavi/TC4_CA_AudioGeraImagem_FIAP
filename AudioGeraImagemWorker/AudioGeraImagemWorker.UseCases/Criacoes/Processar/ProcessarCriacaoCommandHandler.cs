using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
using AudioGeraImagemWorker.Domain.Interfaces;
using AudioGeraImagemWorker.Domain.Utility;
using MediatR;
using Microsoft.Extensions.Logging;
using AudioGeraImagemWorker.Domain.Enums;
using AudioGeraImagem.Domain.Entities;
using AudioGeraImagemWorker.Domain.Interfaces.Utility;
using AudioGeraImagemWorker.Domain.Entities;

namespace AudioGeraImagemWorker.UseCases.Criacoes.Processar
{
    internal class ProcessarCriacaoCommandHandler : IRequestHandler<ProcessarCriacaoCommand>
    {
        private readonly IHttpHelper _httpHelper;
        private readonly ICriacaoRepository _criacaoRepository;
        private readonly IErroManager _erroManager;
        private readonly IBucketManager _bucketManager;
        private readonly IOpenAIVendor _openAIVendor;
        private readonly ILogger<ProcessarCriacaoCommandHandler> _logger;
        private readonly string _className = typeof(ProcessarCriacaoCommandHandler).Name;

        public ProcessarCriacaoCommandHandler(
            IHttpHelper httpHelper,
            ICriacaoRepository criacaoRepository,
            IErroManager erroManager,
            IBucketManager bucketManager,
            IOpenAIVendor openAIVendor,
            ILogger<ProcessarCriacaoCommandHandler> logger)
        {
            _httpHelper = httpHelper;
            _criacaoRepository = criacaoRepository;
            _erroManager = erroManager;
            _bucketManager = bucketManager;
            _openAIVendor = openAIVendor;
            _logger = logger;
        }

        public async Task Handle(ProcessarCriacaoCommand request, CancellationToken cancellationToken)
        {
            var criacao = await _criacaoRepository.Obter(request.CriacaoId);

            if (criacao is null)
            {
                _logger.LogWarning($"[{_className}] - [Handle] => Criacao descartada, pois a criacao de id '{request.CriacaoId}' não existe.");
            }
            else
            {
                if (criacao.ProcessamentosCriacao.Last()?.Estado is EstadoProcessamento.Recebido)
                    await AtualizarProcessamentoCriacao(criacao);

                if (request.Retentativa)
                    await AtualizarCriacao(criacao, request.UltimoEstado);

                await ExecutarCriacao(criacao, request.Payload);
            }
        }

        private async Task ExecutarCriacao(Criacao criacao, byte[] payload = null)
        {
            var ultimoProcessamento = criacao.ProcessamentosCriacao.LastOrDefault();

            try
            {
                switch (ultimoProcessamento.Estado)
                {
                    case EstadoProcessamento.SalvandoAudio:
                        await SalvarAudio(criacao, payload);
                        break;

                    case EstadoProcessamento.GerandoTexto:
                        await GerarTexto(criacao);
                        break;

                    case EstadoProcessamento.GerandoImagem:
                        await GerarImagem(criacao);
                        break;

                    case EstadoProcessamento.SalvadoImagem:
                        await SalvarImagem(criacao);
                        break;

                    case EstadoProcessamento.Finalizado:
                        await Finalizar(criacao);
                        return;
                }

                await AtualizarProcessamentoCriacao(criacao);
                await ExecutarCriacao(criacao, payload);
            }
            catch (Exception ex)
            {
                ultimoProcessamento.MensagemErro = ex.Message;
                await _erroManager.TratarErro(criacao, ultimoProcessamento.Estado, payload);
                await _criacaoRepository.Atualizar(criacao);
            }
        }

        private async Task AtualizarProcessamentoCriacao(Criacao criacao)
        {
            var ultimoProcessamento = criacao.ProcessamentosCriacao.LastOrDefault();

            EstadoProcessamento novoEstadoCriacao = default;

            switch (ultimoProcessamento.Estado)
            {
                case EstadoProcessamento.Recebido:
                    novoEstadoCriacao = EstadoProcessamento.SalvandoAudio;
                    break;

                case EstadoProcessamento.SalvandoAudio:
                    novoEstadoCriacao = EstadoProcessamento.GerandoTexto;
                    break;

                case EstadoProcessamento.GerandoTexto:
                    novoEstadoCriacao = EstadoProcessamento.GerandoImagem;
                    break;

                case EstadoProcessamento.GerandoImagem:
                    novoEstadoCriacao = EstadoProcessamento.SalvadoImagem;
                    break;

                case EstadoProcessamento.SalvadoImagem:
                    novoEstadoCriacao = EstadoProcessamento.Finalizado;
                    break;
            }

            await AtualizarCriacao(criacao, novoEstadoCriacao);
        }

        private async Task AtualizarCriacao(Criacao criacao, EstadoProcessamento novoEstado)
        {
            var novoProcessamentoCriacao = new ProcessamentoCriacao(novoEstado);
            criacao.ProcessamentosCriacao.Add(novoProcessamentoCriacao);
            criacao.InstanteAtualizacao = novoProcessamentoCriacao.InstanteCriacao;

            await _criacaoRepository.Atualizar(criacao);
        }

        #region [ Tratamentos dos Estados dos Criacoes ]

        // 1. Estado Recebido >> Salvando Audio
        private async Task SalvarAudio(Criacao criacao, byte[] payload = null)
        {
            try
            {
                var fileName = string.Concat("audios/", criacao.Id.ToString(), ".mp3");
                criacao.UrlAudio = await _bucketManager.ArmazenarObjeto(payload, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [SalvarAudio] => Exception.: {ex.Message}");
                throw;
            }
        }

        // 2. Salvando Audio >> Gerando Texto
        private async Task GerarTexto(Criacao criacao)
        {
            try
            {
                var bytes = await _httpHelper.GetBytes(criacao.UrlAudio);
                criacao.Transcricao = await _openAIVendor.GerarTranscricao(bytes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [GerarTexto] => Exception.: {ex.Message}");
                throw;
            }
        }

        // 3. Salvando Texto >> Gerando Imagem
        private async Task GerarImagem(Criacao criacao)
        {
            try
            {
                criacao.UrlImagem = await _openAIVendor.GerarImagem(criacao.Transcricao);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [GerarImagem] => Exception.: {ex.Message}");
                throw;
            }
        }

        // 4. Gerando Imagem >> Salvado Imagem
        private async Task SalvarImagem(Criacao criacao)
        {
            try
            {
                var bytes = await _httpHelper.GetBytes(criacao.UrlImagem);
                var fileName = string.Concat("imagens/", criacao.Id.ToString(), ".jpeg");
                var urlImagem = await _bucketManager.ArmazenarObjeto(bytes, fileName);
                criacao.UrlImagem = urlImagem;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [SalvarImagem] => Exception.: {ex.Message}");
                throw;
            }
        }

        // 5. Salvando Imagem >> Finalizado
        private async Task Finalizar(Criacao criacao)
        {
            try
            {
                await _criacaoRepository.Atualizar(criacao);
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
