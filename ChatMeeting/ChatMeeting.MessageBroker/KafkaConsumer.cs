using ChatMeeting.Core.Domain;
using ChatMeeting.Core.Domain.Consts;
using ChatMeeting.Core.Domain.Dtos;
using ChatMeeting.Core.Domain.Models;
using ChatMeeting.Core.Domain.Options;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ChatMeeting.MessageBroker;

public class KafkaConsumer : BackgroundService
{
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly KafkaOption _kafkaOption;
    private readonly IDbContextFactory<ChatDbContext> _dbContextFactory;

    public KafkaConsumer(IDbContextFactory<ChatDbContext> dbContextFactory, ILogger<KafkaConsumer> logger, IOptions<KafkaOption> options)
    {
        _kafkaOption = options.Value;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await ConsumeAsync(TopicKafka.Message, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occured while consuming messages");
        }
    }

    private async Task ConsumeAsync(string topic, CancellationToken stoppingToken)
    {
        var config = CreateConsumerConfig();
        using var consumer = new ConsumerBuilder<string, string>(config).Build();

        consumer.Subscribe(topic);
        _logger.LogInformation($"Subscribed to topic: {topic}");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumerResult = consumer.Consume(stoppingToken);
                await ProcessMessageAsync(consumerResult.Message.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured while processing the message.");
                await Task.Delay(1000);
            }
        }
    }

    private async Task ProcessMessageAsync(string value)
    {
        var messageDto = JsonSerializer.Deserialize<MessageDto>(value);
        var message = CreateMessage(messageDto);
        await SaveMessageToDbAsync(message);
    }

    private async Task SaveMessageToDbAsync(Message message)
    {
        try
        {
            var dbContext = _dbContextFactory.CreateDbContext();
            await dbContext.Messages.AddAsync(message);
            await dbContext.SaveChangesAsync();
            _logger.LogInformation($"Message with Id: {message.MessageId} saved in databse");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occured while saving message to database");
            throw;
        }
    }

    private Message CreateMessage(MessageDto? messageDto) => new Message()
    {
        MessageId = messageDto.MessageId,
        SenderId = messageDto.SenderId,
        CreatedAt = messageDto.CreatedAt,
        MessageText = messageDto.MessageText,
        ChatId = messageDto.ChatId
    };

    private ConsumerConfig CreateConsumerConfig() => new ConsumerConfig()
    {
        GroupId = GroupKafka.Message,
        BootstrapServers = _kafkaOption.Url,
        AutoOffsetReset = AutoOffsetReset.Earliest
    };
}
