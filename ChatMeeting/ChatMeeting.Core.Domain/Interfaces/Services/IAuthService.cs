using ChatMeeting.Core.Domain.Dtos;

namespace ChatMeeting.Core.Domain.Interfaces.Services;

public interface IAuthService
{
    Task<AuthDto> GetTokenAsync(LoginDto loginModel);
    Task RegisterUserAsync(RegisterUserDto user);
}
