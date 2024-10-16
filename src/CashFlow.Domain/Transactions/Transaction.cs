using CashFlow.Domain.Transactions.Enums;
using DDD.Core.DomainObjects;

namespace CashFlow.Domain.Transactions
{
    public class Transaction : AggregateRoot<DefaultGuidId>
    {
        public double Amount { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;

        public TransactionType Type { get; set; }

        internal Transaction(DefaultGuidId id, double amount, DateTime dateTime, TransactionType type)
            : base(id)
        {
            Amount = amount;
            DateTime = dateTime;
            Type = type;
        }

        public static Transaction Create(double amount, DateTime dateTime, TransactionType type)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero.");
            }

            return new Transaction(
                DefaultGuidId.Create(),
                amount,
                dateTime,
                type);
        }

        public void Update(double amount, DateTime? dateTime, TransactionType type)
        {
            if (Id is null)
            {
                throw new ArgumentException("Transaction not instaciated");
            }

            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero.");
            }

            Amount = amount;
            DateTime = dateTime ?? DateTime.Now;
            Type = type;
        }
    }
}
