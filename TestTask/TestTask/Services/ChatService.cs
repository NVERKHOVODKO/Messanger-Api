using TestApplication.Models;
using TestTask.Controllers;
using TestTask.Exceptions;
using TestTask.Repositories.Interfaces;

namespace TestApplication.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ChatController> _logger;

    public ChatService(ILogger<ChatController> logger, IChatRepository chatRepository, IUserRepository userRepository)
    {
        _chatRepository = chatRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task CreateChatAsync(CreateChatRequest request)
    {
        if (!await IsChatNameUniqueAsync(request.ChatName))
            throw new InvalidOperationException("Chat name is not unique.");
        else if (await _userRepository.GetAsync(request.CreatorId) == null)
            throw new EntityNotFoundException("Creator doesn't exists.");
        else
        {
            await _chatRepository.CreateAsync(request);
            var chat = await _chatRepository.GetAsync(request.ChatName);
            var userChat = new UserChats
            {
                UserId = request.CreatorId,
                ChatId = chat.Id
            };
            await _chatRepository.AddUserToChatAsync(userChat);
        }
    }
    
    public async Task RenameChatAsync(RenameChatRequest request)
    {
        var user = await _userRepository.GetAsync(request.UserId);
        var chat = await _chatRepository.GetAsync(request.ChatId);
        if (!await IsChatNameUniqueAsync(request.ChatId, request.NewName))
            throw new InvalidOperationException("Chat name is not unique.");
        if (chat == null)
            throw new EntityNotFoundException("Chat not found");
        else if (user == null)
            throw new EntityNotFoundException("User not found");
        else if (chat.CreatorId != user.Id)
            throw new AccessViolationException("You can't to edit this chat.");
        else
            await _chatRepository.RenameChatAsync(request);
        await _chatRepository.RenameChatAsync(request);

    }

    private async Task<bool> IsChatNameUniqueAsync(string name)
    {
        var chatWithSameName = await _chatRepository.GetAsync(name);
        return chatWithSameName == null;
    }
    
    private async Task<bool> IsChatNameUniqueAsync(Guid chatId, string name)
    {
        var chatWithSameName = await _chatRepository.GetAsync(name);
        if (chatWithSameName == null)
        {
            return true;
        }else if (chatWithSameName.Id == chatId)
        {
            return true;
        }
        return chatWithSameName.Id == chatId;
    }
    
    public async Task<List<GetChatsResponse>> GetChatsAsync()
    {
        var chats = await _chatRepository.GetChatsAsync();
        return chats;
    }

    public async Task<Chat> GetChatAsync(Guid chatId)
    {
        var chat = await _chatRepository.GetAsync(chatId);
        if (chat == null)
            throw new EntityNotFoundException("Chat not found");
        return chat;
    }
    
    public async Task<List<GetChatMembersResponse>> GetChatMembersAsync(Guid chatId)
    {
        var chat = await _chatRepository.GetChatMembersAsync(chatId);
        if (chat == null)
            throw new EntityNotFoundException("Chat not found");
        return chat;
    }
    
    public async Task<List<GetChatMessagesResponse>> GetChatMessagesAsync(Guid chatId)
    {
        var messages = await _chatRepository.GetChatMessagesAsync(chatId);
        return messages;
    }
    
    public async Task AddUserToChatAsync(AddUserToChat request)
    {
        var chat = await _chatRepository.GetAsync(request.ChatId);
        if (chat == null)
            throw new EntityNotFoundException("Chat not found");
        var user = await _userRepository.GetAsync(request.UserId);
        if (user == null)
            throw new EntityNotFoundException("User not found");
        var userChat = new UserChats
        {
            UserId = request.UserId,
            ChatId = request.ChatId
        };
        await _chatRepository.AddUserToChatAsync(userChat);
    }
    
    public async Task DeleteChatAsync(DeleteChatRequest request)
    {
        var chat = await _chatRepository.GetAsync(request.ChatId);
        if(chat == null)
            throw new EntityNotFoundException("Chat not found");
        var user = await _userRepository.GetAsync(request.UserId);
        if(user == null)
            throw new EntityNotFoundException("User not found");
        if (chat.CreatorId != user.Id)
            throw new AccessViolationException("User is not authorized to delete this chat.");
        await _chatRepository.DeleteChatAsync(request.ChatId);
    }
}