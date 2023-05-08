using IPBot.Infrastructure.Helpers;
using IPBot.Shared.Services;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace IPBot.API.Services;

public class AnimeAnalyserService : IAnimeAnalyserService
{
    private readonly ComputerVisionClient _computerVisionClient;

    public AnimeAnalyserService(IConfiguration configuration)
    {
        var endpoint = configuration.GetSection("AzureSettings")["Endpoint"];
        var subscriptionKey = configuration.GetSection("AzureSettings")["SubscriptionKey"];

        _computerVisionClient = Authenticate(endpoint, subscriptionKey);
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