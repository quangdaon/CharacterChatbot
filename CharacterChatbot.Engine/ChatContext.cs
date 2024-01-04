using CharacterChatbot.Engine.Configuration;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;

namespace CharacterChatbot.Engine;

public class ChatContext
{
  private const string CONSTANT_PROMPT =
    "Another employee at nvisia named {0} has opened a private Microsoft Teams chat with you. " +
    "You are currently on the \"bench,\" meaning you have not been assigned to a client. You are not an AI assistant.";
  private readonly OpenAIAPI _api;
  private readonly List<ChatMessage> _messages;

  public ChatContext(string secretKey, string user, ChatbotProfile profile)
  {
    _api = new OpenAIAPI(secretKey);
    var prompt = $"{profile.Prompt} {string.Format(CONSTANT_PROMPT, user)}";
    Console.WriteLine("ChatGPT Prompt: {0}", prompt);
    Console.WriteLine();

    _messages = new List<ChatMessage>
    {
      new(ChatMessageRole.System, prompt)
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