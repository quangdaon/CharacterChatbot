using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;

namespace CharacterChatbot.Teams.Factories;

public class MsGraphClientFactory
{
  private readonly IConfiguration _config;
  private static readonly string[] Scopes = ["User.Read", "Chat.ReadWrite", "ChatMember.Read", "ChatMessage.Read"];

  public MsGraphClientFactory(IConfiguration config)
  {
    _config = config;
  }

  public GraphServiceClient Create()
  {
    var domain = _config.GetValue<string>("MicrosoftGraph:Domain");

    var username = ReadUsername();
    var password = ReadPassword();
    
    return GetAuthenticatedGraphClient($"{username}@{domain}", password);
  }

  private GraphServiceClient GetAuthenticatedGraphClient(string username, string password)
  {
    var authProvider = CreateAuthenticationProvider(username, password);
    return new GraphServiceClient(authProvider);
  }

  private MsalAuthenticationProvider CreateAuthenticationProvider(string username, string password)
  {
    var clientId = _config.GetValue<string>("MicrosoftGraph:ClientId");
    var tenantId = _config.GetValue<string>("MicrosoftGraph:TenantId");
    var authority = $"https://login.microsoftonline.com/{tenantId}";

    var cca = PublicClientApplicationBuilder.Create(clientId).WithAuthority(authority).Build();
    return MsalAuthenticationProvider.GetInstance(cca, Scopes, username, password);
  }

  private static string ReadUsername()
  {
    Console.Write("Username: ");
    return Console.ReadLine();
  }

  private static string ReadPassword()
  {
    Console.Write("Password: ");
    var password = string.Empty;
    ConsoleKey key;
    do
    {
      var keyInfo = Console.ReadKey(intercept: true);
      key = keyInfo.Key;

      if (key == ConsoleKey.Backspace && password.Length > 0)
      {
        Console.Write("\b \b");
        password = password[0..^1];
      }
      else if (!char.IsControl(keyInfo.KeyChar))
      {
        Console.Write("*");
        password += keyInfo.KeyChar;
      }
    } while (key != ConsoleKey.Enter);
  
    Console.WriteLine();

    return password;
  }
}