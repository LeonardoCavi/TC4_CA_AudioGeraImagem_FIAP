using AudioGeraImagemAPI.Domain.Utility.DTO;
using MediatR;

namespace AudioGeraImagemAPI.UseCases.Imagens.Get
{
    public class ObterImagemQuery: IRequest<ResultadoOperacao<Stream>>
    {
        public string Id { get; set; }
    }
}
