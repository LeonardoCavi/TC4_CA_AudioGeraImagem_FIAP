using Microsoft.Extensions.Azure;
using System.Diagnostics.CodeAnalysis;

namespace AudioGeraImagemWorker.Worker.Configurations
{
    [ExcludeFromCodeCoverage]
    public static class AzureBlobConfiguration
    {
        public static void AddAzureBlobContainerConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetRequiredSection("AzureBlob")["ConnectionString"] ?? string.Empty;

            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(connectionString).WithName("MyBlobService");
            });
        }
    }
}
