using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Domain.Enums;

namespace AudioGeraImagem.Domain.Entities
{
    public class Criacao : EntidadeBase
    {
        public string Descricao { get; set; }
        public string UrlAudio { get; set; }
        public string Transcricao { get; set; }
        public string UrlImagem { get; set; }
        public DateTime InstanteCriacao { get; set; }
        public DateTime InstanteAtualizacao { get; set; }
        public List<ProcessamentoCriacao> ProcessamentosCriacao { get; set; }

        public Criacao() { }
    }
}