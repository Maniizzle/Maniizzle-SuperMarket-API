using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Supermarket.API.Persistence.Context;

//using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supermarket.API.Extensions
{
    public static class SwaggerExtension
    {
        public static void UseCustomSwaggerApi(this IApplicationBuilder app, string name = "SuperMarket V1")
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Enable middleware to serve swagger - ui assets(HTML, JS, CSS etc.)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", name);
                c.DocExpansion(DocExpansion.None);
                c.RoutePrefix = string.Empty;
            });
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services, string title = "SuperMarket V1")
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = title, Version = "v1" });

                var securityScheme = new OpenApiSecurityScheme()
                {
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Type = SecuritySchemeType.ApiKey,
                    Description = "Jwt Authorization header using the bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", securityScheme);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securityScheme, new List<string>() }
                };

                c.AddSecurityRequirement(securityRequirement);

                //var security = new Dictionary<string, IEnumerable<string>>
                //{
                //    {"Bearer", new string[] { }},
                //};
                //c.OperationFilter<SecurityRequirementsOperationFilter>();
                //c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                //{
                //    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                //    In = ParameterLocation.Header,
                //    Name = "Authorization",
                //    Type = SecuritySchemeType.ApiKey
                //});
            });
            return services;
        }
    }
}