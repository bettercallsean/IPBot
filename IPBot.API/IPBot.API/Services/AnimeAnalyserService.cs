﻿using IPBot.API.Configuration;
using IPBot.Common.Helpers;
using IPBot.Common.Services;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace IPBot.API.Services;

public class AnimeAnalyserService(AzureSettings azureSettings) : IAnimeAnalyserService
{
    private readonly ComputerVisionClient _computerVisionClient = Authenticate(azureSettings);

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

    private static ComputerVisionClient Authenticate(AzureSettings azureSettings)
    {
        return new ComputerVisionClient(new ApiKeyServiceClientCredentials(azureSettings.SubscriptionKey))
        {
            Endpoint = azureSettings.Endpoint
        };
    }
}