using System;
using System.Threading.Tasks;
using Discord.WebSocket;

public class ConsoleCommandHandler
{
    private readonly DiscordSocketClient _client;

    public ConsoleCommandHandler(DiscordSocketClient client)
    {
        _client = client;
    }

    public void StartListening()
    {
        MonitorConsoleCommands();
    }

    private void MonitorConsoleCommands()
    {
        while (true)
        {
            string command = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(command)) continue;

            switch (command.ToLower())
            {
                case "stop":
                    StopBot().Wait(); // Ensure bot stops properly
                    break;
                default:
                    Console.WriteLine("Unknown command. Available commands: stop");
                    break;
            }
        }
    }

    private async Task StopBot()
    {
        Console.WriteLine("Stopping bot...");
        await _client.LogoutAsync();
        await _client.StopAsync();
        Environment.Exit(0);
    }
}
