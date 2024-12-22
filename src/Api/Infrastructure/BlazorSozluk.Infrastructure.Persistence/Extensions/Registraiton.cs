using BlazorSozluk.Api.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Api.Infrastructure.Persistence.Extensions
{
    public static class Registraiton
    {
        public static IServiceCollection AddInfrastructureRegistration(this IServiceCollection services,IConfiguration configuration)
        {
            var connStr = configuration["BlazorSozlukDbConnecitonString"].ToString();
            services.AddDbContext<BlazorSozlukContext>(conf =>
            {
                
                conf.UseSqlServer(connStr, opt => { opt.EnableRetryOnFailure(); });
            });
            return services;
        }
    }
}
