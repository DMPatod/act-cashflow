using CashFlow.Domain.Transactions.Enums;

namespace CashFlow.Domain.Transactions.Messaging
{
    public record TransactionCreated(
        string Id,
        double Amount,
        DateTime DateTime,
        TransactionType Type);
}
