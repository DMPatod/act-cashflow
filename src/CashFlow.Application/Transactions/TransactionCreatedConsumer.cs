using CashFlow.Domain;
using CashFlow.Domain.Transactions;
using CashFlow.Domain.Transactions.Messaging;
using Confluent.Kafka;

namespace CashFlow.Application.Transactions
{
    public class TransactionCreatedConsumer : IConsumer<Null, TransactionCreated>
    {
        private readonly UnitOfWork _unitOfWork;

        public TransactionCreatedConsumer(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<TransactionCreated> context)
        {
            var transaction = Transaction.Create(context.Message.Amount, context.Message.DateTime, context.Message.Type);

            await _unitOfWork.TransactionRepository.AddAsync(transaction);
        }
    }
}
