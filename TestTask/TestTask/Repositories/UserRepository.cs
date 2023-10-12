using Microsoft.EntityFrameworkCore;
using TestApplication.Data;
using TestApplication.Models;
using TestTask.Repositories.Interfaces;

namespace TestTask.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(DataContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task CreateAsync(CreateUserRequest request)
    {
        var user = new User(Guid.NewGuid(), request.UserName);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"User created (UserName: {user.UserName}; Id: {user.Id};)");
    }
    
    public async Task<User?> GetAsync(string userName)
    {
        var userWithSameUserName = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        return userWithSameUserName;
    }
    
    public async Task<User?> GetAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(cm => cm.ChatMembers)
            .ThenInclude(c => c.Chat)
            .FirstOrDefaultAsync(u => u.Id == userId);
            
        return user;
    }
    
    public async Task<List<User>?> GetAsync()
    {
        var user = await _context.Users
            .Include(cm => cm.ChatMembers)
            .ThenInclude(c => c.Chat)
            .ToListAsync();
            
        return user;
    }
}