using System.ComponentModel.DataAnnotations;

namespace TestApplication.Models;

public class Chat
{
    public Chat(Guid id, string chatName, Guid creatorId)
    {
        Id = id;
        ChatName = chatName;
        CreatorId = creatorId;
    }

    public Chat()
    {
    }

    [Key]
    public Guid Id { get; set; }

    [Required]
    public string ChatName { get; set; }
    public Guid CreatorId { get; set; }
    public User Creator { get; set; }
    public ICollection<UserChats> ChatMembers { get; set; } = new List<UserChats>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
