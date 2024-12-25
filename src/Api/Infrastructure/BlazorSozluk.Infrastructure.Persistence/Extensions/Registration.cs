using AutoMapper;
using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Api.Infrastructure.Persistence.Context;
using BlazorSozluk.Api.Infrastructure.Persistence.Repositories;
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
    public static class Registration
    {
        public static IServiceCollection AddInfrastructureRegistration(this IServiceCollection services,IConfiguration configuration)
        {
            var connStr = configuration.GetConnectionString("BlazorSozlukDbConnectionString");
            services.AddDbContext<BlazorSozlukContext>(conf =>
            {
                
                conf.UseSqlServer(connStr, opt => { opt.EnableRetryOnFailure(); });
            });

            // var seedData = new SeedData();
            //seedData.SeedAsync(configuration).GetAwaiter().GetResult();

            services.AddScoped<IUserRepository,UserRepository>();
            services.AddScoped<IEntryRepository, EntryRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IEntryCommentRepository, EntryCommentRepository>();
            services.AddScoped<IEmailConfirmationRepository, EmailConfirmationRepository>();

            return services;
        }
    }
}
