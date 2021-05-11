using System;
using System.Text;
using FluentOptionsValidation;
using FluentValidation.AspNetCore;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using wechselGod.Api.Features.Banks;
using wechselGod.Api.Services;
using wechselGod.Api.Settings;
using wechselGod.Infrastructure;

namespace wechselGod.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureFluentOptions<FinApiClientSettings>(configuration, x => x.AbortStartupOnError = true);
            services.ConfigureFluentOptions<JwtSettings>(configuration, x => x.AbortStartupOnError = true);
        }

        public static void ConfigureApi(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("origin",
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:3000/").AllowAnyHeader().AllowAnyMethod()
                            .SetIsOriginAllowed(_ => true);
                    });
            });
            services.AddControllers().AddFluentValidation(
                configuration =>
                    configuration.RegisterValidatorsFromAssemblyContaining(typeof(GetBankDetailsRequest)));
        }

        public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DatabaseContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }

        public static void ConfigureHttpClient(this IServiceCollection services)
        {
            services.AddSingleton<IFlurlClientFactory, PerBaseUrlFlurlClientFactory>();
            FlurlHttp.Configure(settings =>
            {
                var jsonSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                };
                settings.JsonSerializer = new NewtonsoftJsonSerializer(jsonSettings);
            });
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "wechselGod API", Version = "v1"});
                c.EnableAnnotations();
            });
        }

        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<TokenService>();

            var jwtAppSettingOptions = configuration.GetSection(nameof(JwtSettings));

            var signingKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAppSettingOptions[nameof(JwtSettings.SecretKey)]));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(config =>
            {
                config.RequireHttpsMetadata = false;
                config.SaveToken = true;
                config.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = signingKey,
                    ValidateAudience = true,
                    ValidAudience = jwtAppSettingOptions[nameof(JwtSettings.Audience)],
                    ValidateIssuer = true,
                    ValidIssuer = jwtAppSettingOptions[nameof(JwtSettings.Issuer)],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAuthorization(_ => { });
        }
    }
}
