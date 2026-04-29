using Microsoft.Extensions.DependencyInjection;
using Repository.Entities;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public static class ExtensionRepository
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IRepository<Call>, CallRepository>();
            services.AddScoped<ICallRepository<Call>, CallRepository>();
            //services.AddScoped<IRepository<CallParticipantAnalysis>, CallParticipantAnalysisRepository>();
            services.AddScoped<IRepository<Company>,CompanyRepository >();
            services.AddScoped<IRepository<Score>, ScoreRepository>();
            services.AddScoped<IRepository<Operator>, OperatorRepository>();

            return services;
        }
    }
}
