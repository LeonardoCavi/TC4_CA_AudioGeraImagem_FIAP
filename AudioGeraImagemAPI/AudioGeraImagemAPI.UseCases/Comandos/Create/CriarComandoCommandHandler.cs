using AudioGeraImagem.Domain.Messages;
using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Enums;
using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using AudioGeraImagemAPI.Domain.Utility;
using AudioGeraImagemAPI.Domain.Utility.DTO;
using AudioGeraImagemAPI.Domain.Utility.Factory;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace AudioGeraImagemAPI.UseCases.Comandos.Create
{
    public class CriarComandoCommandHandler : IRequestHandler<CriarComandoCommand, ResultadoOperacao<Guid>>
    {
        private readonly IComandoRepository _repository;
        private readonly IBus _bus;
        private string nomeFila;

        public CriarComandoCommandHandler(IComandoRepository repository, IBus bus, IConfiguration configuration)
        {
            _repository = repository;
            _bus = bus;
            nomeFila = configuration.GetRequiredSection("MassTransit")["Fila"] ?? string.Empty;
        }

        public async Task<ResultadoOperacao<Guid>> Handle(CriarComandoCommand request, CancellationToken cancellationToken)
        {
            if (!RequisicaoValida(request))
                return ResultadoOperacaoFactory.Criar(false, "Escreva uma descrição com até 256 caracteres e o arquivo deve ser .mp3", Guid.Empty);

            var comando = CriarComando(request);

            using var stream = request.Arquivo.OpenReadStream();

            var payload = ObterPayload(stream);
            var mensagem = CriarMensagem(comando, payload);

            await _repository.Inserir(comando);

            await PublicarMensagem(mensagem);

            return ResultadoOperacaoFactory.Criar(true, string.Empty, comando.Id);
        }

        private bool RequisicaoValida(CriarComandoCommand request)
        {
            if (request.Descricao.Length > 256)
                return false;

            if (!request.Arquivo.ContentType.Contains("audio/mpeg"))
                return false;

            return true;
        }

        private Comando CriarComando(CriarComandoCommand gerarImagem)
        {
            var comando = new Comando()
            {
                Id = Guid.NewGuid(),
                Descricao = gerarImagem.Descricao,
                InstanteCriacao = DateTime.Now,
                ProcessamentosComandos = new()
            };

            comando.ProcessamentosComandos.Add(new()
            {
                Estado = EstadoComando.Recebido,
                InstanteCriacao = DateTime.Now
            });

            return comando;
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
    }
}
