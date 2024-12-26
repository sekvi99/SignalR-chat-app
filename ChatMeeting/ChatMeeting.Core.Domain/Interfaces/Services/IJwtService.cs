using ChatMeeting.Core.Domain.Dtos;
using ChatMeeting.Core.Domain.Models;

namespace ChatMeeting.Core.Domain.Interfaces.Services;

public interface IJwtService
{
    AuthDto GenerateJwtToken(User user);
}
