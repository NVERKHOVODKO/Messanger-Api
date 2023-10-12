/// <summary>
/// Response containing chat information.
/// </summary>
public class GetChatsResponse
{
    /// <summary>
    /// Gets or sets the ID of the chat.
    /// </summary>
    public Guid ChatId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user who created the chat.
    /// </summary>
    public Guid CreatorId { get; set; }

    /// <summary>
    /// Gets or sets the name of the chat.
    /// </summary>
    public string ChatName { get; set; }

    /// <summary>
    /// Gets or sets the number of members in the chat.
    /// </summary>
    public int AmountOfMembers { get; set; }
}