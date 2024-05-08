using AudioGeraImagemAPI.Domain.Utility.DTO;
using MediatR;
using System.Text.Json;

namespace AudioGeraImagemAPI.UseCases.Comandos.Get
{
    public class ObterComandoQuery: IRequest<ResultadoOperacao<ComandoDTO>>
    {
        public ObterComandoQuery(string id)
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
