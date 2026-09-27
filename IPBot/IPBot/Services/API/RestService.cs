using RestSharp;

namespace IPBot.Services.API;

internal class RestService(IRestClient client)
{
    private readonly IRestClient _client = client;

    protected async Task<T> GetAsync<T>(string url)
    {
        var result = await _client.ExecuteAsync<T>(new RestRequest(url));

        return await ParseResultAsync(result);
    }

    protected async Task<T> PostAsync<T>(string url, object dto)
    {
        var result = await _client.ExecutePostAsync<T>(new RestRequest(url, Method.Post)
        {
            RequestFormat = DataFormat.Json
        }.AddBody(dto));

        return await ParseResultAsync(result);
    }

    protected async Task<T> PatchAsync<T>(string url, object dto)
    {
        var result = await _client.ExecutePatchAsync<T>(new RestRequest(url, Method.Patch)
        {
            RequestFormat = DataFormat.Json
        }.AddBody(dto));

        return await ParseResultAsync(result);
    }

    protected async Task<T> PatchAsync<T>(string url)
    {
        var result = await _client.ExecutePatchAsync<T>(new RestRequest(url, Method.Patch)
        {
            RequestFormat = DataFormat.Json
        });

        return await ParseResultAsync(result);
    }

    protected async Task<T> PutAsync<T>(string url, object dto)
    {
        var result = await _client.ExecutePutAsync<T>(new RestRequest(url, Method.Put)
        {
            RequestFormat = DataFormat.Json
        }.AddBody(dto));

        return await ParseResultAsync(result);
    }

    protected async Task<T> DeleteAsync<T>(string url)
    {
        var result = await _client.ExecuteDeleteAsync<T>(new RestRequest(url, Method.Delete)
        {
            RequestFormat = DataFormat.Json
        });

        return await ParseResultAsync(result);
    }

    private static async Task<T> ParseResultAsync<T>(RestResponse<T> response)
    {
        if (!response.IsSuccessful)
        {
            throw new Exception($"Request failed with status code {response.StatusCode}: {response.Content}");
        }

        return response.Data;
    }
}