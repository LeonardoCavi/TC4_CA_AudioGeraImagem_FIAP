using AudioGeraImagemAPI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AudioGeraImagemAPI.UseCases.Comandos
{
    public class ListarComandosDTO
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
