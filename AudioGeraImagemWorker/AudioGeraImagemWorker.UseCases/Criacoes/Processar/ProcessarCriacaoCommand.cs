using AudioGeraImagemWorker.Domain.Enums;
using MediatR;
using System.Text.Json.Serialization;

namespace AudioGeraImagemWorker.UseCases.Criacoes.Processar
{
    public class ProcessarCriacaoCommand : IRequest
    {
        public Guid CriacaoId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EstadoProcessamento UltimoEstado { get; set; }
        public byte[] Payload { get; set; }
        public bool Retentativa { get; set; }

        public ProcessarCriacaoCommand(Guid criacaoId, byte[] payload)
        {
            CriacaoId = criacaoId;
            Payload = payload;
        }

        public ProcessarCriacaoCommand(Guid criacaoId, EstadoProcessamento ultimoEstado, byte[] payload)
        {
            CriacaoId = criacaoId;
            UltimoEstado = ultimoEstado;
            Payload = payload;
            Retentativa = true;
        }
    }
}
