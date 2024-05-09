using AudioGeraImagemWorker.UseCases.Comandos.Processar;

namespace AudioGeraImagemWorker.Worker.Configurations
{
    public static class MediatRConfiguration
    {
        public static void AddMediatRConfiguration(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(typeof(ProcessarComandoCommand).Assembly);
            });
        }
    }
}
