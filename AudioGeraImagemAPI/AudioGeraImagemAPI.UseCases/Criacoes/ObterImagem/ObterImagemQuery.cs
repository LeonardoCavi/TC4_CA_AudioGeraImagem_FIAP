using AudioGeraImagemAPI.Domain.Utility.DTO;
using MediatR;
using System.Text.Json;

namespace AudioGeraImagemAPI.UseCases.Criacoes.ObterImagem
{
    public class ObterImagemQuery : IRequest<ResultadoOperacao<Stream>>
    {
        public ObterImagemQuery(string id)
        {
            Id = id;
        }

        public string Id { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
