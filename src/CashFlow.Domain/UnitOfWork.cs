using CashFlow.Domain.Transactions.Interfaces;

namespace CashFlow.Domain
{
    public class UnitOfWork
    {
        private readonly ITransactionRepository _transactionRepository;

        public UnitOfWork(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public ITransactionRepository TransactionRepository => _transactionRepository;
    }
}
