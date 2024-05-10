﻿using AudioGeraImagemWorker.Domain.Interfaces;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using AudioGeraImagemWorker.Domain.Interfaces.Utility;
using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
using AudioGeraImagemWorker.Domain.Services;
using AudioGeraImagemWorker.Domain.Utility;
using AudioGeraImagemWorker.Infra.Repositories;
using AudioGeraImagemWorker.Infra.Vendor;

namespace AudioGeraImagemWorker.Worker.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDepencyInjection(this IServiceCollection services)
        {
            // Worker
            services.AddScoped<IErroManager, ErroManager>();
            // Domain - Vendors
            services.AddScoped<IBucketManager, BucketManager>();
            services.AddScoped<IOpenAIVendor, OpenAIVendor>();
            //Repository
            services.AddScoped<ICriacaoRepository, CriacaoRepository>();
            //Utility HttpClient
            services.AddScoped<IHttpHelper, HttpHelper>();
        }
    }
}