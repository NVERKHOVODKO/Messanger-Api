/// <summary>
/// Request to send a message in a chat.
/// </summary>
public class SendMessageRequest
{
    /// <summary>
    /// Gets or sets the ID of the chat where the message will be sent.
    /// </summary>
    public Guid ChatId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user sending the message.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the text of the message to be sent.
    /// </summary>
    public string MessageText { get; set; }
}