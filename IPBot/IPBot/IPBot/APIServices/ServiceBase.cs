using IPBot.Common.Dtos;
using IPBot.Common.Helpers;
using RestSharp;
using RestSharp.Authenticators;

namespace IPBot.APIServices;

public class ServiceBase
{
    private readonly IRestClient _client;
    private readonly string _username;
    private readonly string _password;

    private string _jwt;

    protected ServiceBase(IRestClient client, IConfiguration configuration)
    {
        _client = client;

        var apiLogin = configuration.GetSection("APILogin");
        _username = apiLogin.GetValue<string>("Username");
        _password = apiLogin.GetValue<string>("Password");
    }

    protected async Task<T> GetAsync<T>(string url)
    {
        await ValidateJwtAsync();

        return await _client.GetAsync<T>(new RestRequest(url)
        {
            Authenticator = new JwtAuthenticator(_jwt)
        });
    }

    protected async Task<T> PostAsync<T>(string url, object dto)
    {
        await ValidateJwtAsync();

        return await _client.PostAsync<T>(new RestRequest(url, Method.Post)
        {
            RequestFormat = DataFormat.Json,
            Authenticator = new JwtAuthenticator(_jwt)
        }.AddBody(dto));
    }

    private async Task ValidateJwtAsync()
    {
        if (JwtHelper.CheckTokenIsValid(_jwt)) return;

        _jwt = await _client.PostAsync<string>(new RestRequest("User/Login", Method.Post)
        {
            RequestFormat = DataFormat.Json
        }.AddBody(new UserDto
        {
            Username = _username,
            Password = _password
        }));

    }
}