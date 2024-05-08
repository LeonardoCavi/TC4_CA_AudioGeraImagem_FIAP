using AudioGeraImagemAPI.Domain.Utility.DTO;
using MediatR;
using System.Text.Json;


namespace AudioGeraImagemAPI.UseCases.Comandos.List
{
    public class ListarComandosQuery : IRequest<ResultadoOperacao<IEnumerable<ListarComandosDTO>>>
    {
        public ListarComandosQuery(string busca)
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
