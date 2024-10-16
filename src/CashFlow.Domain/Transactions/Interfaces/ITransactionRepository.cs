
namespace CashFlow.Domain.Transactions.Interfaces
{
    public interface ITransactionRepository
    {
        Task<int> AddAsync(Transaction transaction);
        Task<Transaction?> FindAsync(string id);
        Task UpdateAsync(Transaction transaction);
    }
}
