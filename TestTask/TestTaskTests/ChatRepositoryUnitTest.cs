using Microsoft.EntityFrameworkCore;
using TestApplication.Data;
using TestApplication.Models;
using TestTask.Repositories;

namespace TestTask.Tests
{
    [TestFixture]
    public class ChatRepositoryTests
    {
        private ChatRepository _chatRepository;
        private DataContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new DataContext(options);
            _chatRepository = new ChatRepository(_context);
        }

        [Test]
        public async Task GetChatsAsync_Should_ReturnChats()
        {
            // Arrange
            _context.Chats.Add(new Chat { Id = Guid.NewGuid(), ChatName = "Chat 1", CreatorId = Guid.NewGuid() });
            _context.Chats.Add(new Chat {  Id = Guid.NewGuid(), ChatName = "Chat 2", CreatorId = Guid.NewGuid() });
            await _context.SaveChangesAsync();

            // Act
            var result = await _chatRepository.GetChatsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task RenameChatAsync_Should_RenameChat()
        {
            // Arrange
            var chat = new Chat { ChatName = "Original Name", CreatorId = Guid.NewGuid() };
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            var request = new RenameChatRequest
            {
                ChatId = chat.Id,
                NewName = "New Name"
            };

            // Act
            await _chatRepository.RenameChatAsync(request);
            var updatedChat = await _context.Chats.FirstOrDefaultAsync();

            // Assert
            Assert.IsNotNull(updatedChat);
            Assert.AreEqual(request.NewName, updatedChat.ChatName);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
