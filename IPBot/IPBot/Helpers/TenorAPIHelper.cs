using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using IPBot.Models.TenorModels;

namespace IPBot.Helpers;

public class TenorApiHelper
{
    private const string TenorGifEndpoint = "https://g.tenor.com/v1/gifs?";
    private readonly IConfigurationRoot _config;
    
    public TenorApiHelper(IConfigurationRoot config)
    {
        _config = config;
    }
    
    public async Task<string> GetDirectTenorGifUrlAsync(string tenorUrl)
    {
        var tenorGifId = GetTenorGifIdFromUrl(tenorUrl);
        var apiUrl = CreateAPIUrl(new Dictionary<string, string>
        {
            {"key", _config["tenorAPIKey"]},
            {"media_filter", "minimal"},
            {"ids", tenorGifId}
        });
        using var httpClient = new HttpClient();
        
        var content = new StringContent(apiUrl);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        
        var response = await httpClient.GetAsync(apiUrl);
        var contentString = await response.Content.ReadAsStringAsync();

        return ParseTenorAPIJson(contentString);
    }

    private string GetTenorGifIdFromUrl(string tenorUrl)
    {
        var urlParts = tenorUrl.Split("-");
        return urlParts.Last();
    }

    private string CreateAPIUrl(IDictionary<string, string> parameters)
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

    private string ParseTenorAPIJson(string jsonString)
    {
        var jsonRoot = JsonConvert.DeserializeObject<Root>(jsonString);

        return jsonRoot?.Results.FirstOrDefault()?.Media.FirstOrDefault()?.Tinygif.Url;
    }
}