using JobScheduler;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(JobScheduler.Startup))]
namespace JobScheduler
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            var configuration = configurationBuilder.Build();
            builder.Services.UpdateConfiguration(configuration);

            builder.Services.AddHttpContextAccessor();
            var baseAddress = configuration.GetSection("ApiClientBaseAddress").Get<string>();
            builder.Services.AddHttpClient("ApiClient", (client) =>
            {
                client.BaseAddress = new Uri(baseAddress);
            });
        }
    }

    internal static class ServiceExtensions
    {
        public static void UpdateConfiguration(this IServiceCollection serviceCollection, IConfiguration newConfiguration)
        {
            var configuration = serviceCollection.GetConfiguration();
            foreach (var pair in newConfiguration.AsEnumerable())
            {
                configuration[pair.Key] = pair.Value;
            }
            serviceCollection.AddSingleton<IConfiguration>(configuration);
        }

        public static IConfiguration GetConfiguration(this IServiceCollection serviceCollection)
        {
            var sp = serviceCollection.BuildServiceProvider();
            var configuration = sp.GetRequiredService<IConfiguration>();
            return configuration;
        }
    }
}
