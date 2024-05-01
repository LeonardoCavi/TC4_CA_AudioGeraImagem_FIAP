namespace AudioGeraImagemAPI.Domain.Entities
{
    public class Comando : EntidadeBase
    {
        public string Descricao { get; set; }
        public string UrlAudio { get; set; }
        public string Transcricao { get; set; }
        public string UrlImagem { get; set; }
        public DateTime InstanteCriacao { get; set; }
        public DateTime InstanteAtualizacao { get; set; }
        public List<ProcessamentoComando> ProcessamentosComandos { get; set; }
    }
}
