using CharacterChatbot.Engine.Configuration;

namespace CharacterChatbot.Engine;

public class ChatContext
{
  private readonly ChatbotProfile _profile;

  public ChatContext(ChatbotProfile profile)
  {
    _profile = profile;
  }

  public Task<string> Process(string next)
  {
    return Task.FromResult($"My name is {_profile.DisplayName}.");
  }
}
