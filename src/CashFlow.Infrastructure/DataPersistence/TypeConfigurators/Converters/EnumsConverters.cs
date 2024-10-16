using CashFlow.Domain.Transactions.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CashFlow.Infrastructure.DataPersistence.TypeConfigurators.Converters
{
    internal class EnumsConverters
    {
        public static readonly ValueConverter<TransactionType, string> TransactionTypeConverter = new(
            v => v.ToString(),
            v => (TransactionType)Enum.Parse(typeof(TransactionType), v));
    }
}
