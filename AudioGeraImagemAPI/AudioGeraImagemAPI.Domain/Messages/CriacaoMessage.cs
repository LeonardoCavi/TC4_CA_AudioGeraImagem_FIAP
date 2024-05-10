namespace AudioGeraImagem.Domain.Messages
{
    public class CriacaoMessage
    {
        public Guid CriacaoId { get; set; }
        public byte[] Payload { get; set; }

        public CriacaoMessage()
        {

        }

        public CriacaoMessage(Guid criacaoId, byte[] payload)
        {
            CriacaoId = criacaoId;
            Payload = payload;
        }
    }
}
