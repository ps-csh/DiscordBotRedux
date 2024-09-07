using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.DiscordAPI;
using DiscordBot.Startup.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Startup
{
    internal class AppStartup
    {
        public static void Initialize()
        {
            //HostApplicationBuilder builder = Host.CreateApplicationBuilder();

            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            IConfiguration configuration = CreateConfiguration();

            ServiceCollection services = new ServiceCollection();

            
        }

        private static IConfiguration CreateConfiguration()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddUserSecrets<AppStartup>();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");

            return builder.Build();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //TODO: Add necessary configurations 

            //var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            //var logger = loggerFactory.CreateLogger<Program>();

            services.Configure<BotSettingsConfiguration>(configuration.GetSection("Bot"));
            services.Configure<DiscordGatewayConfiguration>(configuration.GetSection("Gateway"));
            services.Configure<BotSecretsConfiguration>(configuration.GetSection("Authentication"));

            services.AddLogging(builder => builder.AddConsole());

            services.AddSingleton<DiscordGatewayClient>();
        }
    }
}
