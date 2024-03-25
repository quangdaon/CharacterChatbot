// See https://aka.ms/new-console-template for more information

using CharacterChatbot.Teams.Factories;
using CharacterChatbot.Teams.Services.Chatting;
using Microsoft.Extensions.Configuration;

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
Console.WriteLine("Found {0} active chat(s).", chats.Count());

var firstChat = chats.First();

Console.WriteLine();

var messages = await teamsService.GetChatMessages(firstChat.Id);
var participant = await teamsService.GetChatParticipant(firstChat);

await teamsService.SendMessage(firstChat.Id, "Ahoy there!");

Console.WriteLine("Chatting With: {0}", participant.DisplayName);
