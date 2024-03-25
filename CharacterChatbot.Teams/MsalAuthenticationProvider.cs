using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace CharacterChatbot.Teams;

// Source: https://www.youtube.com/watch?v=y9JKjMzU4-w
public class MsalAuthenticationProvider : IAuthenticationProvider
{
  private static MsalAuthenticationProvider _singleton;
  private readonly IPublicClientApplication _clientApp;
  private readonly string[] _scopes;
  private readonly string _username;
  private readonly string _password;
  private string _userId;

  private MsalAuthenticationProvider(IPublicClientApplication clientApp, string[] scopes, string username,
    string password)
  {
    _clientApp = clientApp;
    _scopes = scopes;
    _username = username;
    _password = password;
  }

  public static MsalAuthenticationProvider GetInstance(IPublicClientApplication clientApp, string[] scopes,
    string username, string password)
  {
    if (_singleton is null)
    {
      _singleton = new MsalAuthenticationProvider(clientApp, scopes, username, password);
    }

    return _singleton;
  }

  public async Task AuthenticateRequestAsync(RequestInformation request,
    Dictionary<string, object> additionalAuthenticationContext = null,
    CancellationToken cancellationToken = new CancellationToken())
  {
    var accessToken = await GetTokenAsync();
    request.Headers.Add("Authorization", $"Bearer {accessToken}");
  }

  private async Task<string> GetTokenAsync()
  {
    if (!string.IsNullOrEmpty(_userId))
    {
      try
      {
        var account = await _clientApp.GetAccountAsync(_userId);

        if (account is not null)
        {
          var silentResult = await _clientApp.AcquireTokenSilent(_scopes, account).ExecuteAsync();
          return silentResult.AccessToken;
        }
      }
      catch (MsalUiRequiredException)
      {
      }
    }

    var result = await _clientApp.AcquireTokenByUsernamePassword(_scopes, _username, _password).ExecuteAsync();
    _userId = result.Account.HomeAccountId.Identifier;
    return result.AccessToken;
  }
}