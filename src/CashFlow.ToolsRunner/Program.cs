using CashFlow.Infrastructure.DataPersistence.MSSqlServer;
using CashFlow.Infrastructure.Messaging;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        try
        {
            Console.WriteLine("Tools Runner Starting.");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);
            var config = builder.Build();

            SetupDatabase(config);

            await SetupMessaging(config);

            Environment.Exit(0);
            return 0;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");

            Environment.Exit(1);
            return 1;
        }

        static async Task SetupMessaging(IConfigurationRoot config)
        {
            var settings = config.GetSection("Kafka").Get<KafkaSettings>() ?? throw new Exception("Kafka configuration not loaded.");

            Console.WriteLine("Connecting to Kafka");

            using var adminClient = new AdminClientBuilder(new AdminClientConfig
            {
                BootstrapServers = settings.BootstrapServers
            }).Build();

            try
            {
                var specs = new List<TopicSpecification>
                {
                    new() { Name = "transaction-created-topic", NumPartitions = 1, ReplicationFactor = 1 },
                    new() { Name = "transaction-updated-topic", NumPartitions = 1, ReplicationFactor = 1 }
                };

                await adminClient.CreateTopicsAsync(specs);
            }
            catch (CreateTopicsException ex)
            {
                if (ex.Results.Any(x => x.Error.Code != ErrorCode.TopicAlreadyExists))
                {
                    throw;
                }
            }

            Console.WriteLine("Kafka Configuration Complete.");
        }

        static void SetupDatabase(IConfigurationRoot config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection") ?? throw new Exception("Connection string not loaded.");

            var optionsBuilder = new DbContextOptionsBuilder<MSSqlServerContext>();
            optionsBuilder.UseSqlServer(connectionString);
            var context = new MSSqlServerContext(optionsBuilder.Options);

            Console.WriteLine("Applying Migrations.");

            context.Database.Migrate();

            Console.WriteLine("Migrations Applied.");
        }
    }
}