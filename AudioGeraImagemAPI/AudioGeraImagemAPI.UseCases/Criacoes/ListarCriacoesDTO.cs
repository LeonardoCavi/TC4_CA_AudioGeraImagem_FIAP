using AudioGeraImagemAPI.Domain.Enums;
using System.Text.Json.Serialization;

namespace AudioGeraImagemAPI.UseCases.Criacoes
{
    public class ListarCriacoesDTO
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; }
        public string Transcricao { get; set; }
        public DateTime InstanteCriacao { get; set; }
        public DateTime InstanteAtualizacao { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EstadoProcessamento UltimoEstado { get; set; }
    }
}
