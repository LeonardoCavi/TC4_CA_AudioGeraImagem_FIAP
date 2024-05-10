using AudioGeraImagemWorker.Domain.Enums;
using MediatR;
using System.Text.Json.Serialization;

namespace AudioGeraImagemWorker.UseCases.Criacoes.Processar
{
    public class ProcessarCriacaoCommand : IRequest
    {
        public Guid CriacaoId { get; set; }
        public byte[] Payload { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EstadoProcessamento UltimoEstado { get; set; }
        public bool Retentativa { get; set; }

        public ProcessarCriacaoCommand(Guid criacaoId, byte[] payload)
        {
            CriacaoId = criacaoId;
            Payload = payload;
        }

        public ProcessarCriacaoCommand(Guid criacaoId, byte[] payload, EstadoProcessamento ultimoEstado)
        {
            CriacaoId = criacaoId;
            Payload = payload;
            UltimoEstado = ultimoEstado;
            Retentativa = true;
        }
    }
}
