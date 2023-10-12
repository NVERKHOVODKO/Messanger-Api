using TestApplication.Models;
namespace TestTask.Repositories.Interfaces;

public interface IChatRepository
{
    public Task CreateAsync(CreateChatRequest request);
    public Task<List<GetChatsResponse>> GetChatsAsync();
    public Task<Chat> GetAsync(Guid chatId);
    public Task AddUserToChatAsync(UserChats userChat);
    public Task RenameChatAsync(RenameChatRequest request);
    public Task<Chat> GetAsync(string name);
    public Task<List<GetChatMembersResponse>> GetChatMembersAsync(Guid chatId);
    public Task DeleteChatAsync(Guid chatId);
    public Task<List<GetChatMessagesResponse>> GetChatMessagesAsync(Guid chatId);
}