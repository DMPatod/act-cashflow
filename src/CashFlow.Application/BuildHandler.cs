using CashFlow.Application.Transactions;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Application
{
    public static class BuildHandler
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<TransactionService>();
            return services;
        }
    }
}
