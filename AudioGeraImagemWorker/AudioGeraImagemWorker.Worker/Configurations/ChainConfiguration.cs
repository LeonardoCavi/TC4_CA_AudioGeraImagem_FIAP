using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using AudioGeraImagemWorker.Domain.Interfaces.Services;
using AudioGeraImagemWorker.Domain.Interfaces.Utility;
using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
using AudioGeraImagemWorker.Domain.Services.ProcessamentoHandler;

namespace AudioGeraImagemWorker.Worker.Configurations
{
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

            //services.AddScoped<IProcessamentoHandler>(serviceProvider =>
            //{
            //    var bucketManager = serviceProvider.GetRequiredService<IBucketManager>();
            //    var httpHelper = serviceProvider.GetRequiredService<IHttpHelper>();
            //    var criacaoRepository = serviceProvider.GetRequiredService<ICriacaoRepository>();
            //    var openAiVendor = serviceProvider.GetRequiredService<IOpenAIVendor>();
            //    var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            //    var salvarAudioHandler = new SalvarAudioHandler(
            //        loggerFactory.CreateLogger<SalvarAudioHandler>(),
            //        bucketManager,
            //        criacaoRepository);

            //    var gerarTextoHandler = new GerarTextoHandler(
            //        loggerFactory.CreateLogger<GerarTextoHandler>(),
            //        openAiVendor,
            //        httpHelper,
            //        criacaoRepository);

            //    var gerarImagemHandler = new GerarImagemHandler(
            //        loggerFactory.CreateLogger<GerarImagemHandler>(),
            //        openAiVendor,
            //        criacaoRepository);

            //    var salvarImagemHandler = new SalvarImagemHandler(
            //        loggerFactory.CreateLogger<SalvarImagemHandler>(),
            //        bucketManager,
            //        httpHelper,
            //        criacaoRepository
            //        );

            //    salvarAudioHandler
            //        .ProximaEtapa(gerarTextoHandler)
            //        .ProximaEtapa(gerarImagemHandler)
            //        .ProximaEtapa(salvarImagemHandler);

            //    return salvarAudioHandler;
            //});
        }
    }
}
