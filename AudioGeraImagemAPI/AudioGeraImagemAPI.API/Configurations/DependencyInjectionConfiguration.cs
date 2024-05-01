using AudioGeraImagemAPI.Application.Intefaces;
using AudioGeraImagemAPI.Application.Services;
using AudioGeraImagemAPI.Domain.Interfaces;
using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using AudioGeraImagemAPI.Domain.Services;
using AudioGeraImagemAPI.Domain.Utility;
using AudioGeraImagemAPI.Infra.Repositories;

namespace AudioGeraImagemAPI.API.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDepencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IComandoRepository, ComandoRepository>();
            services.AddScoped<IComandoApplicationService, ComandoApplicationService>();
            services.AddScoped<IComandoService, ComandoService>();
            services.AddScoped<HttpHelper>();
        }
    }
}
