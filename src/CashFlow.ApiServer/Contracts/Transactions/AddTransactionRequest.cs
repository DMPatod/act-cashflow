using CashFlow.Domain.Transactions.Enums;

namespace CashFlow.ApiServer.Contracts.Transactions
{
    public record AddTransactionRequest(double Amount, string? Datetime, TransactionType Type);
}
