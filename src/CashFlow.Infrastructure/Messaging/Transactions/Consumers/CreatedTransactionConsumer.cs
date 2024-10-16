using CashFlow.Domain.Transactions;
using CashFlow.Domain.Transactions.Messaging;
using CashFlow.Infrastructure.DataPersistence.MSSqlServer;
using CashFlow.Infrastructure.Messaging.Transactions.Serializers;
using Confluent.Kafka;
using DDD.Core.DomainObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CashFlow.Infrastructure.Messaging.Transactions.Consumers
{
    public class CreatedTransactionConsumer : BackgroundService
    {
        private readonly IConsumer<Ignore, TransactionCreated> _consumer;
        private readonly string _topic = "transaction-created-topic";
        private readonly ILogger<CreatedTransactionConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;

        public CreatedTransactionConsumer(
            ILogger<CreatedTransactionConsumer> logger,
            IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;


            var settings = configuration.GetSection("Kafka").Get<KafkaSettings>() ?? throw new ArgumentException("Kafka settings not found");
            _consumer = new ConsumerBuilder<Ignore, TransactionCreated>(new ConsumerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                GroupId = settings.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            })
                .SetValueDeserializer(TransactionObjectSerializers.TransactionCreatedDeserializer)
                .Build();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => Consumer(stoppingToken), stoppingToken);
        }

        private async void Consumer(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_topic);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var consumeResult = _consumer.Consume(cancellationToken);

                    if (consumeResult is not null)
                    {
                        var message = consumeResult.Message.Value;
                        _logger.LogInformation($"Consumed Transaction '{message.Id}' at: '{DateTime.UtcNow}'");

                        if (!Guid.TryParse(message.Id, out var id))
                        {
                            throw new ArgumentException("Invalid Id");
                        }

                        var transaction = new Transaction(DefaultGuidId.Create(id), message.Amount, message.DateTime, message.Type);
                        var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>() ?? throw new InvalidOperationException("IServiceScopeFactory is not available.");
                        using var scope = scopeFactory.CreateScope();
                        var _context = scope.ServiceProvider.GetRequiredService<MSSqlServerContext>();
                        await _context.AddAsync(transaction, cancellationToken);
                        await _context.SaveChangesAsync(cancellationToken);

                        _consumer.Commit(consumeResult);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming transaction");
            }
            finally
            {
                _consumer.Close();
            }
        }
    }
}
