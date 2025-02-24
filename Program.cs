using Discord;
using Discord.WebSocket;
using OpenAI;
using dotenv.net;
using dotenv.net.Utilities;

class Program
{
    // Declaration of Discord and OpenAI client instances
    private static DiscordSocketClient _client;
    private static OpenAIClient _openAi;

    static async Task Main(string[] args)
    {
        // Load environment variables from .env file
        DotEnv.Load();

        // Retrieve API keys from environment variables
        string discordToken = EnvReader.GetStringValue("DISCORD_TOKEN");
        string openAiKey = EnvReader.GetStringValue("OPENAI_KEY");

        // Initialize the Discord bot with specific intents
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
        });

        // Initializing OpenAI API client
        _openAi = new OpenAIClient(openAiKey);

        // Hook up event handlers
        _client.Log += LogAsync;
        _client.MessageReceived += MessageReceivedAsync;
    
        // Login to Discord and start the bot
        await _client.LoginAsync(TokenType.Bot, discordToken);
        await _client.StartAsync();

        Console.WriteLine("MBP Bot is online.");

        // Start listening for console commands (running on a separate thread)
        Task.Run(() => MonitorConsoleCommands());

        // Keep the bot running indefinitely
        await Task.Delay(-1);
    }

    /// <summary>
    /// Handles message logging from the Discord client.
    /// </summary>
    private static Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles messages received in Discord.
    /// Listens for the "!ask" command and responds accordingly.
    /// </summary>
    private static async Task MessageReceivedAsync(SocketMessage message) 
    {
        // Ignore messages from other bots or direct messages
        if(message.Author.IsBot || message.Channel is IDMChannel) return;

        // Check if the message has prefix !ask
        if(message.Content.StartsWith("!ask"))
        {
            await message.Channel.SendMessageAsync("Let me think a bit on that one...");
            return;
        }
    }

    /// <summary>
    /// Monitors console input and processes commands while the bot is running.
    /// Currently supports the "stop" command to shut down the bot.
    /// </summary>
    private static void MonitorConsoleCommands()
    {
        while (true)
        {
            string command = Console.ReadLine();

            if(string.IsNullOrWhiteSpace(command)) continue;

            switch (command.ToLower())
            {
                case "stop":
                    StopBot();
                    break;
            }

        }
    }

    /// <summary>
    /// Gracefully logs out the bot and shuts down the application.
    /// </summary>
    /// <returns></returns>
    private static async Task StopBot()
    {
        Console.WriteLine("Stopping bot...");
        await _client.LogoutAsync();
        await _client.StopAsync();
        Environment.Exit(0);
    }
}