﻿using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using AudioGeraImagemWorker.Domain.Interfaces.Utility;
using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
using AudioGeraImagemWorker.Domain.Utility;
using AudioGeraImagemWorker.Infra.Repositories;
using AudioGeraImagemWorker.Infra.Vendor;
using System.Diagnostics.CodeAnalysis;

namespace AudioGeraImagemWorker.Worker.Configurations
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjectionConfiguration
    {
        public static void AddDepencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IBucketManager, BucketManager>();
            services.AddScoped<IOpenAIVendor, OpenAIVendor>();
            services.AddScoped<ICriacaoRepository, CriacaoRepository>();
            services.AddScoped<IHttpHelper, HttpHelper>();
        }
    }
}