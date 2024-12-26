namespace ChatMeeting.Core.Domain.Exceptions;

public class ChatNotFoundException : Exception
{
    public ChatNotFoundException(string chatName) : base($"Chat with name `{chatName}` not found")
    {
    }
}
