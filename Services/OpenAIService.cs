using OpenAI;
using OpenAI.Chat;

public class OpenAIService
{
    private readonly ChatClient _openAiClient;

    public OpenAIService(string apiKey)
    {
        _openAiClient = new ChatClient("gpt-4o", apiKey);
    }

    public async Task<string> AnalyzePlayerStatsAsync(string stats)
    {
        ChatCompletion completion = await _openAiClient.CompleteChatAsync(
            [
            new SystemChatMessage("You are a Faceit stats analyst."),
            new UserChatMessage($"Analyze this player stats:\n{stats}")
            ]);
            return completion.Content[0].Text;
    }
}