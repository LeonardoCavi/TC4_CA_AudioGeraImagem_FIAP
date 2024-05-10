using AudioGeraImagem.Domain.Messages;
using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using AudioGeraImagemAPI.Domain.Utility.DTO;
using AudioGeraImagemAPI.Domain.Utility.Factory;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace AudioGeraImagemAPI.UseCases.Criacoes.GerarImagem
{
    public class GerarImagemCommandHandler : IRequestHandler<GerarImagemCommand, ResultadoOperacao<Guid>>
    {
        private readonly ICriacaoRepository _repository;
        private readonly IBus _bus;
        private string nomeFila;

        public GerarImagemCommandHandler(ICriacaoRepository repository, IBus bus, IConfiguration configuration)
        {
            _repository = repository;
            _bus = bus;
            nomeFila = configuration.GetRequiredSection("MassTransit")["Fila"] ?? string.Empty;
        }

        public async Task<ResultadoOperacao<Guid>> Handle(GerarImagemCommand request, CancellationToken cancellationToken)
        {
            if (!request.Valido())
                return ResultadoOperacaoFactory.Criar(false, "Escreva uma descrição com até 256 caracteres e o arquivo deve ser .mp3", Guid.Empty);

            var criacao = new Criacao(request.Descricao);

            var payload = ObterPayload(request);

            var mensagem = new CriacaoMessage(criacao.Id, payload);

            await _repository.Inserir(criacao);

            await PublicarMensagem(mensagem);

            return ResultadoOperacaoFactory.Criar(true, string.Empty, criacao.Id);
        }

        private byte[] ObterPayload(GerarImagemCommand request)
        {
            using var stream = request.Arquivo.OpenReadStream();
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            var bytes = memoryStream.ToArray();

            return bytes;
        }
        private async Task PublicarMensagem(CriacaoMessage mensagem)
        {
            var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{nomeFila}"));
            await endpoint.Send(mensagem);
        }
    }
}
