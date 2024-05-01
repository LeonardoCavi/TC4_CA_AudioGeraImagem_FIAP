using AudioGeraImagemWorker.Worker.Events;
using MassTransit;
using Quartz;

namespace AudioGeraImagemWorker.Worker.Configurations
{
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
                        e.ConfigureConsumer<NovoComandoConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(filaRetentativa, e =>
                    {
                        e.ConfigureConsumer<RetentativaComandoConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });

                x.AddConsumer<NovoComandoConsumer>();
                x.AddConsumer<RetentativaComandoConsumer>();
            });
        }
    }
}