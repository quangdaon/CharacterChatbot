using Microsoft.Graph.Models;

namespace CharacterChatbot.Teams.Services.Chat;

// TODO: Genericize to common models
public interface IChatService
{
  Task<User> GetProfile();
  Task<ChatCollectionResponse> GetChats();
}