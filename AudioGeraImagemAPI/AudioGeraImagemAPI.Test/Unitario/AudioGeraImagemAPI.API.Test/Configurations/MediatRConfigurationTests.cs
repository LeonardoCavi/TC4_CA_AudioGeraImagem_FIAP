using AudioGeraImagemAPI.API.Configurations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AudioGeraImagemAPI.Test.Unitario.AudioGeraImagemAPI.API.Test.Configurations
{
    public class MediatRConfigurationTests
    {
        [Fact]
        public async Task AddMediatRConfiguration_Teste_Sucesso()
        {
            // Arrange
            var services = new ServiceCollection();
     
            // Act
            MediatRConfiguration.AddMediatRConfiguration(services);

            // Assert
            Assert.True(services.BuildServiceProvider().GetRequiredService<IMediator>() != null);
        }
    }
}