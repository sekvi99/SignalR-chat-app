using ChatMeeting.Core.Domain.Models;

namespace ChatMeeting.Core.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task AddUserAsync(User user);
    Task<User?> GetUserByIdAsync(Guid id);
    Task<User?> GetUserByLoginAsync(string login);
}
