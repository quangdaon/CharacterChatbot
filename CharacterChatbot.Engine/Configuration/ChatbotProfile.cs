namespace CharacterChatbot.Engine.Configuration;

public record ChatbotProfile
{
  public string DisplayName { get; set; }
  public string Prompt { get; set; }
}