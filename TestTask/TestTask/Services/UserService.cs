using TestApplication.Models;
using TestTask.Repositories.Interfaces;

namespace TestApplication.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        
        public async Task CreateUserAsync(CreateUserRequest request)
        {
            if(await IsUsernameUniqueAsync(request.UserName))
                await _userRepository.CreateAsync(request);
            else
            {
                throw new InvalidOperationException("Username is not unique.");
            }
        }
        
        private async Task<bool> IsUsernameUniqueAsync(string name)
        {
            var userWithSameEmail = await _userRepository.GetAsync(name);
            return userWithSameEmail == null;
        }
        
        public async Task<User> GetUserAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User doesn't exists");
            }
            
            return user;
        }
        
        public async Task<List<User>> GetUsersAsync()
        {
            var users = await _userRepository.GetAsync();
            if (users == null)
            {
                throw new InvalidOperationException("User doesn't exists");
            }
            
            return users;
        }
    }
}