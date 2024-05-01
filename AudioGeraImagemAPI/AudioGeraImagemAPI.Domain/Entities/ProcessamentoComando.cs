using AudioGeraImagemAPI.Domain.Enums;
using System.Text.Json.Serialization;

namespace AudioGeraImagemAPI.Domain.Entities
{
    public class ProcessamentoComando
    {
        public Guid Id { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EstadoComando Estado { get; set; }
        public DateTime InstanteCriacao { get; set; }
        public string MensagemErro { get; set; }
    }
}
