using AudioGeraImagemAPI.UseCases.Criacoes.Create;

namespace AudioGeraImagemAPI.API.Configurations
{
    public static class MediatRConfiguration
    {
        public static void AddMediatRConfiguration(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(typeof(CriarCriacaoCommand).Assembly);
            });
        }
    }
}
