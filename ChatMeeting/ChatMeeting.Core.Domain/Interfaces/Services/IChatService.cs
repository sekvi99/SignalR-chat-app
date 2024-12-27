using ChatMeeting.Core.Domain.Dtos;

namespace ChatMeeting.Core.Domain.Interfaces.Services;

public interface IChatService
{
    Task<ChatDto> GetPaginatedChatAsync(string chatName, int pageNumber, int pageSize);
    Task SaveMessageInKafkaAsync(MessageDto message);
}
