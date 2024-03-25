using Microsoft.Graph.Models;

namespace CharacterChatbot.Teams.Services.Chatting;

// TODO: Genericize to common models
public interface IChatService
{
  Task<User> GetProfile();
  Task<IEnumerable<Chat>> GetChats();
  Task<IEnumerable<ChatMessage>> GetChatMessages(string id);
  Task<ConversationMember> GetChatParticipant(Chat chat);
}