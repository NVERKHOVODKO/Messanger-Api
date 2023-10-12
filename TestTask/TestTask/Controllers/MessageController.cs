using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TestApplication.Models;
using TestApplication.Services;
using TestTask.Exceptions;
using TestTask.Hubs;

namespace TestApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly ILogger<MessageController> _logger;
        private readonly IMessageService _messageService;
        
        public MessageController(IHubContext<ChatHub> chatHub, IMessageService messageService, ILogger<MessageController> logger)
        {
            _logger = logger;
            _chatHub = chatHub;
            _messageService = messageService;
        }
        
        
        /// <summary>
        /// Send a message in a chat.
        /// </summary>
        /// <param name="request">Message sending request.</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/messages/sendMessage
        ///     {
        ///         "ChatId": "your-chat-id-here",
        ///         "UserId": "your-user-id-here",
        ///         "MessageText": "Hello, world!"
        ///     }
        ///
        /// </remarks>
        /// <returns>The result of the message sending operation.</returns>
        /// <response code="200">Message sent successfully.</response>
        /// <response code="409">Creator not found (EntityNotFoundException).</response>
        /// <response code="500">An error occurred while sending the message.</response>
        [HttpPost("sendMessage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Send([FromBody] SendMessageRequest request)
        {
            try
            {
                var message = new Message
                {
                    Id = Guid.NewGuid(),
                    ChatId = request.ChatId,
                    UserId = request.UserId,
                    Text = request.MessageText,
                    Timestamp = DateTime.UtcNow
                };
                await _messageService.Send(message);
                await _chatHub.Clients.Group(request.ChatId.ToString()).SendAsync("ReceiveMessage", message);
                return Ok("Message sent successfully!");
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError($"Creator not found: {ex.Message}");
                return StatusCode(StatusCodes.Status409Conflict, $"Creator not found");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Can't create user: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Can't create user");
            }
        }
    }
}