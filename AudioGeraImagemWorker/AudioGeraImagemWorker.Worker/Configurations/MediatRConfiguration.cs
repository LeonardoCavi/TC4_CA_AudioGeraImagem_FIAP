using AudioGeraImagemWorker.UseCases.Criacoes.Processar;
using System.Diagnostics.CodeAnalysis;

namespace AudioGeraImagemWorker.Worker.Configurations
{
    [ExcludeFromCodeCoverage]
    public static class MediatRConfiguration
    {
        public static void AddMediatRConfiguration(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(typeof(ProcessarCriacaoCommand).Assembly);
            });
        }
    }
}
