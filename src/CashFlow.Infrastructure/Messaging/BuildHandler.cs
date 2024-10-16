using CashFlow.Domain.Transactions.Messaging;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Infrastructure.Messaging
{
    public static class BuildHandler
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            var kafkaSettings = configuration.GetSection("Kafka").Get<KafkaSettings>() ?? throw new System.Exception("Kafka settings not found");

            services.AddSingleton(sp =>
            {
                var config = new ProducerConfig
                {
                    BootstrapServers = kafkaSettings.BootstrapServers
                };
                return new ProducerBuilder<Null, TransactionCreated>(config).Build();
            });

            services.AddSingleton(sp =>
            {
                var cfg = new ConsumerConfig
                {
                    BootstrapServers = kafkaSettings.BootstrapServers,
                    GroupId = kafkaSettings.GroupId,
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };

                return new ConsumerBuilder<Null, TransactionCreated>(cfg).Build();
            });


            return services;
        }
    }
}
