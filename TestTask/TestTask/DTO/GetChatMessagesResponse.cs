/// <summary>
/// Response containing chat messages.
/// </summary>
public class GetChatMessagesResponse
{
    /// <summary>
    /// Gets or sets the ID of the chat message.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user who sent the message.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the text of the chat message.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the message was sent.
    /// </summary>
    public DateTime Timestamp { get; set; }
}