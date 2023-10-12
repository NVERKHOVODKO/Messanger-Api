namespace TestApplication.Models;

public class UserChats
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ChatId { get; set; }

    public User User { get; set; }
    public Chat Chat { get; set; }
}
