using CashFlow.Domain.Transactions;
using CashFlow.Infrastructure.DataPersistence.TypeConfigurators.Converters;
using DDD.Core.DomainObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashFlow.Infrastructure.DataPersistence.TypeConfigurators
{
    internal class TransactionTypeConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    v => DefaultGuidId.Create(v));

            builder.Property(t => t.Amount)
                .IsRequired();

            builder.Property(t => t.DateTime)
                .IsRequired();

            builder.Property(t => t.Type)
                .HasConversion(EnumsConverters.TransactionTypeConverter);
        }
    }
}
