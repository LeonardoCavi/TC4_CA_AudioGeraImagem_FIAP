using AudioGeraImagemAPI.Domain.Utility.DTO;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace AudioGeraImagemAPI.UseCases.Comandos.Create
{
    public class CriarComandoCommand: IRequest<ResultadoOperacao<Guid>>
    {
        public string Descricao { get; set; }
        public IFormFile Arquivo { get; set; }
    }
}
