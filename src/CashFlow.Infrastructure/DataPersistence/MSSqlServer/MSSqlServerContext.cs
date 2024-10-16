using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CashFlow.Infrastructure.DataPersistence.MSSqlServer
{
    public class MSSqlServerContext : DbContext
    {
        public MSSqlServerContext(DbContextOptions<MSSqlServerContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
