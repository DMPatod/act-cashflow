using CashFlow.Domain.Transactions;
using CashFlow.Domain.Transactions.Interfaces;
using CashFlow.Domain.Transactions.Messaging;
using CashFlow.Infrastructure.Messaging.Transactions.Serializers;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CashFlow.Infrastructure.Messaging.Transactions.Producers
{
    internal class TransactionCreatedProducer : ITransactionCreatedProducer
    {
        private readonly ILogger<TransactionCreatedProducer> _logger;
        private readonly IProducer<Null, TransactionCreated> _producer;
        private readonly string _topic = "transaction-created-topic";

        public TransactionCreatedProducer(ILogger<TransactionCreatedProducer> logger, IConfiguration configuration)
        {
            _logger = logger;

            var settings = configuration.GetSection("Kafka").Get<KafkaSettings>() ?? throw new ArgumentException("Kafka settings not found");
            _producer = new ProducerBuilder<Null, TransactionCreated>(new ProducerConfig
            {
                BootstrapServers = settings.BootstrapServers
            })
                .SetValueSerializer(TransactionObjectSerializers.TransactionCreatedSerializer)
                .Build();
        }

        public async Task PublishTransactionCreatedAsync(Transaction transaction)
        {
            try
            {
                var result = await _producer.ProduceAsync(_topic, new Message<Null, TransactionCreated>
                {
                    Value = new TransactionCreated(
                        transaction.Id.ToString(),
                        transaction.Amount,
                        transaction.DateTime,
                        transaction.Type
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
