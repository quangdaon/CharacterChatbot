// See https://aka.ms/new-console-template for more information

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

var client = GetAuthenticatedGraphClient(conf, $"{user}@{domain}", pass);

var profile = await client.Me.GetAsync();

if (profile is null)
{
  Console.WriteLine("Bad Login");
  Environment.Exit(1);
} 

Console.WriteLine(profile.DisplayName);

string ReadUsername()
{
  Console.WriteLine("Username: ");
  return Console.ReadLine();
}

string ReadPassword()
{
  Console.WriteLine("Password: ");
  return Console.ReadLine();
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

  string[] scopes = ["User.Read", "User.Read.All"];

  var cca = PublicClientApplicationBuilder.Create(clientId).WithAuthority(authority).Build();
  return MsalAuthenticationProvider.GetInstance(cca, scopes, username, password);
}