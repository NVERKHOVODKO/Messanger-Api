/// <summary>
/// Response containing the list of user IDs in a chat.
/// </summary>
public class GetChatMembersResponse
{
    /// <summary>
    /// Gets or sets the ID of a user in the chat.
    /// </summary>
    public Guid UserId { get; set; }
}