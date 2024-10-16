using CashFlow.Domain.Transactions.Enums;

namespace CashFlow.Domain.Transactions.Messaging
{
    public record TransactionUpdated(string Id, double Amount, DateTime? DateTime, TransactionType Type);
}
