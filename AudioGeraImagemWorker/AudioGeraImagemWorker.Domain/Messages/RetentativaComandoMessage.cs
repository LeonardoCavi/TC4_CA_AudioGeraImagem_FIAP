using AudioGeraImagemWorker.Domain.Enums;
using System.Text.Json.Serialization;

namespace AudioGeraImagem.Domain.Messages
{
    public class RetentativaComandoMessage
    {
        public Guid ComandoId { get; set; }
        public byte[] Payload { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EstadoComando UltimoEstado { get; set; }
    }
}
