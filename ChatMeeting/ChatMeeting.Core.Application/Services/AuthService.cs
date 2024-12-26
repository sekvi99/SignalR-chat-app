using ChatMeeting.Core.Domain.Dtos;
using ChatMeeting.Core.Domain.Interfaces.Repositories;
using ChatMeeting.Core.Domain.Interfaces.Services;
using ChatMeeting.Core.Domain.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace ChatMeeting.Core.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AuthService> _logger;
    private readonly IJwtService _jwtService;

    public AuthService(IUserRepository userRepository, ILogger<AuthService> logger, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _logger = logger;
        _jwtService = jwtService;
    }

    public async Task RegisterUserAsync(RegisterUserDto user)
    {
        try
        {
            var existingUser = await _userRepository.GetUserByLoginAsync(user.Username);
            if (existingUser != null)
            {
                _logger.LogWarning("User with this login already exists");
                throw new InvalidOperationException("User with this login already exists");
            }
            var newUser = new User(user.Username, HashPasswword(user.Password));
            await _userRepository.AddUserAsync(newUser);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occured while registering user: {user.Username}");
            throw new InvalidProgramException();
        }
    }

    public async Task<AuthDto> GetTokenAsync(LoginDto loginModel)
    {
        try
        {
            var user = await _userRepository.GetUserByLoginAsync(loginModel.Username);

            if (user == null)
            {
                _logger.LogWarning($"User with login: {loginModel.Username} not found");
                throw new InvalidOperationException("User with this login does not exists");
            }

            if (!VerifyPassword(loginModel.Password, user.Password))
            {
                _logger.LogWarning($"Provided password was incorrect");
                throw new UnauthorizedAccessException("Provided password was incorrect");
            }

            var authData = _jwtService.GenerateJwtToken(user);
            return authData;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}", ex);
            throw;
        }
    }

    private string HashPasswword(string password)
    {
        var salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return $"{Convert.ToBase64String(salt)}:{Hash(password, salt)}";
    }

    private string Hash(string password, byte[] salt) => Convert.ToBase64String(KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 10000, 256/8));

    private bool VerifyPassword(string enteredPassword, string storedPassword)
    {
        var passwordParts = storedPassword.Split(':');

        if (passwordParts.Length != 2)
        {
            throw new FormatException("Unexpected hash format.");
        }

        var salt = Convert.FromBase64String(passwordParts[0]);
        var storedHashedPassword = passwordParts[1];

        var enteredHashedPassword = Hash(enteredPassword, salt);

        return enteredHashedPassword == storedHashedPassword;
    }
}
