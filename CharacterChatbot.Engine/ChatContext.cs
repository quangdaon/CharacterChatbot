using CharacterChatbot.Engine.Configuration;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;

namespace CharacterChatbot.Engine;

public class ChatContext
{
  private const string CONSTANT_PROMPT = "A user named {0} has opened a private chat with you.";
  private readonly OpenAIAPI _api;
  private readonly List<ChatMessage> _messages;

  public readonly string Prompt;

  public ChatContext(string secretKey, string user, ChatbotProfile profile)
  {
    Prompt = $"{profile.Prompt} {string.Format(CONSTANT_PROMPT, user)}";
    _api = new OpenAIAPI(secretKey);

    _messages = new List<ChatMessage>
    {
      new(ChatMessageRole.System, Prompt)
    };
  }

  public async Task<string> Process(string next)
  {
    var userMessage = new ChatMessage(ChatMessageRole.User, next);
    _messages.Add(userMessage);

    var chatResult = await _api.Chat.CreateChatCompletionAsync(new ChatRequest
    {
      Model = Model.ChatGPTTurbo,
      Temperature = 0.1,
      MaxTokens = 180,
      Messages = _messages
    });


    var responseMessage = chatResult.Choices[0].Message;
    _messages.Add(responseMessage);

    return responseMessage.TextContent;
  }
}