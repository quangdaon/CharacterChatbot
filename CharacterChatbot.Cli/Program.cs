// See https://aka.ms/new-console-template for more information

using CharacterChatbot.Engine;
using CharacterChatbot.Engine.Configuration;
using Microsoft.Extensions.Configuration;

IConfiguration config = new ConfigurationBuilder()
  .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
  .AddJsonFile("appsettings.secrets.json", optional: true, reloadOnChange: true)
  .Build();

var chatbotProfile = GetProfile();

var secretKey = config.GetValue<string>("OpenAi:SecretKey");
var context = new ChatContext(secretKey, "Quangdao Nguyen", chatbotProfile);

Console.WriteLine("ChatGPT Prompt: {0}", context.Prompt);
Console.WriteLine();

Console.WriteLine("You are speaking to {0}.", chatbotProfile.DisplayName);
Console.WriteLine();

await DoMessageLoop();
return;

async Task DoMessageLoop()
{
  Console.Write("You: ");
  var next = Console.ReadLine();

  if (string.IsNullOrWhiteSpace(next)) return;

  var response = await context.Process(next);

  Console.WriteLine("{0}: {1}", chatbotProfile.DisplayName, response);

  await DoMessageLoop();
}

ChatbotProfile GetProfile()
{
  var sections = config.GetSection("ChatbotProfiles").Get<Dictionary<string, ChatbotProfile>>();
  var profiles = sections.Values.ToArray();

  var i = 0;
  foreach (var profile in profiles)
  {
    Console.WriteLine($"{i + 1}) {profile.DisplayName}");
    i++;
  }

  Console.WriteLine();
  Console.Write("Select a profile: ");

  var selection = SelectProfile(profiles);

  Console.WriteLine();
  return selection;
}

ChatbotProfile SelectProfile(ChatbotProfile[] profiles)
{
  var input = Console.ReadLine();

  if (string.IsNullOrWhiteSpace(input)) Environment.Exit(0);

  try
  {
    var entry = int.Parse(input);
    var index = entry - 1;

    return profiles[index];
  }
  catch
  {
    Console.Write("Invalid input. Try again: ");
    return SelectProfile(profiles);
  }
}