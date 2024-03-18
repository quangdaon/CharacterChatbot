// See https://aka.ms/new-console-template for more information

using System.Text;
using CharacterChatbot.Teams;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;

var conf = new ConfigurationBuilder()
  .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
  .AddJsonFile("appsettings.secrets.json", optional: true, reloadOnChange: true)
  .Build();

var domain = conf.GetValue<string>("MicrosoftGraph:Domain");
var user = ReadUsername();
var pass = ReadPassword();

string[] scopes = ["User.Read", "Chat.ReadWrite", "ChatMember.Read", "ChatMessage.Read"];

var client = GetAuthenticatedGraphClient(conf, $"{user}@{domain}", pass);

var profile = await client.Me.GetAsync();

if (profile is null)
{
  Console.WriteLine("Bad Login");
  Environment.Exit(1);
}


var chats = await client.Me.Chats.GetAsync((requestConfiguration) =>
{
  requestConfiguration.QueryParameters.Expand = new[] { "members" };
});

Console.WriteLine(profile.DisplayName);

string ReadUsername()
{
  Console.Write("Username: ");
  return Console.ReadLine();
}

string ReadPassword()
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

GraphServiceClient GetAuthenticatedGraphClient(IConfiguration config, string username, string password)
{
  var authProvider = CreateAuthenticationProvider(config, username, password);
  return new GraphServiceClient(authProvider);
}

IAuthenticationProvider CreateAuthenticationProvider(IConfiguration config, string username, string password)
{
  var clientId = config.GetValue<string>("MicrosoftGraph:ClientId");
  var tenantId = config.GetValue<string>("MicrosoftGraph:TenantId");
  var authority = $"https://login.microsoftonline.com/{tenantId}";

  var cca = PublicClientApplicationBuilder.Create(clientId).WithAuthority(authority).Build();
  return MsalAuthenticationProvider.GetInstance(cca, scopes, username, password);
}