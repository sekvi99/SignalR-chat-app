namespace ChatMeeting.Core.Domain.Dtos;

public class AuthDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiredDate { get; set; }
}
