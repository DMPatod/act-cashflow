using CashFlow.Domain.Transactions;
using CashFlow.Domain.Transactions.Interfaces;
using DDD.Core.DomainObjects;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.DataPersistence.MSSqlServer.Repositories
{
    internal class TransactionRepository : ITransactionRepository
    {
        private readonly MSSqlServerContext _context;

        public TransactionRepository(MSSqlServerContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(Transaction transaction)
        {
            await _context.Set<Transaction>().AddAsync(transaction);
            return await _context.SaveChangesAsync();
        }

        public async Task<Transaction?> FindAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid))
            {
                throw new ArgumentException("Invalid id format.");
            }
            return await _context.Set<Transaction>().FirstOrDefaultAsync(t => t.Id == DefaultGuidId.Create(guid));
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            // Could be updated to Execute Update
            _context.Set<Transaction>().Update(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
