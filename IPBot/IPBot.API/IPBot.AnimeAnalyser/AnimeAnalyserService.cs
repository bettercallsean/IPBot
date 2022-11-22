using System.Text.Json;
using IPBot.AnimeAnalyser.Helpers;
using IPBot.AnimeAnalyser.Models;
using IPBot.Infrastructure.Interfaces;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace IPBot.AnimeAnalyser;

public class AnimeAnalyserService : IAnimeAnalyser
{
    private readonly ComputerVisionClient _computerVisionClient;

    public AnimeAnalyserService()
    {
        var azureCredentialsPath = Path.Combine(AppContext.BaseDirectory, "AzureConfig/azure_credentials.json");
        var azureCredentials = JsonSerializer.Deserialize<AzureCredentials>(File.ReadAllText(azureCredentialsPath));

        _computerVisionClient = Authenticate(azureCredentials.Endpoint, azureCredentials.SubscriptionKey);
    }

    public async Task<double> GetAnimeScoreAsync(string url)
    {
        TagResult imageTags;

        try
        {
            imageTags = await _computerVisionClient.TagImageAsync(url);
        }
        catch (ComputerVisionErrorResponseException)
        {
            var compressedImage = await ImageCompressorHelper.CompressImageFromUrlAsync(url);

            imageTags = await _computerVisionClient.TagImageInStreamAsync(compressedImage);
        }

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