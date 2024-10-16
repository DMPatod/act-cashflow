using CashFlow.Infrastructure.DataPersistence;
using CashFlow.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Infrastructure
{
    public static class BuildHandler
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDataPersistence(configuration);

            services.AddMessaging();

            return services;
        }
    }
}
