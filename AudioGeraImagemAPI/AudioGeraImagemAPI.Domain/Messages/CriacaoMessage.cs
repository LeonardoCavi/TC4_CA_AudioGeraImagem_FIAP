using System.Diagnostics.CodeAnalysis;

namespace AudioGeraImagem.Domain.Messages
{
    [ExcludeFromCodeCoverage]
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
