using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TestApplication.Models;
using TestApplication.Services;
using TestTask.Exceptions;
using TestTask.Hubs;

namespace TestTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(ILogger<ChatController> logger, IChatService chatService, IHubContext<ChatHub> hubContext)
        {
            _logger = logger;
            _chatService = chatService;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Create a new chat.
        /// </summary>
        /// <param name="request">Chat creation request.</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/chats/create
        ///     {
        ///         "CreatorId": "your-creator-id-here",
        ///         "ChatName": "New Chat"
        ///     }
        ///
        /// </remarks>
        /// <returns>The result of chat creation.</returns>
        /// <response code="200">Chat created successfully.</response>
        /// <response code="409">Chat name is not unique (InvalidOperationException) or Creator not found (EntityNotFoundException).</response>
        /// <response code="500">An error occurred while creating the chat.</response>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateChat(CreateChatRequest request)
        {
            try
            {
                await _chatService.CreateChatAsync(request);
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", $"Chat '{request.ChatName}' created");
                return Ok("Chat created successfully");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Chat name is not unique: {ex.Message}");
                return StatusCode(StatusCodes.Status409Conflict, "Chat name is not unique.");
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
        
        
        /// <summary>
        /// Rename an existing chat.
        /// </summary>
        /// <param name="request">Chat renaming request.</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/chats/rename
        ///     {
        ///         "UserId": "your-user-id-here",
        ///         "ChatId": "your-chat-id-here",
        ///         "NewName": "Updated Chat Name"
        ///     }
        ///
        /// </remarks>
        /// <returns>The result of chat renaming.</returns>
        /// <response code="200">Chat renamed successfully.</response>
        /// <response code="500">An error occurred while renaming the chat.</response>
        [HttpPut("rename")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RenameChat(RenameChatRequest request)
        {
            try
            {
                await _chatService.RenameChatAsync(request);
                return Ok("Chat renamed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Can't rename chat: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Can't rename chat: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Add a user to an existing chat.
        /// </summary>
        /// <param name="request">Request to add a user to a chat.</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/chats/addUserToChat
        ///     {
        ///         "UserId": "your-user-id-here",
        ///         "ChatId": "your-chat-id-here"
        ///     }
        ///
        /// </remarks>
        /// <returns>The result of adding a user to the chat.</returns>
        /// <response code="200">User added to the chat successfully.</response>
        /// <response code="409">User or Chat not found (EntityNotFoundException).</response>
        /// <response code="500">An error occurred while adding the user to the chat.</response>
        [HttpPost("addUserToChat")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddUserToChat(AddUserToChat request)
        {
            try
            {
                await _chatService.AddUserToChatAsync(request);
                await _hubContext.Clients.Group(request.ChatId.ToString()).SendAsync("ReceiveMessage", "System", $"User '{request.UserId}' added to the chat");
                return Ok("User added successfully");
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError($"User or Chat not found: {ex.Message}");
                return StatusCode(StatusCodes.Status409Conflict, $"User or Chat not found");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Can't add user to chat: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Can't add user to chat: {ex.Message}");
            }
        }

        
        /// <summary>
        /// Retrieve a list of chats.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/chats/getChats
        ///
        /// </remarks>
        /// <returns>A list of all chats in the system.</returns>
        /// <response code="200">The list of chats was successfully retrieved.</response>
        /// <response code="404">No chats were found in the system.</response>
        /// <response code="500">An error occurred while attempting to retrieve the list of chats.</response>
        [HttpGet("getChats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<GetChatsResponse>>> GetChats()
        {
            try
            {
                var chats = await _chatService.GetChatsAsync();
                return Ok(chats);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Chat not found: {ex.Message}");
                return StatusCode(StatusCodes.Status404NotFound, "Chat not found");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Can't get chats: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Can't get chats: {ex.Message}");
            }
        }
        
        
        /// <summary>
        /// Retrieve a chat by its ID.
        /// </summary>
        /// <param name="chatId">The ID of the chat to retrieve.</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/chats/getChat?chatId=your-chat-id-here
        ///
        /// </remarks>
        /// <returns>The chat with the specified ID.</returns>
        /// <response code="200">The chat was successfully retrieved.</response>
        /// <response code="404">The specified chat was not found (EntityNotFoundException).</response>
        /// <response code="500">An error occurred while attempting to retrieve the chat.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("getChat")]
        public async Task<ActionResult<List<Chat>>> GetChat(Guid chatId)
        {
            try
            {
                var chats = await _chatService.GetChatAsync(chatId);
                return Ok(chats);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError($"Chat not found: {ex.Message}");
                return StatusCode(StatusCodes.Status404NotFound, "Chat not found");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Can't get chat: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Can't get chat: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Retrieve the members of a chat by its ID.
        /// </summary>
        /// <param name="chatId">The ID of the chat to retrieve members from.</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/chats/getChatMembers?chatId=your-chat-id-here
        ///
        /// </remarks>
        /// <returns>The members of the chat with the specified ID.</returns>
        /// <response code="200">The chat members were successfully retrieved.</response>
        /// <response code="404">The specified chat was not found (EntityNotFoundException).</response>
        /// <response code="500">An error occurred while attempting to retrieve the chat members.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("getChatMembers")]
        public async Task<ActionResult<List<Chat>>> GetChatMembers(Guid chatId)
        {
            try
            {
                var chats = await _chatService.GetChatMembersAsync(chatId);
                return Ok(chats);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError($"Chat not found: {ex.Message}");
                return StatusCode(StatusCodes.Status404NotFound, "Chat not found");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Can't get chat: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Can't get chat: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Retrieve messages from a chat by its ID.
        /// </summary>
        /// <param name="chatId">The ID of the chat to retrieve messages from.</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/chats/getChatMessages?chatId=your-chat-id-here
        ///
        /// </remarks>
        /// <returns>The messages from the chat with the specified ID.</returns>
        /// <response code="200">The chat messages were successfully retrieved.</response>
        /// <response code="500">An error occurred while attempting to retrieve the chat messages.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("getChatMessages")]
        public async Task<ActionResult<List<Chat>>> GetChatMessages(Guid chatId)
        {
            try
            {
                var chats = await _chatService.GetChatMessagesAsync(chatId);
                return Ok(chats);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Can't get messages: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Can't get messages: {ex.Message}");
            }
        }
        
        
        /// <summary>
        /// Delete a chat.
        /// </summary>
        /// <param name="request">Request to delete a chat.</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/chats/delete
        ///     {
        ///         "UserId": "your-user-id-here",
        ///         "ChatId": "your-chat-id-here"
        ///     }
        ///
        /// </remarks>
        /// <returns>The result of deleting the chat.</returns>
        /// <response code="200">The chat was successfully deleted.</response>
        /// <response code="403">The user does not have permission to delete the chat (AccessViolationException).</response>
        /// <response code="404">The specified chat or user was not found (EntityNotFoundException).</response>
        /// <response code="500">An error occurred while attempting to delete the chat.</response>
        [HttpDelete("delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteChat(DeleteChatRequest request)
        {
            try
            {
                await _chatService.DeleteChatAsync(request);
                await _hubContext.Clients.Group(request.ChatId.ToString()).SendAsync("ReceiveMessage", "System", $"Chat '{request.ChatId}' deleted");
                return Ok("Chat deleted");
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError($"Chat or User not found: {ex.Message}");
                return StatusCode(StatusCodes.Status404NotFound, "Chat not found");
            }
            catch (AccessViolationException ex)
            {
                _logger.LogError($"User can't delete this chat: {ex.Message}");
                return StatusCode(StatusCodes.Status403Forbidden, $"User can't delete this chat: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Can't delete user: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Can't delete chat");
            }
        }
    }
}