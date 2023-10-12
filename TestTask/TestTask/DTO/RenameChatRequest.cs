/// <summary>
/// Request to rename a chat.
/// </summary>
public class RenameChatRequest
{
    /// <summary>
    /// Gets or sets the ID of the user requesting the chat renaming.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the chat to be renamed.
    /// </summary>
    public Guid ChatId { get; set; }

    /// <summary>
    /// Gets or sets the new name for the chat.
    /// </summary>
    public string NewName { get; set; }
}