using ChatMeeting.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
namespace ChatMeeting.Core.Domain;

public class ChatDbContext : DbContext
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; }
}
