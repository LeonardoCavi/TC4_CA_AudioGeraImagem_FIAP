using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using AudioGeraImagemAPI.Domain.Interfaces.Utility;
using AudioGeraImagemAPI.Domain.Utility;
using AudioGeraImagemAPI.Infra.Repositories;

namespace AudioGeraImagemAPI.API.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDepencyInjection(this IServiceCollection services)
        {
            services.AddScoped<ICriacaoRepository, CriacaoRepository>();
            services.AddScoped<IHttpHelper, HttpHelper>();
        }
    }
}
