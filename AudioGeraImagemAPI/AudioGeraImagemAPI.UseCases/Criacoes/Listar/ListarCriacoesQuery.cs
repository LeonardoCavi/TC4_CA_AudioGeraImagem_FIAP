using AudioGeraImagemAPI.Domain.Utility.DTO;
using MediatR;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace AudioGeraImagemAPI.UseCases.Criacoes.Listar
{
    [ExcludeFromCodeCoverage]
    public class ListarCriacoesQuery : IRequest<ResultadoOperacao<IEnumerable<ListarCriacoesDTO>>>
    {
        public ListarCriacoesQuery(string busca)
        {
            Busca = busca;
        }

        public string Busca { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}