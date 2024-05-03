using AudioGeraImagemAPI.Domain.Utility.DTO;
using MediatR;

namespace AudioGeraImagemAPI.UseCases.Comandos.Get
{
    public class ObterComandoQuery: IRequest<ResultadoOperacao<ComandoDTO>>
    {
        public string Id { get; set; }
    }
}
