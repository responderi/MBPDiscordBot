using System.Text.Json;
using System.Text.Json.Serialization;


public class FaceitService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string BaseUrl = "https://open.faceit.com/data/v4/";

    public FaceitService(string apiKey)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        _apiKey = apiKey;
    }

    public async Task<List<FaceitMatch>> GetLast10MatchesAsync(string playerNickname)
    {
        try
        {
            // Get the Player ID first
            string playerId = await GetPlayerIdAsync(playerNickname);

            if (string.IsNullOrEmpty(playerId)) return null;

            // Retrieve match history
            string url = $"{BaseUrl}players/{playerId}/games/csgo/stats?limit=10";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            var matchData = JsonSerializer.Deserialize<FaceitMatchResponse>(responseBody);

            return matchData?.Items.Select(m => m.Stats).ToList() ?? new List<FaceitMatch>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving matches: {ex.Message}");
            return null;
        }
    }

    private async Task<string> GetPlayerIdAsync(string playerNickname)
    {
        try
        {
            string url = $"{BaseUrl}players?nickname={playerNickname}";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
                        
            var playerData = JsonSerializer.Deserialize<FaceitPlayerResponse>(responseBody);

            return playerData?.PlayerId ?? string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving player ID: {ex.Message}");
            return string.Empty;
        }
    }
}

// Faceit API response models
public class FaceitPlayerResponse
{
    [JsonPropertyName("player_id")]
    public string PlayerId { get; set; }
}
public class FaceitMatchResponse
{
    [JsonPropertyName("items")]
    public List<FaceitMatchWrapper> Items { get; set; }
}

// Wrapper to extract "stats" from the JSON response
public class FaceitMatchWrapper
{
    [JsonPropertyName("stats")]
    public FaceitMatch Stats { get; set; }
}

public class FaceitMatch
{
    [JsonPropertyName("Match Id")]
    public string MatchId { get; set; }

    [JsonPropertyName("Result")]
    public string Result { get; set; }

    [JsonPropertyName("Map")]
    public string Map { get; set; }

    [JsonPropertyName("Score")]
    public string Score { get; set; }

    [JsonPropertyName("Winner")]
    public string Winner { get; set; }

    [JsonPropertyName("Team")]
    public string Team { get; set; }
}
