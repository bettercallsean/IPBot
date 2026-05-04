using Azure;
using Azure.AI.ContentSafety;
using Azure.AI.Vision.ImageAnalysis;
using IPBot.API.Configuration;
using IPBot.Common.Dtos;
using IPBot.Common.Helpers;
using IPBot.Common.Services;

namespace IPBot.API.Services;

public class ContentAnalyserService : IContentAnalyserService
{
    private readonly ImageAnalysisClient _imageAnalysisClient;
    private readonly ContentSafetyClient _contentSafetyClient;
    private readonly ILogger<ContentAnalyserService> _logger;

    public ContentAnalyserService(ILogger<ContentAnalyserService> logger, AzureSettings azureSettings)
    {
        _logger = logger;
        _imageAnalysisClient = CreateImageAnalysisClient(azureSettings.ImageAnalysisSettings);
        _contentSafetyClient = CreateContentSafetyClient(azureSettings.ContentSafetyAnalysisSettings);
    }

    public async Task<double> GetAnimeScoreAsync(string url)
    {
        const VisualFeatures visualFeatures = VisualFeatures.Tags;
        Response<ImageAnalysisResult> imageTags;

        try
        {
            imageTags = await _imageAnalysisClient.AnalyzeAsync(new Uri(url), visualFeatures);
        }
        catch (RequestFailedException)
        {
            var compressedImage = await ImageCompressorHelper.CompressImageFromUrlAsync(url);

            imageTags = await _imageAnalysisClient.AnalyzeAsync(new BinaryData(compressedImage), visualFeatures);
        }

        return imageTags.Value.Tags.Values.Where(x => x.Name == "anime").Select(x => x.Confidence).FirstOrDefault();
    }

    public async Task<List<CategoryAnalysisDto>> GetImageContentSafetyAnalysisAsync(string imageUrl)
    {
        var image = new ContentSafetyImageData(new Uri(imageUrl));
        var request = new AnalyzeImageOptions(image);

        Response<AnalyzeImageResult> response;
        try
        {
            response = await _contentSafetyClient.AnalyzeImageAsync(request);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError("Analyze image failed. Status code: {Status}, Error code: {ErrorCode}, Error message: {Message}", ex.Status, ex.ErrorCode, ex.Message);
            throw;
        }

        return [.. response.Value.CategoriesAnalysis.Select(x => new CategoryAnalysisDto
        {
            Category = x.Category.ToString(),
            Severity = x.Severity ?? 0
        })];
    }

    public async Task<List<CategoryAnalysisDto>> GetTextContentSafetyAnalysisAsync(string text)
    {
        var request = new AnalyzeTextOptions(text);

        Response<AnalyzeTextResult> response;
        try
        {
            response = await _contentSafetyClient.AnalyzeTextAsync(request);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError("Analyze image failed. Status code: {Status}, Error code: {ErrorCode}, Error message: {Message}", ex.Status, ex.ErrorCode, ex.Message);
            throw;
        }

        return [.. response.Value.CategoriesAnalysis.Select(x => new CategoryAnalysisDto
        {
            Category = x.Category.ToString(),
            Severity = x.Severity ?? 0
        })];
    }

    private static ContentSafetyClient CreateContentSafetyClient(ContentSafetyAnalysisSettings contentSafetyAnalysisSettings)
    {
        return new(new(contentSafetyAnalysisSettings.Endpoint),
            new AzureKeyCredential(contentSafetyAnalysisSettings.SubscriptionKey));
    }

    private static ImageAnalysisClient CreateImageAnalysisClient(ImageAnalysisSettings imageAnalysisSettings)
    {
        return new(new(imageAnalysisSettings.Endpoint),
            new AzureKeyCredential(imageAnalysisSettings.SubscriptionKey));
    }
}