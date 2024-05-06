namespace AudioGeraImagem.Domain.Messages
{
    public class ComandoMessage
    {
        public Guid ComandoId { get; set; }
        public byte[] Payload { get; set; }

        public ComandoMessage()
        {

        }

        public ComandoMessage(Guid comandoId, byte[] payload)
        {
            ComandoId = comandoId;
            Payload = payload;
        }
    }
}
