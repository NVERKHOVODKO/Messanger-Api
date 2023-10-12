using TestApplication.Models;

namespace TestTask.Repositories.Interfaces;

public interface IUserRepository
{
    public Task CreateAsync(CreateUserRequest request);
    public Task<User?> GetAsync(string userName);
    public Task<User?> GetAsync(Guid userId);
    public Task<List<User>?> GetAsync();
}