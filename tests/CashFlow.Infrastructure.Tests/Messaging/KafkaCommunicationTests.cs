using CashFlow.Application.Transactions;
using CashFlow.Domain;
using CashFlow.Domain.Transactions;
using CashFlow.Domain.Transactions.Enums;
using CashFlow.Domain.Transactions.Interfaces;
using CashFlow.Domain.Transactions.Messaging;
using DDD.Core.DomainObjects;
using MassTransit.Testing;
using Moq;

namespace CashFlow.Infrastructure.Tests.Messaging
{
    public class KafkaCommunicationTests
    {
        [Fact]
        public async Task Should_Consume_TransactionCreated_Event()
        {
            var transactionRepository = new Mock<ITransactionRepository>();
            transactionRepository.Setup(r => r.AddAsync(It.IsAny<Transaction>()))
                                 .ReturnsAsync(0);

            var harness = new InMemoryTestHarness();
            var consumerHarness = harness.Consumer(() => new TransactionCreatedConsumer(new UnitOfWork(transactionRepository.Object)));

            await harness.Start();

            try
            {
                var message = new TransactionCreated
                (
                    Guid.NewGuid().ToString(),
                    100,
                    DateTime.Now,
                    TransactionType.Debit
                );

                await harness.InputQueueSendEndpoint.Send(message);

                Assert.True(consumerHarness.Consumed.Select<TransactionCreated>().Any());
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}
