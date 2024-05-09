using AudioGeraImagemWorker.Domain.Enums;
using MediatR;
using System.Text.Json.Serialization;

namespace AudioGeraImagemWorker.UseCases.Comandos.Processar
{
    public class ProcessarComandoCommand : IRequest
    {
        public Guid ComandoId { get; set; }
        public byte[] Payload { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EstadoComando UltimoEstado { get; set; }
        public bool Retentativa { get; set; }

        public ProcessarComandoCommand(Guid comandoId, byte[] payload)
        {
            ComandoId = comandoId;
            Payload = payload;
        }

        public ProcessarComandoCommand(Guid comandoId, byte[] payload, EstadoComando ultimoEstado)
        {
            ComandoId = comandoId;
            Payload = payload;
            UltimoEstado = ultimoEstado;
            Retentativa = true;
        }
    }
}
