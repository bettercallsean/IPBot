using Azure;
using Azure.AI.Vision.ImageAnalysis;
using IPBot.API.Configuration;
using IPBot.Common.Helpers;
using IPBot.Common.Services;

namespace IPBot.API.Services;

public class AnimeAnalyserService(AzureSettings azureSettings) : IAnimeAnalyserService
{
    private readonly ImageAnalysisClient _imageAnalysisClient = Authenticate(azureSettings);

    public async Task<double> GetAnimeScoreAsync(string url)
    {
        Response<ImageAnalysisResult> imageTags;

        try
        {
            imageTags  = await _imageAnalysisClient.AnalyzeAsync(new Uri(url), VisualFeatures.Tags);
        }
        catch (RequestFailedException)
        {
            var compressedImage = await ImageCompressorHelper.CompressImageFromUrlAsync(url);

            imageTags  = await _imageAnalysisClient.AnalyzeAsync(new BinaryData(compressedImage), VisualFeatures.Tags);
        }

        return imageTags.Value.Tags.Values.Where(x => x.Name == "anime").Select(x => x.Confidence).FirstOrDefault();
    }

    private static ImageAnalysisClient Authenticate(AzureSettings azureSettings) => new(new Uri(azureSettings.Endpoint), new AzureKeyCredential(azureSettings.SubscriptionKey));
}