using TestApplication.Models;

namespace TestApplication.Services;

public interface IMessageService
{
    
    public Task Send(Message message);
}