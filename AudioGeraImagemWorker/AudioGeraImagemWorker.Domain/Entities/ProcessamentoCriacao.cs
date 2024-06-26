﻿using AudioGeraImagemWorker.Domain.Enums;
using System.Text.Json.Serialization;

namespace AudioGeraImagemWorker.Domain.Entities
{
    public class ProcessamentoCriacao
    {
        public Guid Id { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EstadoProcessamento Estado { get; set; }
        public DateTime InstanteCriacao { get; set; }
        public string MensagemErro { get; set; }

        public ProcessamentoCriacao() { }

        public ProcessamentoCriacao(EstadoProcessamento estado)
        {
            Estado = estado;
            InstanteCriacao = DateTime.Now;
        }
    }
}