namespace ChatMeeting.Core.Domain.Models;

public class User
{
    public User(string username, string password)
    {
        Username = username;
        Password = password;
        Id = Guid.NewGuid();
        CreatedAt = DateTime.Now;
    }
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }
}
