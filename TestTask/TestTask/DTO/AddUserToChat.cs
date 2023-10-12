/// <summary>
/// Request to add a user to a chat.
/// </summary>
public class AddUserToChat
{
    /// <summary>
    /// Gets or sets the ID of the user to add to the chat.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the chat to which the user should be added.
    /// </summary>
    public Guid ChatId { get; set; }
}