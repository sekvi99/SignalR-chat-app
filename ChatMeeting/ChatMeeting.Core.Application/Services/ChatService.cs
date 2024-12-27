using ChatMeeting.Core.Domain.Consts;
using ChatMeeting.Core.Domain.Dtos;
using ChatMeeting.Core.Domain.Interfaces.Producer;
using ChatMeeting.Core.Domain.Interfaces.Repositories;
using ChatMeeting.Core.Domain.Interfaces.Services;
using ChatMeeting.Core.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ChatMeeting.Core.Application.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly ILogger<ChatService> _logger;
    private readonly IKafkaProducer _producer;

    public ChatService(ILogger<ChatService> logger, IChatRepository chatRepository, IKafkaProducer producer)
    {
        _logger = logger;
        _chatRepository = chatRepository;
        _producer = producer;
    }

    public async Task<ChatDto> GetPaginatedChatAsync(string chatName, int pageNumber, int pageSize)
    {
        try
        {
            var chat = await _chatRepository.GetChatWithMessagesAsync(chatName, pageNumber, pageSize);
            return ConvertToChatDto(chat);

        } catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }

    public async Task SaveMessageInKafkaAsync(MessageDto message) => await _producer.ProduceAsync(TopicKafka.Message, new Confluent.Kafka.Message<string, string>()
    {
        Key = message.MessageId.ToString(),
        Value = JsonSerializer.Serialize(message)
    });
    private ChatDto ConvertToChatDto(Chat chat)
    {
        var chatDto = new ChatDto()
        {
            Id = chat.ChatId,
            Name = chat.Name,
            Messages = chat.Messages?
                .OrderByDescending(x => x.CreatedAt)
                .Select(m => new MessageDto()
                {
                    MessageId = m.MessageId,
                    Sender = m.Sender.Username,
                    SenderId = m.SenderId,
                    MessageText = m.MessageText,
                    CreatedAt = m.CreatedAt
                })
                .ToHashSet()
        };

        return chatDto;
    }
}
