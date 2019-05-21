using DataTransferObjects.Configurations;
using Enums;
using Hangfire;
using Hangfire.PostgreSql;
using ImpulseAPI.Extensions;
using Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Providers;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImpulseAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.ConfigureExceptionHandler();

            app.UseMvc();
            app.UseStaticFiles();

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Impulse");
            });

            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new MyAuthorizationFilter() }
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Default page.");
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "localhost",
                        ValidAudience = "localhost",
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes("SuperKeyftyfthfhfghffhgfhgfhgfhgfghf"))
                    };
                });

            services.AddMvc();

            string hostname = Environment.GetEnvironmentVariable("POSTGRESQL_HOST");
            string database = Environment.GetEnvironmentVariable("POSTGRESQL_DATABASE");
            string userId = Environment.GetEnvironmentVariable("POSTGRESQL_USER_ID");
            string password = Environment.GetEnvironmentVariable("POSTGRESQL_PASSWORD");
            services.AddHangfire(x => x.UsePostgreSqlStorage("Host=" + hostname + ";Database=" + database +
                ";User ID=" + userId + ";Password=" + password + ";Port=5432;"));

            services.Configure<TelegramConfigurationsDto>(Configuration.GetSection("services:telegram"));
            services.Configure<SlackConfigurationsDto>(Configuration.GetSection("services:slack"));
            services.Configure<EmailConfigurationsDto>(Configuration.GetSection("services:email"));

            // services
            services.AddSingleton(typeof(TelegramProvider));
            services.AddSingleton(typeof(SlackProvider));
            services.AddSingleton(typeof(EmailProvider));

            // provider selector
            services.AddTransient<Func<Provider, ISocialProvider>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case Provider.Telegram:
                        return serviceProvider.GetService<TelegramProvider>();
                    case Provider.Slack:
                        return serviceProvider.GetService<SlackProvider>();
                    case Provider.Email:
                        return serviceProvider.GetService<EmailProvider>();
                    default:
                        throw new KeyNotFoundException();
                }
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Impulse", Version = "v1",
                    Description = "Program for sending notifications through different communication channels (such as Telegram, Slack and similar)." });
                c.IncludeXmlComments(AppDomain.CurrentDomain.BaseDirectory + "ImpulseAPI.xml");
            });
        }
    }
}
