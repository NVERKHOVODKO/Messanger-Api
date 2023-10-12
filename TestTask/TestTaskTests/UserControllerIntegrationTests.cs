using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestApplication.Data;
using TestApplication.Services;
using TestTask.Repositories;
using TestTask.Repositories.Interfaces;

namespace TestApplication.IntegrationTests
{
    [TestFixture]
    public class UserControllerIntegrationTests
    {
        private IHost _host;
        private IUserService _userService;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<DataContext>(options =>
                        options.UseInMemoryDatabase("TestDatabase"));

                    services.AddScoped<IUserRepository, UserRepository>();
                    services.AddScoped<IUserService, UserService>();
                    services.AddLogging(builder => builder.AddConsole());
                })
                .Build();

            await _host.StartAsync();

            _userService = _host.Services.GetRequiredService<IUserService>();
        }

        [Test]
        public async Task CreateUserAsync_ValidRequest_UserCreatedSuccessfully()
        {
            var createUserRequest = new CreateUserRequest { UserName = "TestUser" };

            await _userService.CreateUserAsync(createUserRequest);

            using var scope = _host.Services.CreateScope();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var user = await userRepository.GetAsync("TestUser");

            Assert.IsNotNull(user);
            Assert.AreEqual("TestUser", user.UserName);
        }
        
        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _host.StopAsync();
            _host.Dispose();
        }
    }
}