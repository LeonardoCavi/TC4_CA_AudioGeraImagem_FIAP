using AudioGeraImagem.Domain.Messages;
using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Interfaces;
using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using AudioGeraImagemAPI.Domain.Utility;
using AudioGeraImagemAPI.Domain.Utility.DTO;
using AudioGeraImagemAPI.Domain.Utility.Factory;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace AudioGeraImagemAPI.Domain.Services
{
    public class ComandoService : IComandoService
    {
        private readonly IComandoRepository _repository;
        private readonly IBus _bus;
        private readonly HttpHelper _httpHelper;
        private string nomeFila;

        public ComandoService(IComandoRepository repository, IBus bus, HttpHelper httpHelper, IConfiguration configuration)
        {
            _repository = repository;
            _bus = bus;
            _httpHelper = httpHelper;
            nomeFila = configuration.GetRequiredSection("MassTransit")["Fila"] ?? string.Empty;
        }

        public async Task GerarImagem(Comando comando, IFormFile arquivo)
        {
            using var stream = arquivo.OpenReadStream();

            var payload = ObterPayload(stream);
            var mensagem = CriarMensagem(comando, payload);

            await _repository.Inserir(comando);

            await PublicarMensagem(mensagem);
        }

        public async Task<ResultadoOperacao<Stream>> ObterImagem(string id)
        {
            var comando = await ObterComando(id);

            if (!comando.Sucesso)
                return ResultadoOperacaoFactory.Criar(false, comando.MensagemErro, Stream.Null);

            if(!comando.Objeto.ProcessamentosComandos.Any(x => x.Estado == Enums.EstadoComando.Finalizado))
                return ResultadoOperacaoFactory.Criar(false, "Comando ainda está em processamento ou finalizou com falha.", Stream.Null);

            var bytes = await _httpHelper.GetBytes(comando.Objeto.UrlImagem);

            return ResultadoOperacaoFactory.Criar(true, comando.MensagemErro, ObterStream(bytes));
        }

        public async Task<ResultadoOperacao<Comando>> ObterComando(string id)
        {
            var comando = await _repository.ObterComandoProcessamentos(id);

            if (comando == null)
                return ResultadoOperacaoFactory.Criar(false, "Criação não encontrada.", comando);

            return ResultadoOperacaoFactory.Criar(true, string.Empty, comando);
        }
        public async Task<ResultadoOperacao<ICollection<Comando>>> Buscar(string busca)
        {
            ICollection<Comando> comandos;

            if (string.IsNullOrEmpty(busca))
            {
                comandos = await _repository.ObterComandosProcessamentos();

                if (comandos == null || !comandos.Any())
                    return ResultadoOperacaoFactory.Criar(false, "Não existem criações.", comandos);
            }
            else
            {
                comandos = await _repository.Buscar(x => x.Descricao.Contains(busca) || x.Transcricao.Contains(busca));

                if (comandos == null || !comandos.Any())
                    return ResultadoOperacaoFactory.Criar(false, "Criações não encontradas.", comandos);
            }

            return ResultadoOperacaoFactory.Criar(true, string.Empty, comandos);
        }
        private byte[] ObterPayload(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            var bytes = memoryStream.ToArray();

            return bytes;
        }
        private ComandoMessage CriarMensagem(Comando comando, byte[] payload)
        {
            return new()
            {
                ComandoId = comando.Id,
                Payload = payload
            };
        }
        private async Task PublicarMensagem(ComandoMessage mensagem)
        {
            var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{nomeFila}"));
            await endpoint.Send(mensagem);
        }
        private Stream ObterStream(byte[] bytes)
        {
            return new MemoryStream(bytes);
        }
    }
}
