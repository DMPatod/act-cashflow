using CashFlow.Domain.Transactions;
using CashFlow.Domain.Transactions.Messaging;
using CashFlow.Infrastructure.DataPersistence.MSSqlServer;
using CashFlow.Infrastructure.Messaging.Transactions.Serializers;
using Confluent.Kafka;
using DDD.Core.DomainObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CashFlow.Infrastructure.Messaging.Transactions.Consumers
{
    public class UpdatedTransactionConsumer : BackgroundService
    {
        private readonly IConsumer<Ignore, TransactionUpdated> _consumer;
        private readonly string _topic = "transaction-updated-topic";
        private readonly ILogger<UpdatedTransactionConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;

        public UpdatedTransactionConsumer(
            ILogger<UpdatedTransactionConsumer> logger,
            IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

            var settings = configuration.GetSection("Kafka").Get<KafkaSettings>() ?? throw new ArgumentException("Kafka settings not found");
            _consumer = new ConsumerBuilder<Ignore, TransactionUpdated>(new ConsumerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                GroupId = settings.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            })
                .SetValueDeserializer(TransactionObjectSerializers.TransactionUpdatedDeserializer)
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

                        var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>() ?? throw new ArgumentException("Service scope factory not found");
                        using var scope = scopeFactory.CreateScope();
                        var context = scope.ServiceProvider.GetRequiredService<MSSqlServerContext>();
                        var transaction = await context.Set<Transaction>().FirstOrDefaultAsync(t => t.Id == DefaultGuidId.Create(id));
                        if (transaction is null)
                        {
                            throw new InvalidOperationException("Transaction not found");
                        }
                        transaction.Update(message.Amount, message.DateTime, message.Type);
                        context.Set<Transaction>().Update(transaction);
                        await context.SaveChangesAsync(cancellationToken);

                        _consumer.Commit(consumeResult);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Consumer was cancelled");
            }
            finally
            {
                _consumer.Close();
            }
        }
    }
}
