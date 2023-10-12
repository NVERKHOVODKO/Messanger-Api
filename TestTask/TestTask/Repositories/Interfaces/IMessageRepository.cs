using TestApplication.Models;

namespace TestTask.Repositories.Interfaces;

public interface IMessageRepository
{
    public Task WriteMessageToDataBaseAsync(Message message);
}