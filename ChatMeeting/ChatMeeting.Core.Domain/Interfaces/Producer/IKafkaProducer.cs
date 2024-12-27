
using Confluent.Kafka;

namespace ChatMeeting.Core.Domain.Interfaces.Producer;

public interface IKafkaProducer
{
    Task ProduceAsync(string topic, Message<string, string> message);
}
