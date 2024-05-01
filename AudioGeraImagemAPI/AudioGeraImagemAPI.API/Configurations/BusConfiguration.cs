using MassTransit;

namespace AudioGeraImagemAPI.API.Configurations
{
    public static class BusConfiguration
    {
        public static void AddBusConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var massTransitParameters = configuration.GetRequiredSection("MassTransit");

            var servidor = massTransitParameters["Servidor"] ?? string.Empty;
            var usuario = massTransitParameters["Usuario"] ?? string.Empty;
            var senha = massTransitParameters["Senha"] ?? string.Empty;

            services.AddMassTransit(cfg =>
            {
                cfg.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(servidor, "/", h =>
                    {
                        h.Username(usuario);
                        h.Password(senha);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}
