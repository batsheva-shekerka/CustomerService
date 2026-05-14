using Common.Dto;
using Microsoft.Extensions.DependencyInjection;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository;
using Service.Interfaces;
using Repository.Repositories;
using Repository.Entities;
using Azure.AI.TextAnalytics;
using Azure;
using AutoMapper;
using DataContext;
using Repository.Interfaces;
using Microsoft.Extensions.Configuration;


namespace Service.Services
{
    public static class ExtensionService
    {
        
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRepository();
            services.AddScoped<Iservice<OperatorDto>, OperatorService>();
            //services.AddScoped<IsExist<Operator>, OperatorService>();
            services.AddScoped<OperatorService>();
            //services.AddScoped<IsExist<Operator>, OperatorService>();
            services.AddScoped<IAuthService, AuthService>();
            //services.AddScoped<IsExist<Operator>, AuthService>();
            //services.AddScoped<Iservice<CallDto>, CallService>();
            services.AddScoped<Iservice<CompanyDto>, CompanyService>();
            services.AddScoped<CompanyService>();
            services.AddScoped<Iservice<CallDto>, CallService>();
            services.AddScoped<ICallService<CallDto>, CallService>();
            services.AddScoped<IOperatorService, OperatorService>();
            services.AddScoped<ICompanyService, CompanyService>();



            var endpoint = new Uri(configuration["AzureServices:TextAnalyticsEndpoint"]);
            var credentials = new AzureKeyCredential(configuration["AzureServices:TextAnalyticsKey"]);

            services.AddSingleton(new TextAnalyticsClient(endpoint, credentials));
            // 1. רישום ה-Service הרגיל (Scoped - נוצר בכל בקשה)
            services.AddScoped<CallAnalysisService>();

            // 2. רישום ה-Worker (Singleton - רץ תמיד ברקע)
            services.AddHostedService<FolderWatcherWorker>();
            //services.AddScoped<IAuthService, AuthService>();
            return services;
        }

    }
}
