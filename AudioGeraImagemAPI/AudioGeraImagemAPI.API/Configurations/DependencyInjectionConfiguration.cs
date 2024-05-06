using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using AudioGeraImagemAPI.Domain.Utility;
using AudioGeraImagemAPI.Infra.Repositories;

namespace AudioGeraImagemAPI.API.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDepencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IComandoRepository, ComandoRepository>();
            services.AddScoped<HttpHelper>();
        }
    }
}
