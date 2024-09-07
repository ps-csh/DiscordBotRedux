
using DiscordBot;
using DiscordBot.Application;
using DiscordBot.Bot;
using DiscordBot.Bot.Commands;
using DiscordBot.Bot.Commands.Modules;
using DiscordBot.Bot.Commands.Validators;
using DiscordBot.Bot.ServerData;
using DiscordBot.Controller;
using DiscordBot.Database;
using DiscordBot.DiscordAPI;
using DiscordBot.DiscordAPI.Structures.Voice;
using DiscordBot.Logging;
using DiscordBot.src.Controller;
using DiscordBot.Startup;
using DiscordBot.Startup.Configuration;
using DiscordBot.Startup.Reflection;
using DiscordBot.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace DiscordBot
{
    class Program
    {
        private const string DEFAULT_CONNECTION_STRING = "default";

        private static IServiceProvider? _serviceProvider;
        private static ILogger? _logger;
        private static ConsoleBotController? _botController;
        private static ConsoleIOHandler? _consoleIOHandler;
        private static BotManager? _botManager;

        public static bool IsRunning { get; private set; }

        public static int Main()
        {
            Initialize();

           // var s = _serviceProvider.GetRequiredService<DefaultCommandModule>();

            if (SetRequiredServices())
            {
                Run();
            }

            //var gateway = _serviceProvider!.GetRequiredService<DiscordGatewayClient>();
            //var requestHandler = _serviceProvider!.GetRequiredService<DiscordHttpRequestHandler>();
            //var apiClient = _serviceProvider!.GetRequiredService<DiscordAPIClient>();
            //var repo = _serviceProvider!.GetRequiredService<DiscordDataRepository>();


            Cleanup();

            return 0;

            //try
            //{
            //    Console.WriteLine("Sending test message");
            //    _logger?.LogInfo("Waiting for response...");
            //    //var response =  requestHandler.PostRequest("channels/237887183535472640/messages", "Test", "application/json").Result;
            //    //   [ new KeyValuePair<string, string>("Content-Type", "application/json") ]).Result;
            //    var response = apiClient.PostMessage("Test", "237887183535472640").Result;
            //    _logger?.LogInfo(response?.StatusCode.ToString() ?? "No Code");
            //    _logger?.LogInfo(response?.Content?.ToString() ?? "Empty");
            //}
            //catch (Exception ex)
            //{
            //    _logger?.LogException(ex);
            //}

        }

        public static void Initialize()
        {
            //HostApplicationBuilder builder = Host.CreateApplicationBuilder();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            IConfiguration configuration = CreateConfiguration();

            ServiceCollection services = new();
            ConfigureServices(services, configuration);
            _serviceProvider = services.BuildServiceProvider();

            
        }

        private static IConfiguration CreateConfiguration()
        {
            ConfigurationBuilder builder = new();
            builder.AddUserSecrets<Program>();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");

            return builder.Build();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //TODO: Add necessary configurations 

            services.Configure<BotSettingsConfiguration>(configuration.GetSection("Bot"))
                .Configure<DiscordGatewayConfiguration>(configuration.GetSection("Gateway"))
                .Configure<BotSecretsConfiguration>(configuration.GetSection("Authentication"))
                .Configure<DiscordApiEndpointsConfiguration>(configuration.GetSection("ApiEndpoints"))
                .Configure<ConnectionStringsConfiguration>(configuration.GetSection("ConnectionStrings"))
                .Configure<LoggerConfiguration>(configuration.GetSection("Logger"));

            //services.AddLogging(builder => builder.AddConsole());

            services.AddDbContext<BotDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString(DEFAULT_CONNECTION_STRING)));

            //Using custom Logger to make it easier to write to file
            services.AddSingleton<ILogger, Logger>()
                .AddSingleton<ApplicationLifetimeService>()
                .AddSingleton<DiscordGatewayClient>()
                .AddSingleton<DiscordAPIClient>() //TODO: Change to IHttpHandler factory
                .AddSingleton<DiscordVoiceGatewayClient>()
                .AddSingleton<DiscordDataRepository>()
                .AddSingleton<DiscordHttpRequestHandler>()
                .AddSingleton<BotManager>()
                .AddScoped<RoleValidator>()
                .AddSingleton<CommandParserService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<DefaultCommandModule>()
                .AddSingleton<DbAccessCommandModule>()
                .AddSingleton<ConsoleIOHandler>()
                .AddScoped<ConsoleBotController>();            
        }

        private static bool SetRequiredServices()
        {
            if (_serviceProvider != null)
            {
                try
                {
                    _logger = _serviceProvider.GetRequiredService<ILogger>();
                    _serviceProvider.GetRequiredService<ApplicationLifetimeService>().RegisterShutDownCallback(Shutdown);
                    _botManager = _serviceProvider.GetRequiredService<BotManager>();
                    _consoleIOHandler = _serviceProvider.GetRequiredService<ConsoleIOHandler>();
                    _botController = _serviceProvider.GetRequiredService<ConsoleBotController>();
                    LoadCommandModules();
                    //_ = _serviceProvider.GetRequiredService<DatabaseContext>();

                    return true;
                }
                catch (Exception ex)
                {
                    _logger?.LogException(ex, "Exception caught when loading required services");
                }
            }
            _logger?.LogWarning("Failed to initialize required services");
            return false;
        }

        private static void LoadCommandModules()
        {
            if (_serviceProvider != null)
            {
                // Ensure modules are initialized so they load commands into CommandHandler
                //TODO: Load modules using reflection instead
                _ = _serviceProvider.GetRequiredService<DefaultCommandModule>();
                _ = _serviceProvider.GetRequiredService<DbAccessCommandModule>();
            }
        }

        private static void Run()
        {
            IsRunning = true;

            _consoleIOHandler?.PrintMessage("Application running and awaiting input. Type shutdown to end.");

            _consoleIOHandler?.PrintMessage($"{_serviceProvider?.GetService<CommandHandler>()?.Commands.Keys.Count() ?? 0} commands loaded");

            //var test = new DiscordVoiceGatewayEvent(DiscordVoiceGatewayOpcode.Identify,
            //    new DiscordVoiceIdentifyPayload("server", "user", "session", "token"));
            //_consoleIOHandler?.PrintMessage($"{test.ToJson()}");

            while (IsRunning)
            {
                //_botManager?.
                _botController?.PollCommands();
            }
        }

        //TODO: Should Shutdown be called directly from controller classes
        public static void Shutdown()
        {
            IsRunning = false;
            _botManager?.StopBot();
        }

        private static void Cleanup()
        {
            //TEMP:
            _logger?.WriteLogsToFile("C:\\Users\\oncom\\source\\repos\\DiscordBotRedux\\DiscordBot\\bin\\Debug\\net8.0\\Logs\\Debug.txt");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                _logger?.LogException(ex, "Program encountered an unhandled exception");
            }
            Shutdown();
        }
    }
}