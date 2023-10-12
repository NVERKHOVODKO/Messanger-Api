using TestApplication.Models;

namespace TestApplication.Services;

public interface IUserService
{
    public Task CreateUserAsync(CreateUserRequest request);
    public Task<User> GetUserAsync(Guid userId);
    public Task<List<User>> GetUsersAsync();
}