using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using IPBot.Infrastructure.Helpers;
using IPBot.Models.TenorModels;

namespace IPBot.Helpers;

public static class TenorApiHelper
{
    private const string TenorGifEndpoint = "https://g.tenor.com/v1/gifs?";

    public static async Task<string> GetDirectTenorGifUrlAsync(string tenorUrl)
    {
        var tenorGifId = GetTenorGifIdFromUrl(tenorUrl);
        var apiUrl = CreateApiUrl(new Dictionary<string, string>
        {
            { "key", DotEnvHelper.EnvironmentVariables["TENOR_API_KEY"] },
            { "media_filter", "minimal" },
            { "ids", tenorGifId }
        });

        using var httpClient = new HttpClient();

        var content = new StringContent(apiUrl);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var gifJson = await httpClient.GetStringAsync(apiUrl);

        return ParseTenorApiJson(gifJson);
    }

    private static string GetTenorGifIdFromUrl(string tenorUrl)
    {
        var urlParts = tenorUrl.Split("-");
        return urlParts.Last();
    }

    private static string CreateApiUrl(IDictionary<string, string> parameters)
    {
        var uriBuilder = new UriBuilder(TenorGifEndpoint);
        var paramValues = HttpUtility.ParseQueryString(uriBuilder.Query);

        foreach (var parameter in parameters)
        {
            paramValues.Add(parameter.Key, parameter.Value);
        }

        uriBuilder.Query = paramValues.ToString() ?? string.Empty;

        return uriBuilder.Uri.AbsoluteUri;
    }

    private static string ParseTenorApiJson(string jsonString)
    {
        var jsonRoot = JsonConvert.DeserializeObject<Root>(jsonString);

        return jsonRoot?.Results.FirstOrDefault()?.Media.FirstOrDefault()?.Tinygif.Url;
    }
}