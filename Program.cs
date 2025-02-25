using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using dotenv.net;
using dotenv.net.Utilities;

class Program
{
    static async Task Main(string[] args)
    {
        // Load environment variables from .env file
        DotEnv.Load();

        // Dependency Injection
        var services = ConfigureServices();

        // Get Bot instance
        var bot = services.GetRequiredService<Bot>();
        var consoleHandler = services.GetRequiredService<ConsoleCommandHandler>();

        Task.Run(() => consoleHandler.StartListening());

        // Start bot
        await bot.RunAsync();
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        
        string faceitApiKey = EnvReader.GetStringValue("FACEIT_KEY");
        string openAiKey = EnvReader.GetStringValue("OPENAI_KEY");

        // Register services
        services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
        }));
        services.AddSingleton<FaceitService>(provider => new FaceitService(faceitApiKey));
        services.AddSingleton<OpenAIService>(provider => new OpenAIService(openAiKey));
        services.AddSingleton<ConsoleCommandHandler>();
        services.AddSingleton<CommandHandler>();
        services.AddSingleton<Bot>();

        return services.BuildServiceProvider();
    }
}