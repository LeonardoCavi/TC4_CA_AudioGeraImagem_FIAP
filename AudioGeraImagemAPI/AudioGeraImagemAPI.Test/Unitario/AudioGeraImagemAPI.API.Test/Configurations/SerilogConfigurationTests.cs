using AudioGeraImagemAPI.API.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace AudioGeraImagemAPI.Test.Unitario.AudioGeraImagemAPI.API.Test.Configurations
{
    public class SerilogConfigurationTests
    {
        [Fact]
        public async Task AddSerilogConfiguration_Teste_Levels()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();

            // Act
            SerilogConfiguration.AddSerilogConfiguration(services, configuration);

            // Assert
            var logger = Log.Logger;
            Assert.NotNull(logger);

            Assert.False(logger.IsEnabled(LogEventLevel.Verbose));
            Assert.False(logger.IsEnabled(LogEventLevel.Debug));
            Assert.True(logger.IsEnabled(LogEventLevel.Information));
            Assert.True(logger.IsEnabled(LogEventLevel.Warning));
            Assert.True(logger.IsEnabled(LogEventLevel.Error));
            Assert.True(logger.IsEnabled(LogEventLevel.Fatal));
        }

        [Fact]
        public async Task UseSerilogConfiguration_Teste_Sucesso()
        {
            // Arrange
            var hostBuilder = new HostBuilder();

            // Act
            hostBuilder.UseSerilogConfiguration();

            // Assert
            try
            {
                hostBuilder.Build();
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.True(false, "Falha na configuracao do Serilog");
            }
        }
    }
}