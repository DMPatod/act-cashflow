using CashFlow.Domain.Transactions;
using CashFlow.Domain.Transactions.Enums;
using CashFlow.Domain.Transactions.Interfaces;
using CashFlow.Domain.Transactions.Messaging;
using MassTransit;

namespace CashFlow.Application.Transactions
{
    public class TransactionService
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(IPublishEndpoint publishEndpoint, ITransactionRepository transactionRepository)
        {
            _publishEndpoint = publishEndpoint;
            _transactionRepository = transactionRepository;
        }

        public async Task<Transaction> AddAsync(double Amount, DateTime? dateTime, TransactionType type)
        {
            dateTime ??= DateTime.UtcNow;

            var transaction = Transaction.Create(
                Amount,
                dateTime.Value,
                type);

            var message = new TransactionCreated(transaction.Id.ToString(), transaction.Amount, transaction.DateTime, type);

            await _publishEndpoint.Publish(message);

            return transaction;
        }

        public async Task UpdateAsync(string id, double amount, DateTime? dateTime, TransactionType type)
        {
            if (!Guid.TryParse(id, out var transactionId))
            {
                throw new ArgumentException("Invalid transaction id.");
            }

            var message = new TransactionUpdated(id, amount, dateTime, type);

            await _publishEndpoint.Publish(message);
        }
    }
}
