using AudioGeraImagemWorker.Infra;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace AudioGeraImagemWorker.Worker.Configurations
{
    [ExcludeFromCodeCoverage]
    public static class DbContextConfiguration
    {
        public static void AddDbContextConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("ApplicationConnectionString"));
            });
        }
    }
}
