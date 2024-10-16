using CashFlow.Domain.Transactions.Enums;
using CashFlow.Domain.Transactions.Interfaces;
using CashFlow.Domain.Transactions.Messaging;
using CashFlow.Infrastructure.Messaging.Transactions.Serializers;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CashFlow.Infrastructure.Messaging.Transactions.Producers
{
    internal class TransactionUpdatedProducer : ITransactionUpdatedProducer
    {
        private readonly ILogger<TransactionUpdatedProducer> _logger;
        private readonly IProducer<Null, TransactionUpdated> _producer;
        private readonly string _topic = "transaction-updated-topic";

        public TransactionUpdatedProducer(ILogger<TransactionUpdatedProducer> logger, IConfiguration configuration)
        {
            _logger = logger;

            var settings = configuration.GetSection("Kafka").Get<KafkaSettings>() ?? throw new ArgumentException("Kafka settings not found");
            _producer = new ProducerBuilder<Null, TransactionUpdated>(new ProducerConfig
            {
                BootstrapServers = settings.BootstrapServers
            })
                .SetValueSerializer(TransactionObjectSerializers.TransactionUpdatedSerializer)
                .Build();
        }

        public async Task PublishTransactionUpdatedAsync(Guid id, double amount, DateTime? dateTime, TransactionType type)
        {
            try
            {
                var result = await _producer.ProduceAsync(_topic, new Message<Null, TransactionUpdated>
                {
                    Value = new TransactionUpdated(
                        id.ToString(),
                        amount,
                        dateTime,
                        type
                    )
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing transaction");
            }
        }
    }
}
