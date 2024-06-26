﻿using AudioGeraImagemAPI.Domain.Enums;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace AudioGeraImagemAPI.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public class ProcessamentoCriacao
    {
        public Guid Id { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EstadoProcessamento Estado { get; set; }
        public DateTime InstanteCriacao { get; set; }
        public string MensagemErro { get; set; }
    }
}
