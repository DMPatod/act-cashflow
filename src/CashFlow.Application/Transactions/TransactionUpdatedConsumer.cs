using CashFlow.Domain;
using CashFlow.Domain.Transactions.Messaging;
using MassTransit;

namespace CashFlow.Application.Transactions
{
    public class TransactionUpdatedConsumer : IConsumer<TransactionUpdated>
    {
        private readonly UnitOfWork _unitOfWork;

        public TransactionUpdatedConsumer(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<TransactionUpdated> context)
        {
            var transaction = await _unitOfWork.TransactionRepository.FindAsync(context.Message.Id)
                ?? throw new InvalidOperationException($"Transaction with id {context.Message.Id} not found.");

            transaction.Update(context.Message.Amount, context.Message.DateTime, context.Message.Type);

            await _unitOfWork.TransactionRepository.UpdateAsync(transaction);
        }
    }
}
