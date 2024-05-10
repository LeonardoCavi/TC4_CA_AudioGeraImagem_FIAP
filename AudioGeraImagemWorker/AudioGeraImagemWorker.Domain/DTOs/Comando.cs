using AudioGeraImagem.Domain.Entities;

namespace AudioGeraImagemWorker.Domain.DTOs
{
    public class Comando
    {
        public Criacao Criacao { get; set; }
        public byte[] Payload { get; set; }
    }
}
