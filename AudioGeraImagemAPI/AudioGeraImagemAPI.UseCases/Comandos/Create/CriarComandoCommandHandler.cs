using AudioGeraImagem.Domain.Messages;
using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
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
            if (!request.Valido())
                return ResultadoOperacaoFactory.Criar(false, "Escreva uma descrição com até 256 caracteres e o arquivo deve ser .mp3", Guid.Empty);

            var comando = new Comando(request.Descricao);

            var payload = ObterPayload(request);

            var mensagem = new ComandoMessage(comando.Id, payload);

            await _repository.Inserir(comando);

            await PublicarMensagem(mensagem);

            return ResultadoOperacaoFactory.Criar(true, string.Empty, comando.Id);
        }

        private byte[] ObterPayload(CriarComandoCommand request)
        {
            using var stream = request.Arquivo.OpenReadStream();
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            var bytes = memoryStream.ToArray();

            return bytes;
        }
        private async Task PublicarMensagem(ComandoMessage mensagem)
        {
            var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{nomeFila}"));
            await endpoint.Send(mensagem);
        }
    }
}
