/// <summary>
/// Request to delete a chat.
/// </summary>
public class DeleteChatRequest
{
    /// <summary>
    /// Gets or sets the ID of the user requesting the chat deletion.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the chat to be deleted.
    /// </summary>
    public Guid ChatId { get; set; }
}