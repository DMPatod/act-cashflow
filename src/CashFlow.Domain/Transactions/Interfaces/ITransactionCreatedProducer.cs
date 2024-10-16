namespace CashFlow.Domain.Transactions.Interfaces
{
    public interface ITransactionCreatedProducer
    {
        Task PublishTransactionCreatedAsync(Transaction transaction);
    }
}
