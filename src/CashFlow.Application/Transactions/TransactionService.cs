using CashFlow.Domain.Transactions;
using CashFlow.Domain.Transactions.Enums;
using CashFlow.Domain.Transactions.Interfaces;

namespace CashFlow.Application.Transactions
{
    public class TransactionService
    {
        private readonly ITransactionCreatedProducer _transactionCreatedProducer;
        private readonly ITransactionUpdatedProducer _transactionUpdatedProducer;

        public TransactionService(ITransactionCreatedProducer transactionCreatedProducer, ITransactionUpdatedProducer transactionUpdatedProducer)
        {
            _transactionCreatedProducer = transactionCreatedProducer;
            _transactionUpdatedProducer = transactionUpdatedProducer;
        }

        public async Task<Transaction> AddAsync(double amount, DateTime? dateTime, TransactionType type)
        {
            var transactionDateTime = dateTime ?? DateTime.Now;
            var transaction = Transaction.Create(amount, transactionDateTime, type);
            await _transactionCreatedProducer.PublishTransactionCreatedAsync(transaction);
            return transaction;
        }

        public async Task UpdateAsync(string id, double amount, DateTime? dateTime, TransactionType type)
        {
            if (!Guid.TryParse(id, out var guid))
            {
                throw new ArgumentException("Invalid id");
            }
            await _transactionUpdatedProducer.PublishTransactionUpdatedAsync(guid, amount, dateTime, type);

            await Task.CompletedTask;
        }
    }
}
