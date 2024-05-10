using AudioGeraImagemWorker.Domain.Enums;
using System.Text.Json.Serialization;

namespace AudioGeraImagem.Domain.Messages
{
    public class RetentativaCriacaoMessage
    {
        public RetentativaCriacaoMessage(Guid criacaoId, EstadoProcessamento ultimoEstado, byte[] payload)
        {
            CriacaoId = criacaoId;
            UltimoEstado = ultimoEstado;
            Payload = payload;
        }

        public Guid CriacaoId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EstadoProcessamento UltimoEstado { get; set; }
        public byte[] Payload { get; set; }
    }
}
