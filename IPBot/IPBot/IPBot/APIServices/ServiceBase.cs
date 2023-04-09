using RestSharp;

namespace IPBot.APIServices;

public class ServiceBase
{
    private readonly IRestClient _client;

    protected ServiceBase(IRestClient client)
    {
        _client = client;
    }
    
    protected async  Task<T> GetAsync<T>(string url)
    {
        return await _client.GetAsync<T>(new RestRequest(url));
    }
    
    protected async  Task<T> PostAsync<T>(string url, object dto)
    {
        return await _client.PostAsync<T>(new RestRequest(url, Method.Post)
        {
            RequestFormat = DataFormat.Json
        }.AddBody(dto));
    }
}