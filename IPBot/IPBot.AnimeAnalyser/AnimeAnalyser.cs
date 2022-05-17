using IPBot.AnimeAnalyser.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Newtonsoft.Json;

namespace IPBot.AnimeAnalyser;

public class AnimeAnalyser
{
    private readonly ComputerVisionClient _computerVisionClient;
    
    public AnimeAnalyser()
    {
        var azureCredentialsPath = Path.Combine(AppContext.BaseDirectory, "AzureConfig/azure_credentials.json");
        var azureCredentials = JsonConvert.DeserializeObject<AzureCredentials>(File.ReadAllText(azureCredentialsPath));
        
        _computerVisionClient = Authenticate(azureCredentials.Endpoint, azureCredentials.SubscriptionKey);
    }

    public async Task<double> GetAnimeScoreAsync(string url)
    {
        var imageTags = await _computerVisionClient.TagImageAsync(url);

        return imageTags.Tags.Where(x => x.Name == "anime").Select(x => x.Confidence).FirstOrDefault();
    }
    
    private ComputerVisionClient Authenticate(string endpoint, string key)
    {
        return new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
        {
            Endpoint = endpoint
        };
    }
}