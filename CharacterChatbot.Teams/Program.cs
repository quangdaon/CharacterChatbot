// See https://aka.ms/new-console-template for more information

using System.Text;
using CharacterChatbot.Teams;
using CharacterChatbot.Teams.Factories;
using CharacterChatbot.Teams.Services.Chat;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;

var config = new ConfigurationBuilder()
  .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
  .AddJsonFile("appsettings.secrets.json", optional: true, reloadOnChange: true)
  .Build();

var client = new MsGraphClientFactory(config).Create();
var teamsService = new TeamsChatService(client);

var profile = await teamsService.GetProfile();

var chats = await teamsService.GetChats();

Console.WriteLine("Display Name: {0}", profile.DisplayName);
