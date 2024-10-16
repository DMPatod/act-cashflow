using CashFlow.Domain.Transactions.Enums;

namespace CashFlow.Domain.Transactions.Interfaces
{
    public interface ITransactionUpdatedProducer
    {
        Task PublishTransactionUpdatedAsync(Guid id, double amount, DateTime? dateTime, TransactionType type);
    }
}
