using RestSharp;

namespace IPBot.Services.API;

internal class RestService(IRestClient client)
{
    private readonly IRestClient _client = client;

    protected async Task<T> GetAsync<T>(string url)
    {
        var response = await _client.ExecuteAsync<T>(new RestRequest(url));

        return await ParseResponseAsync(response);
    }

    protected async Task<T> PostAsync<T>(string url, object dto)
    {
        var response = await _client.ExecutePostAsync<T>(new RestRequest(url, Method.Post).AddJsonBody(dto));

        return await ParseResponseAsync(response);
    }

    protected async Task<T> PatchAsync<T>(string url, object dto)
    {
        var response = await _client.ExecutePatchAsync<T>(new RestRequest(url, Method.Patch).AddJsonBody(dto));

        return await ParseResponseAsync(response);
    }

    protected async Task<T> PutAsync<T>(string url, object dto)
    {
        var response = await _client.ExecutePutAsync<T>(new RestRequest(url, Method.Put).AddJsonBody(dto));

        return await ParseResponseAsync(response);
    }

    protected async Task<T> DeleteAsync<T>(string url)
    {
        var response = await _client.ExecuteDeleteAsync<T>(new RestRequest(url, Method.Delete));

        return await ParseResponseAsync(response);
    }

    private static async Task<T> ParseResponseAsync<T>(RestResponse<T> response)
    {
        if (!response.IsSuccessful)
        {
            throw new Exception($"Request failed with status code {response.StatusCode}: {response.Content}");
        }

        return response.Data;
    }
}