using TestApplication.Data;
using TestApplication.Models;
using TestTask.Repositories.Interfaces;

namespace TestTask.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly DataContext _context;

    public MessageRepository(DataContext context)
    {
        _context = context;
    }
    
    public async Task WriteMessageToDataBaseAsync(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
    }
}