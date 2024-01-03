using Common.AppConfiguration;
using Common.Interfaces;
using Common.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using StackExchange.Redis;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder.Extensions;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Features;

namespace Common
{
    public static class CommonServiceExtensions
    {
        public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddScoped<IJwtUtils, JwtUtils>();

            services.Configure<FirebaseSettings>(configuration.GetSection("FirebaseSettings"));
            var firebaseCreds = JsonConvert.SerializeObject(configuration.GetSection("FirebaseSettings").Get<FirebaseSettings>());
            
            var firebaseApp = FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(firebaseCreds),
            });

            if (firebaseApp.Name == null)
            {
                throw new Exception("Firebase is not initialized.");
            }

            services.Configure<RedisSettings>(configuration.GetSection("RedisSettings"));
            RedisSettings redisSettings = configuration.GetSection("RedisSettings").Get<RedisSettings>();
            var redisConfiguration = new StackExchange.Redis.ConfigurationOptions
            {
                EndPoints =
                    {
                        { redisSettings.BaseAddress, redisSettings.Port }
                    },
                Password = redisSettings.Password,
                SyncTimeout = 30000,
            };
            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = redisConfiguration;
            });
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                try
                {
                    return ConnectionMultiplexer.Connect(redisConfiguration);
                }
                catch
                {
                    // Do nothing
                    return null!;
                }
            });
            services.AddSingleton<IRedisCacheService>(sp =>
            {
                var multiplexer = sp.GetService<IConnectionMultiplexer>();
                if (multiplexer == null || !multiplexer.IsConnected)
                {
                    return new EmptyRedisCacheService();
                }

                var distributedCache = sp.GetRequiredService<IDistributedCache>();
                var redisOptionSettings = sp.GetRequiredService<IOptions<RedisSettings>>();

                return new RedisCacheService(distributedCache, redisOptionSettings);
            });

            services.Configure<AgoraSettings>(configuration.GetSection("AgoraSettings"));
            services.AddScoped<IAgoraService, AgoraService>();

            services.Configure<LocationApiSettings>(configuration.GetSection("LocationApiSettings"));
            services.AddScoped<ILocationService, LocationService>();

            services.Configure<FirebaseStorageSettings>(configuration.GetSection("FirebaseStorageSettings"));
            services.AddScoped<IFirebaseStorageService, FirebaseStorageService>();

            services.Configure<PaypalSettings>(configuration.GetSection("PaypalSettings"));
            services.AddScoped<IPaypalService, PaypalService>();

            services.Configure<RabbitmqSettings>(configuration.GetSection("RabbitMqSettings"));
            services.AddScoped<IRabbitmqService, RabbitmqService>();

            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddScoped<IMailService, MailService>();

            services.AddScoped<IAzureStorageService, AzureStorageService>();

            return services;
        }
    }
}
