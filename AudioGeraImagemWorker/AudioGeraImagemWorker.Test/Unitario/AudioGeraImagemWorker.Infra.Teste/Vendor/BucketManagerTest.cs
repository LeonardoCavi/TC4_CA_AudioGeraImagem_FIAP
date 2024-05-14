using AudioGeraImagemWorker.Infra.Vendor;
using AudioGeraImagemWorker.Infra.Vendor.Entities.AzureBlob;
using AutoFixture;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using NSubstitute;
using System.Text;

namespace AudioGeraImagemWorker.Test.Unitario.AudioGeraImagemWorker.Infra.Teste.Vendor
{
    public class BucketManagerTest
    {
        private readonly IFixture _fixture;
        private readonly IAzureClientFactory<BlobServiceClient> _azureClientFactoryMock;
        private readonly AzureBlobParameters _parameters;

        public BucketManagerTest()
        {
            _fixture = new Fixture();
            _azureClientFactoryMock = Substitute.For<IAzureClientFactory<BlobServiceClient>>();
            _parameters = new AzureBlobParameters()
            {
                ConnectionString = "connectionString",
                ContainerName = "openapi-files"
            };

            _fixture.Register(() => _azureClientFactoryMock);
            _fixture.Register(() => _parameters);
        }

        [Fact]
        public async Task ArmazenarObjeto_Sucesso()
        {
            //Arrange
            var bytes = Encoding.UTF8.GetBytes("buffer teste");
            var blobName = "blob_name";

            var blobClientMock = Substitute.For<BlobClient>();
            blobClientMock.Uri.Returns(new Uri("http://blob_uri/"));

            var blobContainerClientMock = Substitute.For<BlobContainerClient>();
            blobContainerClientMock
                .GetBlobClient(Arg.Is(blobName))
                .Returns(blobClientMock);

            var blobServiceClientMock = Substitute.For<BlobServiceClient>();
            blobServiceClientMock
                .GetBlobContainerClient(Arg.Is("openapi-files"))
                .Returns(blobContainerClientMock);

            _azureClientFactoryMock.CreateClient(Arg.Any<string>())
                .Returns(blobServiceClientMock);

            var manager = _fixture.Create<BucketManager>();

            //Act
            var resultado = await manager.ArmazenarObjeto(bytes, blobName);

            //Assert
            Assert.Equal("http://blob_uri/", resultado);
            await blobClientMock.Received().UploadAsync(Arg.Any<BinaryData>(), overwrite: true);
        }
    }
}
