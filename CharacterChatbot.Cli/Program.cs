// See https://aka.ms/new-console-template for more information

using CharacterChatbot.Engine;
using CharacterChatbot.Engine.Configuration;
using Microsoft.Extensions.Configuration;

IConfiguration config = new ConfigurationBuilder()
  .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
  .Build();

var chatbotProfile = GetProfile("Penguin");
var context = new ChatContext(chatbotProfile);

Console.WriteLine("You are speaking to {0}.", chatbotProfile.DisplayName);
Console.WriteLine();

while (true)
{
  Console.Write("You: ");
  var next = Console.ReadLine();

  if (string.IsNullOrWhiteSpace(next)) break;

  var response = await context.Process(next);

  Console.WriteLine("{0}: {1}", chatbotProfile.DisplayName, response);
}

ChatbotProfile GetProfile(string key)
{
  return config.GetSection($"ChatbotProfiles:{key}").Get<ChatbotProfile>();
}