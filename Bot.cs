using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using dotenv.net;
using dotenv.net.Utilities;

public class Bot
{
    private readonly DiscordSocketClient _client;
    private readonly CommandHandler _commandHandler;

    public Bot(DiscordSocketClient client, CommandHandler commandHandler)
    {
        _client = client;
        _commandHandler = commandHandler;

        _client.Log += LogAsync;
        _client.MessageReceived += _commandHandler.HandleMessage;
    }

    public async Task RunAsync()
    {
        string discordToken = EnvReader.GetStringValue("DISCORD_TOKEN");

        await _client.LoginAsync(TokenType.Bot, discordToken);
        await _client.StartAsync();

        Console.WriteLine("🤖 Bot is online.");
        await Task.Delay(-1); // Keeps the bot running indefinitely
    }

    private Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log);
        return Task.CompletedTask;
    }
}
