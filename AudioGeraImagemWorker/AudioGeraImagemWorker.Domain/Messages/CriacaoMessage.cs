namespace AudioGeraImagem.Domain.Messages
{
    public class CriacaoMessage
    {
        public Guid CriacaoId { get; set; }
        public byte[] Payload { get; set; }
    }
}
