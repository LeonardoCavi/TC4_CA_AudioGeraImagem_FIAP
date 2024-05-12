using Microsoft.Extensions.DependencyInjection;
using Polly.Retry;
using Polly;
using AudioGeraImagemAPI.API.Configurations;

namespace AudioGeraImagemAPI.Test.Unitario.AudioGeraImagemAPI.API.Test.Configurations
{
    public class PollyConfigurationTests
    {
        [Fact]
        public async Task AddRetryPolicy_Teste_Sucesso()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            PollyConfiguration.AddRetryPolicy(services);

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var policy = serviceProvider.GetRequiredService<AsyncPolicy>();
            Assert.NotNull(policy);
            Assert.IsType<AsyncRetryPolicy>(policy);
        }

        [Fact]
        public async Task CreateWaitAndRetryPolicy_Teste_Sucesso()
        {
            // Arrange
            var tentativas = 3;
            var count = 0;
            var services = new ServiceCollection();

            // Act
            PollyConfiguration.AddRetryPolicy(services);

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var policy = serviceProvider.GetRequiredService<AsyncPolicy>();

            
            await policy.ExecuteAsync(async () =>
            {
                count++;
                if (count < tentativas)
                {
                    throw new Exception("Mock de falha");
                }
            });

            Assert.NotNull(policy);
            Assert.Equal(tentativas, count);
        }
    }
}