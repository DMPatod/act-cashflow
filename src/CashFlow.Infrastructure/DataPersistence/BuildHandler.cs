using CashFlow.Domain;
using CashFlow.Domain.Transactions.Interfaces;
using CashFlow.Infrastructure.DataPersistence.MSSqlServer;
using CashFlow.Infrastructure.DataPersistence.MSSqlServer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Infrastructure.DataPersistence
{
    public static class BuildHandler
    {
        public static IServiceCollection AddDataPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MSSqlServerContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<UnitOfWork>();

            return services;
        }
    }
}
