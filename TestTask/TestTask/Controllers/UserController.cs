using System.Net;
using Microsoft.AspNetCore.Mvc;
using TestApplication.Models;
using TestApplication.Services;

namespace TestApplication.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }
    
    /// <summary>
    /// Create a new user.
    /// </summary>
    /// <param name="request">The user creation request.</param>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/users/create
    ///     {
    ///         "UserName": "JohnDoe"
    ///     }
    ///
    /// </remarks>
    /// <returns>Result of user creation.</returns>
    /// <response code="200">User created successfully.</response>
    /// <response code="409">The provided username is not unique.</response>
    /// <response code="500">An error occurred while attempting to create the user.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        _logger.LogInformation($"CreateUserRequest(Name: {request.UserName};)");
        try
        {
            await _userService.CreateUserAsync(request);
            return Ok("User created successfully");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError($"Username is not unique: {ex.Message}");
            return StatusCode(StatusCodes.Status409Conflict, "Username is not unique.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Can't create user: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't create user");
        }    
    }
    
    
    
    /// <summary>
    /// Get a user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/users/getUser?id=your-guid-here
    ///
    /// </remarks>
    /// <returns>The user with the specified ID.</returns>
    /// <response code="200">The user was successfully retrieved.</response>
    /// <response code="404">The specified user was not found.</response>
    /// <response code="500">An error occurred while attempting to retrieve the user.</response>
    [HttpGet("getUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUser(Guid id)
    {
        try
        {
            var user = await _userService.GetUserAsync(id);
            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound("User not found");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't get user");
        }
    }
    
    /// <summary>
    /// Retrieve a list of users.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/users/getUsers
    ///
    /// </remarks>
    /// <returns>A list of all users in the system.</returns>
    /// <response code="200">The list of users was successfully retrieved.</response>
    /// <response code="404">No users were found in the system.</response>
    /// <response code="500">An error occurred while attempting to retrieve the list of users.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("getUsers")]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var user = await _userService.GetUsersAsync();
            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound("Users not found");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't get users");
        }
    }
}