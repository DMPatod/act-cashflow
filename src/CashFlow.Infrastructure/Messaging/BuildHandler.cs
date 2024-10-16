using CashFlow.Domain.Transactions.Interfaces;
using CashFlow.Infrastructure.Messaging.Transactions.Consumers;
using CashFlow.Infrastructure.Messaging.Transactions.Producers;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Infrastructure.Messaging
{
    public static class BuildHandler
    {

        public static IServiceCollection AddMessaging(this IServiceCollection services)
        {
            services.AddSingleton<ITransactionCreatedProducer, TransactionCreatedProducer>();
            services.AddSingleton<ITransactionUpdatedProducer, TransactionUpdatedProducer>();

            services.AddHostedService<CreatedTransactionConsumer>();
            services.AddHostedService<UpdatedTransactionConsumer>();

            return services;
        }
    }
}
