using AudioGeraImagemWorker.Infra.Vendor.Entities.AzureBlob;
using AudioGeraImagemWorker.Infra.Vendor.Entities.OpenAI;

namespace AudioGeraImagemWorker.Worker.Configurations
{
    public static class ParametersConfiguration
    {
        public static void AddParameters(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration.GetRequiredSection("AzureBlob").Get<AzureBlobParameters>());
            services.AddSingleton(configuration.GetRequiredSection("OpenAI").Get<OpenAIParameters>());
        }
    }
}
