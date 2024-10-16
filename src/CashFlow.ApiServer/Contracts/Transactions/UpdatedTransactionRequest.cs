using CashFlow.Domain.Transactions.Enums;

namespace CashFlow.ApiServer.Contracts.Transactions
{
    public record UpdatedTransactionRequest(double Amount, string Datetime, TransactionType Type);
}
