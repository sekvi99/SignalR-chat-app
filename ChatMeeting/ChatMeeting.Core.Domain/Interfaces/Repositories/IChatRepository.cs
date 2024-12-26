using ChatMeeting.Core.Domain.Models;

namespace ChatMeeting.Core.Domain.Interfaces.Repositories;

public interface IChatRepository
{
    Task<Chat> GetChatWithMessagesAsync(string chatName, int pageNumber, int pageSize);
}
