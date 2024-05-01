using AudioGeraImagemWorker.Application.Interfaces;
using AudioGeraImagemWorker.Application.Services;
using AudioGeraImagemWorker.Domain.Interfaces;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
using AudioGeraImagemWorker.Domain.Services;
using AudioGeraImagemWorker.Domain.Utility;
using AudioGeraImagemWorker.Infra.Repositories;
using AudioGeraImagemWorker.Infra.Vendor;
using AudioGeraImagemWorker.Worker.Events;
using Polly;

namespace AudioGeraImagemWorker.Worker.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDepencyInjection(this IServiceCollection services)
        {
            // Worker
            services.AddScoped<NovoComandoConsumer>();
            // Application
            services.AddScoped<IEventReceiver, EventReceiver>();
            // Domain
            services.AddScoped<IComandoManager, ComandoManager>();
            services.AddScoped<IErroManager, ErroManager>();
            // Domain - Vendors
            services.AddScoped<IBucketManager, BucketManager>();
            services.AddScoped<IOpenAIVendor, OpenAIVendor>();
            //Repository
            services.AddScoped<IComandoRepository, ComandoRepository>();
            //Utility HttpClient
            services.AddScoped<HttpHelper>();
        }
    }
}