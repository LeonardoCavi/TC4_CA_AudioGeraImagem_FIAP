using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
using AudioGeraImagemWorker.Infra.Vendor.Entities.AzureBlob;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AudioGeraImagemWorker.Infra.Vendor
{
    public class BucketManager : IBucketManager
    {
        private readonly AzureBlobParameters _parameters;

        public BucketManager(AzureBlobParameters parameters)
        {
            _parameters = parameters;
        }

        private async Task<BlobClient> GetBlobClient(string blobName)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_parameters.ConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_parameters.ContainerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
            return containerClient.GetBlobClient(blobName);
        }

        public async Task<string> ArmazenarObjeto(byte[] bytes, string blobName)
        {
            var blobClient = await GetBlobClient(blobName);
            await blobClient.UploadAsync(BinaryData.FromBytes(bytes), overwrite: true);
            return blobClient.Uri.ToString();
        }
    }
}