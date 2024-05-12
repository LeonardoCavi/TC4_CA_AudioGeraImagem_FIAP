using AudioGeraImagemAPI.API.Configurations;
using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using AudioGeraImagemAPI.Domain.Utility;
using AudioGeraImagemAPI.Infra;
using AudioGeraImagemAPI.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Polly;

namespace AudioGeraImagemAPI.Test.Unitario.AudioGeraImagemAPI.API.Test.Configurations
{
    public class DependencyInjectionConfigurationTests
    {
        [Fact]
        public void AddDependencyInjection_Teste_RegistrosDP()
        {
            // Arrange
            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase"));

            services.AddSingleton(typeof(ILogger<HttpHelper>), NullLogger<HttpHelper>.Instance);
            services.AddSingleton<AsyncPolicy>(Policy.NoOpAsync());

            // Act
            DependencyInjectionConfiguration.AddDepencyInjection(services);

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var criacaoRepository = serviceProvider.GetService<ICriacaoRepository>();
            var httpHelper = serviceProvider.GetService<HttpHelper>();

            Assert.NotNull(criacaoRepository);
            Assert.NotNull(httpHelper);
            Assert.IsType<CriacaoRepository>(criacaoRepository);
            Assert.IsType<HttpHelper>(httpHelper);
        }
    }
}