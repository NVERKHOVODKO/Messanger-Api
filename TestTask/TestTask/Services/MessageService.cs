using TestApplication.Models;
using TestTask.Exceptions;
using TestTask.Repositories.Interfaces;

namespace TestApplication.Services;

public class MessageService : IMessageService
{ 
    private readonly IMessageRepository _messageRepository;
    private readonly IChatRepository _chatRepository;
    private readonly IUserRepository _userRepository;


    public MessageService(IMessageRepository messageRepository, IChatRepository chatRepository, IUserRepository userRepository)
    {
        _messageRepository = messageRepository;
        _chatRepository = chatRepository;
        _userRepository = userRepository;
    }
    
    public async Task Send(Message message)
    {
        var chat = await _chatRepository.GetAsync(message.ChatId);
        if (chat == null)
            throw new EntityNotFoundException("Chat not found");
        var user = await _userRepository.GetAsync(message.UserId);
        if (user == null)
            throw new EntityNotFoundException("User not found");
        await _messageRepository.WriteMessageToDataBaseAsync(message);
    }
}