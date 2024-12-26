using ChatMeeting.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
namespace ChatMeeting.Core.Domain;

public class ChatDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }

    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Chat)
            .WithMany(m => m.Messages)
            .HasForeignKey(m => m.ChatId);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.Messages)
            .HasForeignKey(m => m.SenderId);

        modelBuilder.Entity<Chat>()
            .HasData(
                new Chat()
                {
                    ChatId = Guid.NewGuid(),
                    Name = "Global",
                    CreatedAt = DateTime.UtcNow,
                }
            );
    }
}
