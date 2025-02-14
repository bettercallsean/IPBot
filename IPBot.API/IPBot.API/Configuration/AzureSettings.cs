namespace IPBot.API.Configuration;

public class AzureSettings
{
    public ImageAnalysisSettings ImageAnalysisSettings { get; init; }
    public ContentSafetyAnalysisSettings ContentSafetyAnalysisSettings { get; init; }
}

public class ImageAnalysisSettings
{
    public string SubscriptionKey { get; init; }
    public string Endpoint { get; init; }
}

public class ContentSafetyAnalysisSettings
{
    public string SubscriptionKey { get; init; }
    public string Endpoint { get; init; }
}