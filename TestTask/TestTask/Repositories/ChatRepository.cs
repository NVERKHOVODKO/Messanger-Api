using Microsoft.EntityFrameworkCore;
using TestApplication.Data;
using TestApplication.Models;
using TestTask.Repositories.Interfaces;

namespace TestTask.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly DataContext _context;

    public ChatRepository(DataContext context)
    {
        _context = context;
    }
    
    public async Task CreateAsync(CreateChatRequest request)
    {
        var chat = new Chat(Guid.NewGuid(), request.ChatName, request.CreatorId);
        _context.Chats.Add(chat);
        await _context.SaveChangesAsync();
    }
    
    
    public async Task RenameChatAsync(RenameChatRequest request)
    {
        var chat = await _context.Chats.FirstOrDefaultAsync(c => c.Id == request.ChatId);
        chat.ChatName = request.NewName;
        await _context.SaveChangesAsync();
    }
    

    public async Task<List<GetChatsResponse>> GetChatsAsync()
    {
        var chats = await _context.Chats
            .Include(chat => chat.ChatMembers)
            .Select(chat => new GetChatsResponse
            {
                ChatId = chat.Id,
                CreatorId = chat.CreatorId,
                ChatName = chat.ChatName,
                AmountOfMembers = chat.ChatMembers.Count
            })
            .ToListAsync();

        return chats;
    }
    
    
    public async Task<bool> IsChatNameUniqueAsync(Guid chatId, string name)
    {
        var userWithSameEmail = await _context.Chats.FirstOrDefaultAsync(c => c.ChatName == name && c.Id != chatId);
        return userWithSameEmail == null;
    }
    
    
    public async Task<bool> IsChatNameUniqueAsync(string name)
    {
        var userWithSameEmail = await _context.Chats.FirstOrDefaultAsync(c => c.ChatName == name);
        return userWithSameEmail == null;
    }
    
    
    public async Task<Chat> GetAsync(Guid chatId)
    {
        var chat = await _context.Chats
            .FirstOrDefaultAsync(c => c.Id == chatId);
        return chat;
    }
    
    public async Task<List<GetChatMembersResponse>> GetChatMembersAsync(Guid chatId)
    {
        var chat = await _context.Chats
            .Where(chat => chat.Id == chatId)
            .Include(chat => chat.ChatMembers)
            .FirstOrDefaultAsync();
        if (chat == null)
        {
            return null;
        }
        var chatMembers = chat.ChatMembers
        .Select(chatMember => new GetChatMembersResponse
        {
            UserId = chatMember.UserId
        })
        .ToList();

        return chatMembers;
    }
    
    public async Task<List<GetChatMessagesResponse>> GetChatMessagesAsync(Guid chatId)
    {
        var messages = await _context.Messages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.Timestamp)
            .Select(m => new GetChatMessagesResponse
            {
                Id = m.Id,
                UserId = m.UserId,
                Text = m.Text,
                Timestamp = m.Timestamp
            })
            .ToListAsync();

        return messages;
    }
    
    public async Task<Chat> GetAsync(string name)
    {
        var chat = await _context.Chats
            .FirstOrDefaultAsync(c => c.ChatName == name);
        return chat;
    }
    public async Task AddUserToChatAsync(UserChats userChat)
    {
        _context.ChatMembers.Add(userChat);
        await _context.SaveChangesAsync();
    }
    
    public async Task<Guid> GetChatCreatorId(Guid chatId)
    {
        var chat = await _context.Chats
            .FirstOrDefaultAsync(c => c.Id == chatId);

        return chat.CreatorId;
    }
    
    public async Task DeleteChatAsync(Guid chatId)
    {
        var chat = await _context.Chats
            .FirstOrDefaultAsync(c => c.Id == chatId);
        if (chat == null)
        {
            throw new InvalidOperationException("Chat not found.");
        }
        _context.Chats.Remove(chat);
        await _context.SaveChangesAsync();
    }
}