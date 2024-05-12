using AudioGeraImagemAPI.API.Configurations;
using AudioGeraImagemAPI.Infra;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AudioGeraImagemAPI.Test.Unitario.AudioGeraImagemAPI.API.Test.Configurations
{
    public class DbContextConfigurationTests
    {
        [Fact]
        public void AddDbContextConfiguration_AddsDbContextWithCorrectConnectionString()
        {
            // Arrange
            var service = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json") 
                .Build();
            service.AddSingleton<IConfiguration>(configuration);

            // Act
            DbContextConfiguration.AddDbContextConfiguration(service, configuration);
            var serviceProvider = service.BuildServiceProvider();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Assert
            var dbContextOptions = dbContext.Database.GetDbConnection().ConnectionString;
            Assert.Equal(configuration.GetConnectionString("ApplicationConnectionString"), dbContextOptions);
        }
    }
}