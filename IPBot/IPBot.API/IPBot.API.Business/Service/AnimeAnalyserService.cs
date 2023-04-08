using IPBot.API.Shared.Services;
using IPBot.Infrastructure.Helpers;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;

namespace IPBot.API.Business.Service;

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

        return Enumerable.FirstOrDefault<double>(imageTags.Tags.Where(x => x.Name == "anime").Select(x => x.Confidence));
    }

    private ComputerVisionClient Authenticate(string endpoint, string key)
    {
        return new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
        {
            Endpoint = endpoint
        };
    }
}