using System.Security.Claims;

namespace ChatMeeting.Core.Domain.Interfaces.Services;

public interface IUserConnectionService
{
    void AddConnection(string username, string connectionId);
    string GetClaimValue(ClaimsPrincipal? user, string claimType);
    void RemoveConnection(string username);
}
