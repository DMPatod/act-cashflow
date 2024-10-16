using CashFlow.Infrastructure.DataPersistence.MSSqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

internal class Program
{
    private static int Main(string[] args)
    {
        try
        {
            Console.WriteLine("Tools Runner Starting.");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);
            var config = builder.Build();

            var connectionString = config.GetConnectionString("DefaultConnection") ?? throw new Exception("Connection string not loaded.");

            var optionsBuilder = new DbContextOptionsBuilder<MSSqlServerContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using var context = new MSSqlServerContext(optionsBuilder.Options);

            Console.WriteLine("Applying Migrations.");

            context.Database.Migrate();

            Console.WriteLine("Migrations Applied.");

            Environment.Exit(0);
            return 0;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");

            Environment.Exit(1);
            return 1;
        }
    }
}