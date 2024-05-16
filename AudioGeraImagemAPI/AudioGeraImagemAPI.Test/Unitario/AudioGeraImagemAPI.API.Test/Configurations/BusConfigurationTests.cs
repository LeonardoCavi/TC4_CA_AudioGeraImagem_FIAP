using AudioGeraImagemAPI.API.Configurations;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AudioGeraImagemAPI.Test.Unitario.AudioGeraImagemAPI.API.Test.Configurations
{
    public class BusConfigurationTests
    {
        [Fact]
        public void AddBusConfiguration_CorrectlyConfiguresMassTransit()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // Act
            BusConfiguration.AddBusConfiguration(services, configuration);

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var busControl = serviceProvider.GetService<IBusControl>();
            Assert.NotNull(busControl);
        }
    }
}