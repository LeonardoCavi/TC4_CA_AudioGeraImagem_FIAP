using AudioGeraImagemWorker.Worker.Events;
using MassTransit;
using System.Diagnostics.CodeAnalysis;

namespace AudioGeraImagemWorker.Worker.Configurations
{
    [ExcludeFromCodeCoverage]
    public static class BusConfiguration
    {
        public static void AddBusConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var massTransitParameters = configuration.GetRequiredSection("MassTransit");

            var fila = massTransitParameters["Fila"] ?? string.Empty;
            var filaRetentativa = massTransitParameters["FilaRetentativa"] ?? string.Empty;
            var servidor = massTransitParameters["Servidor"] ?? string.Empty;
            var usuario = massTransitParameters["Usuario"] ?? string.Empty;
            var senha = massTransitParameters["Senha"] ?? string.Empty;

            services.AddMassTransit(x =>
            {
                x.AddDelayedMessageScheduler();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(servidor, "/", h =>
                    {
                        h.Username(usuario);
                        h.Password(senha);
                    });

                    cfg.UseDelayedMessageScheduler();

                    cfg.ReceiveEndpoint(fila, e =>
                    {
                        e.ConfigureConsumer<NovaCriacaoConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(filaRetentativa, e =>
                    {
                        e.ConfigureConsumer<RetentativaCriacaoConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });

                x.AddConsumer<NovaCriacaoConsumer>();
                x.AddConsumer<RetentativaCriacaoConsumer>();
            });
        }
    }
}