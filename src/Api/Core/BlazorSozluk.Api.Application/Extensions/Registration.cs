﻿using Microsoft.Extensions.DependencyInjection;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace BlazorSozluk.Api.Application.Extensions
{
    public static class Registration
    {
        public static IServiceCollection AddApplicationRegistration(this IServiceCollection services)
        {
            var assm = Assembly.GetExecutingAssembly();

            services.AddMediatR(cfg=>cfg.RegisterServicesFromAssembly(assm));
            services.AddAutoMapper(assm);
            services.AddValidatorsFromAssembly(assm);

            return services;
        }
    }
}
