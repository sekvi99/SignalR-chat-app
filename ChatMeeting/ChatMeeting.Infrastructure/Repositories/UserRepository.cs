using ChatMeeting.Core.Domain;
using ChatMeeting.Core.Domain.Interfaces.Repositories;
using ChatMeeting.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatMeeting.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ChatDbContext _context;
    private readonly ILogger<UserRepository> _logger;
    public UserRepository(ChatDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddUserAsync(User user)
    {
        try
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occured while adding user with login: {user.Username}");
            throw;
        }
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            
            if (user == null) _logger.LogWarning($"User with id: {id} not found in database");
            return user;

        } catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occured while fetching user with ID: {id}");
            throw;
        }
    }

    public async Task<User?> GetUserByLoginAsync(string login)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == login);

            if (user == null) _logger.LogWarning($"User with login: {login} not found in database");
            return user;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occured while fetching user with login: {login}");
            throw;
        }
    }
}
