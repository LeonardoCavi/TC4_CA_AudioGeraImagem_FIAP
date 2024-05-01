namespace AudioGeraImagemWorker.Domain.Interfaces.Vendor
{
    public interface IBucketManager
    {
        Task<string> ArmazenarObjeto(byte[] bytes, string blobName);
    }
}