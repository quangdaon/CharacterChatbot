using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;

namespace CharacterChatbot.Teams.Services.Chat;

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

  public async Task<ChatCollectionResponse> GetChats()
  {
    return await _client.Me.Chats.GetAsync((requestConfiguration) =>
    {
      requestConfiguration.QueryParameters.Expand = ["members"];
    });
  }
  
  
}