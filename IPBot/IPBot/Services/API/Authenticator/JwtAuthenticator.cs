using IPBot.Common.Dtos;
using IPBot.Common.Helpers;
using IPBot.Configuration;
using RestSharp;
using RestSharp.Authenticators;

namespace IPBot.Services.API.Authenticator;

public sealed class JwtAuthenticator(BotConfiguration botConfiguration) : IAuthenticator
{
    private const string LoginUri = "User/Login";

    private readonly APILogin _apiLogin = botConfiguration.APILogin;
    private readonly RestClient _loginClient = new(botConfiguration.APIEndpoint);

    private string _jwtToken;

    public async ValueTask Authenticate(IRestClient client, RestRequest request)
    {
        if (!JwtHelper.CheckTokenIsValid(_jwtToken))
        {
            var loginRequest = new RestRequest(LoginUri, Method.Post)
                .AddJsonBody(new UserDto
                {
                    Username = _apiLogin.Username,
                    Password = _apiLogin.Password
                });

            var response = await _loginClient.ExecuteAsync<string>(loginRequest);

            if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Data))
            {
                throw new InvalidOperationException($"Failed to authenticate with API. Status code: {response.StatusCode}, Content: {response.Content}");
            }

            _jwtToken = response.Data;
        }

        request.AddOrUpdateHeader("Authorization", $"Bearer {_jwtToken}");
    }
}