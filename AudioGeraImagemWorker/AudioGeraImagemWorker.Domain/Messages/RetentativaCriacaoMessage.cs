using AudioGeraImagemWorker.Domain.Enums;
using System.Text.Json.Serialization;

namespace AudioGeraImagem.Domain.Messages
{
    public class RetentativaCriacaoMessage
    {
        public Guid CriacaoId { get; set; }
        public byte[] Payload { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EstadoProcessamento UltimoEstado { get; set; }
    }
}
