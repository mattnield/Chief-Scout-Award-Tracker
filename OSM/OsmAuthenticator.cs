using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using OSM.Configuration;
using RestSharp;
using RestSharp.Authenticators;

namespace OSM;

record TokenResponse
{
    [JsonPropertyName("token_type")] public string TokenType { get; init; } = string.Empty;
    [JsonPropertyName("access_token")] public string AccessToken { get; init; } = string.Empty;
    [JsonPropertyName("expires_in")] public int ExpiresIn { get; init; }
}

public class OsmAuthenticator : AuthenticatorBase
{
    private readonly OsmOptions _options;
    private DateTime _tokenExpiration = DateTime.Now.AddMilliseconds(-10);

    public OsmAuthenticator(OsmOptions options) : base("")
    {
        _options = options;
    }
    private bool GetTokenHasExpired() => _tokenExpiration <= DateTime.Now;

    private bool GetTokenIsEmpty() => string.IsNullOrEmpty(Token);
    protected override async ValueTask<Parameter> GetAuthenticationParameter(string accessToken)
    {
        if (GetTokenHasExpired() || GetTokenIsEmpty())
        {
            Token = await GetToken();
        }

        return new HeaderParameter(KnownHeaders.Authorization, Token);
    }
    
    async Task<string> GetToken()
    {
        var client = new RestClient(_options.ClientEndpoint);

        var request = new RestRequest("/oauth/token", Method.Get)
            .AddQueryParameter("client_id", _options.ClientId)
            .AddQueryParameter("client_secret", _options.ClientSecret)
            .AddQueryParameter("grant_type", "client_credentials")
            .AddQueryParameter("scope", string.Join('+', _options.Permissions.Select(p => p.ToString())),false);

        var response = await client.GetAsync<TokenResponse>(request);
        _tokenExpiration = DateTime.Now.AddSeconds(response!.ExpiresIn);
        return $"{response!.TokenType} {response!.AccessToken}";

    }

}