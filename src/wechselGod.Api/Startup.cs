using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using wechselGod.Api.Extensions;
using wechselGod.Api.Services;
using wechselGod.Infrastructure;

namespace wechselGod.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            services.ConfigureSwagger();
            ConfigureCommonServices(services);
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            ConfigureCommonServices(services);
        }

        public void ConfigureCommonServices(IServiceCollection services)
        {
            services.ConfigureApi();
            services.ConfigureSettings(Configuration);
            services.ConfigureDatabase(Configuration);
            services.ConfigureAuthentication(Configuration);
            services.AddHttpContextAccessor();
            services.AddEasyCaching(options => { options.UseInMemory("users"); });
            services.AddTransient<FinApiService>();
            services.AddTransient<CachingService>();
            services.ConfigureHttpClient();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            var service = provider.GetService<DatabaseContext>();
            service?.Database.Migrate();

            app.UseCors("origin");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
