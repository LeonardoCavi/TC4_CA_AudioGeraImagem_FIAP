namespace AudioGeraImagemWorker.Domain.Interfaces.Vendor
{
    public interface IOpenAIVendor
    {
        Task<string> GerarTranscricao(byte[] bytes);

        Task<string> GerarImagem(string prompt);
    }
}