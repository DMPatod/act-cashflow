namespace CashFlow.Infrastructure.Messaging
{
    public record KafkaSettings(string BootstrapServers, string GroupId = "");
}
