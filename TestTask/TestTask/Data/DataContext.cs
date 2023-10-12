using Microsoft.EntityFrameworkCore;
using TestApplication.Models;

namespace TestApplication.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        /*Database.EnsureDeleted();
        Database.EnsureCreated();*/
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.ChatMembers)
            .WithOne(cm => cm.User)
            .HasForeignKey(cm => cm.UserId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Messages)
            .WithOne(m => m.User)
            .HasForeignKey(m => m.UserId);

        modelBuilder.Entity<Chat>()
            .HasMany(c => c.ChatMembers)
            .WithOne(cm => cm.Chat)
            .HasForeignKey(cm => cm.ChatId);

        modelBuilder.Entity<Chat>()
            .HasMany(c => c.Messages)
            .WithOne(m => m.Chat)
            .HasForeignKey(m => m.ChatId);

        modelBuilder.Entity<UserChats>()
            .HasKey(cm => new { cm.UserId, cm.ChatId });

        modelBuilder.Entity<Message>()
            .HasOne(m => m.User)
            .WithMany(u => u.Messages)
            .HasForeignKey(m => m.UserId);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Chat)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ChatId);
    }
    
    public DbSet<Chat> Chats { get; set; }
    public DbSet<UserChats> ChatMembers { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
}