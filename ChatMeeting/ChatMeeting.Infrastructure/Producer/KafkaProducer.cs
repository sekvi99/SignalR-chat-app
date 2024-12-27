using ChatMeeting.Core.Domain.Consts;
using ChatMeeting.Core.Domain.Interfaces.Producer;
using ChatMeeting.Core.Domain.Options;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChatMeeting.Infrastructure.Producer;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(IOptions<KafkaOption> options, ILogger<KafkaProducer> logger)
    {
        var kafkaSetting = options.Value;

        var config = new ConsumerConfig
        {
            GroupId = GroupKafka.Message,
            BootstrapServers = kafkaSetting.Url,
            // If lack of information start from earliest message
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
        _logger = logger;
    }

    public async Task ProduceAsync(string topic, Message<string, string> message)
    {
        try
        {
            await _producer.ProduceAsync(topic, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"System got error during send message on topic: {topic}");
            throw;
        }
    }
}
