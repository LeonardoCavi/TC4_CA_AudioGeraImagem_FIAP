using AudioGeraImagemAPI.Domain.Utility.DTO;
using MediatR;
using System.Text.Json;


namespace AudioGeraImagemAPI.UseCases.Criacoes.List
{
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
