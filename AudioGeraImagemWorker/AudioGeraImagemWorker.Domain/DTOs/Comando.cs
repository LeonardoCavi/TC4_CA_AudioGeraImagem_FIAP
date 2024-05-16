using AudioGeraImagem.Domain.Entities;

namespace AudioGeraImagemWorker.Domain.DTOs
{
    public class Comando
    {
        public Comando(Criacao criacao, byte[] payload)
        {
            Criacao = criacao;
            Payload = payload;
        }

        public Criacao Criacao { get; set; }
        public byte[] Payload { get; set; }
    }
}
