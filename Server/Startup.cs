﻿using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CounterCulture.Repositories;
using CounterCulture.Services;
using CounterCulture.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Test;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CounterCulture.Configuration;

namespace CounterCulture
{
  public class Startup
    {
        public Startup(IConfiguration configuration, ILoggerFactory _LoggerFactory, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            env = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment env { get; set; }
        private string mySqlConnectionString {
            get {
                return Configuration["ConnectionStrings:DefaultMySQLConnection"];
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.LoginPath = "/account/login";
                options.AccessDeniedPath = "/";
                options.SlidingExpiration = true;
            });

            services.AddApiVersioning();

            services.AddMvc()
                    .AddJsonOptions(options => {
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    });
            
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var signingKey = Encoding.ASCII
                            .GetBytes(Configuration["AppSecret"]);
            
            var _tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(signingKey),
                    ValidateIssuer = true,
                    ValidIssuer = env.IsDevelopment() 
                                  ? ServerUrls.SECURE[ENV.DEV] 
                                  : ServerUrls.SECURE[ENV.PROD],
                    ValidateAudience = true,
                    ValidAudience = env.IsDevelopment() 
                                  ? $"{ServerUrls.SECURE[ENV.DEV]}/resources" 
                                  : $"{ServerUrls.SECURE[ENV.PROD]}/resources",
                };

            services.AddAuthentication()
            .AddCookie(options => options.SlidingExpiration = true)
            .AddJwtBearer(options =>
            {
                options.Authority = env.IsDevelopment() 
                                  ? ServerUrls.SECURE[ENV.DEV] 
                                  : ServerUrls.SECURE[ENV.PROD];
                options.Audience = env.IsDevelopment() 
                                  ? $"{ServerUrls.SECURE[ENV.DEV]}/resources" 
                                  : $"{ServerUrls.SECURE[ENV.PROD]}/resources";
                options.RequireHttpsMetadata = env.IsProduction();
                options.SaveToken = true;
                options.TokenValidationParameters = _tokenValidationParameters;
            });

            services.ConfigureAspNetIdentity(mySqlConnectionString);

            services.ConfigureIdentityServer4(mySqlConnectionString);
            
            services.AddTransient<IStartupFilter, OnStartupFilter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(policy => {
                policy.AllowAnyOrigin();
                policy.AllowAnyMethod();
                policy.AllowAnyHeader();
                policy.WithHeaders("Origin", "X-Requested-With", "Content-Type", "Accept", "Authorization");
            });
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions(){
                RequestPath = new PathString("")
            });
            app.UseMvcWithDefaultRoute();
            app.UseIdentityServer();
        }
    }
}