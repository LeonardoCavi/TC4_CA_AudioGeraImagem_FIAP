using AudioGeraImagemWorker.Domain.Interfaces.Services;
using AudioGeraImagemWorker.Domain.Services.ProcessamentoHandler;
using System.Diagnostics.CodeAnalysis;

namespace AudioGeraImagemWorker.Worker.Configurations
{
    [ExcludeFromCodeCoverage]
    public static class ChainConfiguration
    {
        public static void AddChainConfiguration(this IServiceCollection services)
        {
            services.AddScoped<SalvarAudioHandler>();
            services.AddScoped<GerarTextoHandler>();
            services.AddScoped<GerarImagemHandler>();
            services.AddScoped<SalvarImagemHandler>();

            services.AddScoped<IProcessamentoHandler>(serviceProvider =>
            {
                var salvarAudioHandler = serviceProvider.GetRequiredService<SalvarAudioHandler>();
                var gerarTextoHandler = serviceProvider.GetRequiredService<GerarTextoHandler>();
                var gerarImagemHandler = serviceProvider.GetRequiredService<GerarImagemHandler>();
                var salvarImagemHandler = serviceProvider.GetRequiredService<SalvarImagemHandler>();

                salvarAudioHandler
                    .ProximaEtapa(gerarTextoHandler)
                    .ProximaEtapa(gerarImagemHandler)
                    .ProximaEtapa(salvarImagemHandler);

                return salvarAudioHandler;
            });
        }
    }
}
