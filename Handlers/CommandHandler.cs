using Discord;
using Discord.WebSocket;
public class CommandHandler
{
    private readonly FaceitService _faceitService;
    private readonly OpenAIService _openAiService;

    public CommandHandler(FaceitService faceitService, OpenAIService openAiService)
    {
        _faceitService = faceitService;
        _openAiService = openAiService;
    }

    public async Task HandleMessage(SocketMessage message)
    {
        if (message.Author.IsBot) return;

        if (message.Content.StartsWith("!stats"))
        {
            string playerNickname = message.Content.Substring(6).Trim();
            if (string.IsNullOrWhiteSpace(playerNickname))
            {
                await message.Channel.SendMessageAsync("Please provide a Faceit nickname.");
                return;
            }

            var reply = await message.Channel.SendMessageAsync("Fetching match history...");

            _ = Task.Run(async () =>
            {
                try
                {
                    var matches = await _faceitService.GetLast10MatchesAsync(playerNickname);
                    if (matches == null || matches.Count == 0)
                    {
                        await reply.ModifyAsync(m => m.Content = "No match history found.");
                        return;
                    }

                    string matchSummary = string.Join("\n", matches.Select(m => $"{m.Result}"));

                    string analysis = await _openAiService.AnalyzePlayerStatsAsync(
                        $"Analyze the following match history and give insights:\n{matchSummary}");

                    await reply.ModifyAsync(m => m.Content = analysis);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing !stats: {ex}");
                    await reply.ModifyAsync(m => m.Content = "An error occurred while fetching stats.");
                }
            });

        }
    }
}
