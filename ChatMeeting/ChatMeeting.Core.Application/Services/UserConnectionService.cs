using ChatMeeting.Core.Domain.Interfaces.Services;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace ChatMeeting.Core.Application.Services;

public class UserConnectionService : IUserConnectionService
{
    private readonly ConcurrentDictionary<string, string> _userConnections = new();
    public void AddConnection(string username, string connectionId)
    {
        if (!string.IsNullOrEmpty(username))
        {
            _userConnections[username] = connectionId;
        }
    }

    public void RemoveConnection(string username)
    {
        if (!string.IsNullOrEmpty(username))
        {
            _userConnections.TryRemove(username, out _);
        }
    }

    public string GetClaimValue(ClaimsPrincipal? user, string claimType)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        var claimValue = user.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;

        if (claimValue == null)
        {
            throw new ArgumentNullException(claimValue);
        }

        return claimValue;
    }
}
