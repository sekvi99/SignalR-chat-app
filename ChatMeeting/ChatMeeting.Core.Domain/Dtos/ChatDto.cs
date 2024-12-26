namespace ChatMeeting.Core.Domain.Dtos;

public class ChatDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public HashSet<MessageDto>? Messages { get; set; }
}
