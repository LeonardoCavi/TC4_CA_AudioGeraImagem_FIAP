using AudioGeraImagemAPI.Domain.Utility.DTO;
using MediatR;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace AudioGeraImagemAPI.UseCases.Criacoes.Obter
{
    public class ObterCriacaoQuery: IRequest<ResultadoOperacao<CriacaoDTO>>
    {
        [ExcludeFromCodeCoverage]
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
