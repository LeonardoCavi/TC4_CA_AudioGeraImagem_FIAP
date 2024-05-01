using Serilog.Events;
using Serilog;

namespace AudioGeraImagemWorker.Worker.Configurations
{
    public static class SerilogConfiguration
    {
        public static void AddSerilogConfiguration(this IServiceCollection services,
                                                   IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(configuration)
                    .MinimumLevel.Override("Default", LogEventLevel.Fatal)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Fatal)
                    .MinimumLevel.Override("System", LogEventLevel.Fatal)
                .CreateLogger();
        }

        public static void UseSerilogConfiguration(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog();
        }
    }
}