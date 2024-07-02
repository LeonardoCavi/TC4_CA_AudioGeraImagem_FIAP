using AudioGeraImagemWorker.Worker;
using AudioGeraImagemWorker.Worker.Configurations;
using System.Diagnostics.CodeAnalysis;

var hostBuilder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;

        services.AddRetryPolicy();
        services.AddHttpClient();
        services.AddParameters(configuration);
        services.AddDepencyInjection();
        services.AddSerilogConfiguration(configuration);
        services.AddBusConfiguration(configuration);
        services.AddDbContextConfiguration(configuration);
        services.AddAzureBlobContainerConfiguration(configuration);
        services.AddMediatRConfiguration();
        services.AddChainConfiguration();
        services.AddHostedService<Worker>();
    });

hostBuilder.UseSerilogConfiguration();
var host = hostBuilder.Build();
host.Run();

[ExcludeFromCodeCoverage]
public static partial class Program { }