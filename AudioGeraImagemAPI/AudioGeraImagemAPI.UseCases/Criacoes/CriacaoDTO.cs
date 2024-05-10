using AudioGeraImagemAPI.Domain.Entities;

namespace AudioGeraImagemAPI.UseCases.Criacoes
{
    public class CriacaoDTO
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; }
        public string UrlAudio { get; set; }
        public string Transcricao { get; set; }
        public string UrlImagem { get; set; }
        public DateTime InstanteCriacao { get; set; }
        public DateTime InstanteAtualizacao { get; set; }
        public List<ProcessamentoCriacao> ProcessamentosCriacao { get; set; }
    }
}
