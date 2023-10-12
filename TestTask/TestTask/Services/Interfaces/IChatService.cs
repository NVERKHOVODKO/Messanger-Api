using Microsoft.AspNetCore.Mvc;
using TestApplication.Models;

namespace TestApplication.Services;

public interface IChatService
{
    public Task CreateChatAsync(CreateChatRequest request);
    public Task<List<GetChatsResponse>> GetChatsAsync();
    public Task<Chat> GetChatAsync(Guid chatId);
    public Task AddUserToChatAsync(AddUserToChat request);
    public Task<List<GetChatMembersResponse>> GetChatMembersAsync(Guid chatId);
    public Task RenameChatAsync(RenameChatRequest request);
    
    public Task DeleteChatAsync(DeleteChatRequest request);
    public Task<List<GetChatMessagesResponse>> GetChatMessagesAsync(Guid chatId);
}