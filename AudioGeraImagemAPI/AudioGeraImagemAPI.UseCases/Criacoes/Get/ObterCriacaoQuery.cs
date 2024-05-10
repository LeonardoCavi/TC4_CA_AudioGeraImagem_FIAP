using AudioGeraImagemAPI.Domain.Utility.DTO;
using MediatR;
using System.Text.Json;

namespace AudioGeraImagemAPI.UseCases.Criacoes.Get
{
    public class ObterCriacaoQuery: IRequest<ResultadoOperacao<CriacaoDTO>>
    {
        public ObterCriacaoQuery(string id)
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
