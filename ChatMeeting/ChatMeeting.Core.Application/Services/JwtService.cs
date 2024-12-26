using ChatMeeting.Core.Domain.Dtos;
using ChatMeeting.Core.Domain.Interfaces.Services;
using ChatMeeting.Core.Domain.Models;
using ChatMeeting.Core.Domain.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatMeeting.Core.Application.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettingsOption _jwtSettingsOption;

    public JwtService(IOptions<JwtSettingsOption> jwtSettingsOption)
    {
        _jwtSettingsOption = jwtSettingsOption.Value;
    }

    public AuthDto GenerateJwtToken(User user)
    {
        var claims = GetClaims(user);
        var expiryDate = DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettingsOption.ExpiryInMinutes));
        var signingCredentials = GetCredentials();

        var token = new JwtSecurityToken(
                claims: claims,
                expires: expiryDate,
                signingCredentials: signingCredentials
        );

        return new AuthDto()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiredDate = expiryDate
        };
    }

    private SigningCredentials GetCredentials()
    {
        var byteSecretKey = Encoding.ASCII.GetBytes(_jwtSettingsOption.SecretKey);
        var key = new SymmetricSecurityKey(byteSecretKey);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        return creds;
    }

    private IEnumerable<Claim> GetClaims(User user) => new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString())
    };
}
