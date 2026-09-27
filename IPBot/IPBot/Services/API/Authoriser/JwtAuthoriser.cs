using IPBot.Common.Dtos;
using IPBot.Common.Helpers;
using IPBot.Configuration;
using RestSharp;
using RestSharp.Authenticators;

namespace IPBot.Services.API.Authoriser;

public class JwtAuthoriser(APILogin apiLogin) : IAuthenticator
{
    private const string LoginUri = "User/Login";

    private readonly APILogin _apiLogin = apiLogin;
    private string _jwtToken;

    public async ValueTask Authenticate(IRestClient client, RestRequest request)
    {
        if (request.Resource == LoginUri) return;

        if (!JwtHelper.CheckTokenIsValid(_jwtToken))
        {
            var response = await client.ExecutePostAsync<string>(new RestRequest(LoginUri, Method.Post)
            {
                RequestFormat = DataFormat.Json
            }.AddBody(new UserDto
            {
                Username = _apiLogin.Username,
                Password = _apiLogin.Password
            }));

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to authenticate with API. Status code: {response.StatusCode}, Content: {response.Content}");

            _jwtToken = response.Data;
        }

        request.AddHeader("Authorization", $"Bearer {_jwtToken}");

        return;
    }
}
