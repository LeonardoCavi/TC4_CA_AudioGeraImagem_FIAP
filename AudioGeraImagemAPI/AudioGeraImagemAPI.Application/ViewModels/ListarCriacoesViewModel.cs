using AudioGeraImagemAPI.Domain.Enums;
using System.Text.Json.Serialization;

namespace AudioGeraImagemAPI.Application.ViewModels
{
    public class ListarCriacaoViewModel
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; }
        public string Transcricao { get; set; }
        public DateTime InstanteCriacao { get; set; }
        public DateTime InstanteAtualizacao { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EstadoComando UltimoEstado { get; set; }
    }
}
