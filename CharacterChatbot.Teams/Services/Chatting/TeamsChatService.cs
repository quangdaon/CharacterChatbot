using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;

namespace CharacterChatbot.Teams.Services.Chatting;

public class TeamsChatService : IChatService
{
  private readonly GraphServiceClient _client;
  private User _profile;
  
  public TeamsChatService(GraphServiceClient client)
  {
    _client = client;
  }

  public async Task<User> GetProfile()
  {
    if (_profile is not null) return _profile;
    
    try
    {
      _profile = await _client.Me.GetAsync();
    }
    catch (MsalUiRequiredException)
    {
      Console.WriteLine("Bad Login");
      Environment.Exit(1);
    }

    return _profile;
  }

  public async Task<IEnumerable<Chat>> GetChats()
  {
    var response = await _client.Me.Chats.GetAsync(requestConfiguration =>
    {
      requestConfiguration.QueryParameters.Expand = ["members"];
    });

    return response?.Value!.Where(e => e.ChatType == ChatType.OneOnOne);
  }

  public async Task<IEnumerable<ChatMessage>> GetChatMessages(string id)
  {
    var response = await _client.Me.Chats[id].Messages.GetAsync();

    return response?.Value!.Where(e => e.DeletedDateTime is null && e.MessageType == ChatMessageType.Message);
  }

  public async Task<ConversationMember> GetChatParticipant(Chat chat)
  {
    var profile = await GetProfile();
    var members = chat.Members!
      .OfType<AadUserConversationMember>()
      .Where(e => e.UserId != profile.Id);

    return members.Single();
  }

  public Task SendMessage(string id, string message)
  {
    return _client.Me.Chats[id].Messages.PostAsync(new ChatMessage
    {
      Body = new ItemBody
      {
        Content = message
      }
    });
  }
}