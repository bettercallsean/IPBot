namespace IPBot.API.Configuration;

internal class AzureSettings
{
    public ImageAnalysisSettings ImageAnalysisSettings { get; init; }
    public ContentSafetyAnalysisSettings ContentSafetyAnalysisSettings { get; init; }
}

internal class ImageAnalysisSettings
{
    public string SubscriptionKey { get; init; }
    public string Endpoint { get; init; }
}

internal class ContentSafetyAnalysisSettings
{
    public string SubscriptionKey { get; init; }
    public string Endpoint { get; init; }
}