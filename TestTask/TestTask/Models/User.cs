using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestApplication.Models;

public class User
{
    public Guid Id { get; set; }

    [Required]
    public string UserName { get; set; }
    public ICollection<UserChats> ChatMembers { get; set; } = new List<UserChats>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();

    public User(Guid id, string userName)
    {
        Id = id;
        UserName = userName;
    }
}
