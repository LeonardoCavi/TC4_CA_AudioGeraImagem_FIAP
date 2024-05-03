using AudioGeraImagemAPI.Domain.Utility.DTO;
using MediatR;


namespace AudioGeraImagemAPI.UseCases.Comandos.List
{
    public class ListarComandosQuery : IRequest<ResultadoOperacao<IEnumerable<ListarComandosDTO>>>
    {
        public string Busca { get; set; }
    }
}
