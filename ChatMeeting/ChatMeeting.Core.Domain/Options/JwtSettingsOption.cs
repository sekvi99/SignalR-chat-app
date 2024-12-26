namespace ChatMeeting.Core.Domain.Options;

public class JwtSettingsOption
{
    public string SecretKey { get; set; } = string.Empty;
    public int ExpiryInMinutes { get; set; }
}
