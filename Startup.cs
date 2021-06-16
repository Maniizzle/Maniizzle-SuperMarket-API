using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Supermarket.API.Domain.Repositories;
using Supermarket.API.Domain.Services;
using Supermarket.API.Persistence.Context;
using Supermarket.API.Persistence.Repositories;
using Supermarket.API.Services;
using Microsoft.Extensions.DependencyModel;
using Supermarket.API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Supermarket.API.Persistence.Data;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;

namespace Supermarket.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:token").Value);

            services.AddTransient<Seed>();
            services.AddAutoMapper(typeof(Startup));

            //redis
            // services.AddSingleton<IConnectionMultiplexer>(x => ConnectionMultiplexer.Connect(Configuration.GetValue<string>("RedisConnection")));
            services.AddSingleton<ICacheService, RedisCacheService>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetValue<string>("RedisConnection");
                options.InstanceName = "RedisDemo__";
            });
            
            //caching
            services.AddResponseCaching();
            string envVar = Environment.GetEnvironmentVariable("DATABASE_URL");
            string connectionString = null;
            if (string.IsNullOrEmpty(envVar))
            {
                connectionString = Configuration["Connectionstrings:database"];
            }
            else
            {
                //parse database URL. Format is postgres://<username>:<password>@<host>/<dbname>
                var uri = new Uri(envVar);
                var ddd = uri.ToString().Split('@');
                var host = ddd[1].Split(':')[0];
                var username = uri.UserInfo.Split(':')[0];
                var password = uri.UserInfo.Split(':')[1];
                connectionString ="Server="+host+ "; Database=" + uri.AbsolutePath.Substring(1) + "; Username=" + username +
                "; Password=" + password + "; Port=" + uri.Port +
                "; SSL Mode=Require; Trust Server Certificate=true;";
            }
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                // options.UseInMemoryDatabase(("supermarket-api-in-memory"));
                // options.UseSqlite(Configuration.GetConnectionString("Default"));
            });
            // services.AddDbContext<AppDbContext>(x => x.

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped(typeof(IEntityRepository<>), typeof(EntityRepository<>));
            services.AddScoped<IDatingRepository, DatingRepository>();//, typeof(EntityRepository<>));
            services.AddScoped<IAuthRepository, AuthRepository>();

           
            services.AddHttpCacheHeaders((expirationModelConfig)=> 
            {
                //Global Registeration
                expirationModelConfig.MaxAge = 60;
                expirationModelConfig.CacheLocation = Marvin.Cache.Headers.CacheLocation.Public;
            },
            (validationModelConfig)=>
            {
                validationModelConfig.MustRevalidate = true;
            });
            services.AddOptions();
            services.AddCors(c =>
            {
                c.AddPolicy("FirstCor", coo =>
                {
                    coo.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });
            services.AddSwagger();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                     {
                         ValidateIssuerSigningKey = true,
                         IssuerSigningKey = new SymmetricSecurityKey(key),
                         ValidateIssuer = false,
                         ValidateAudience = false
                     };
                 });

            services.AddMvc(options=>
            {
                //configure a profile for Cache
                options.CacheProfiles.Add("240CacheProfile", new CacheProfile { Duration = 240 });
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //use to store counters and rule for rate limiting
            services.AddMemoryCache();
            services.AddInMemoryRateLimiting();
            services.Configure<IpRateLimitOptions>((options) =>
            {

                options.GeneralRules = new List<RateLimitRule>
                {
                new RateLimitRule()
                {
                    Endpoint = "*",
                    Limit = 7,
                    Period = "5m"
                }
            };
                //new RateLimitRule()
                //{
                //    Endpoint = "*",
                //    Limit = 3,
                //    Period = "10s"
                //};
            });
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();

            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>(); //register as singleton so as to track all policies and rate counter across request 
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seed seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            seeder.SeedData();
            app.UseCustomSwaggerApi();

            app.UseIpRateLimiting();
            app.UseCors("FirstCor");
            //order of arrangement matters
            app.UseResponseCaching();
            app.UseHttpCacheHeaders();
            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}