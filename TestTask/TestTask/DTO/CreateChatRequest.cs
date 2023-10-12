/// <summary>
/// Request to create a new chat.
/// </summary>
public class CreateChatRequest
{
    /// <summary>
    /// Gets or sets the ID of the user who is creating the chat.
    /// </summary>
    public Guid CreatorId { get; set; }

    /// <summary>
    /// Gets or sets the name of the chat.
    /// </summary>
    public string ChatName { get; set; }
}