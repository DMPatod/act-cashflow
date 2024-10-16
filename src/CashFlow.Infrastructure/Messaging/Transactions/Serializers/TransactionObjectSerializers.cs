
using CashFlow.Domain.Transactions.Messaging;
using Confluent.Kafka;
using Newtonsoft.Json;
using System.Text;

namespace CashFlow.Infrastructure.Messaging.Transactions.Serializers
{
    internal static class TransactionObjectSerializers
    {
        public static TransactionCreatedDeserializer TransactionCreatedDeserializer => new TransactionCreatedDeserializer();
        public static TransactionUpdatedDeserializer TransactionUpdatedDeserializer => new TransactionUpdatedDeserializer();
        public static TransactionCreatedSerializer TransactionCreatedSerializer => new TransactionCreatedSerializer();
        public static TransactionUpdatedSerializer TransactionUpdatedSerializer => new TransactionUpdatedSerializer();
    }

    class TransactionCreatedDeserializer : IDeserializer<TransactionCreated>
    {
        public TransactionCreated Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            var jsonString = System.Text.Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<TransactionCreated>(jsonString);
        }
    }

    class TransactionUpdatedDeserializer : IDeserializer<TransactionUpdated>
    {
        public TransactionUpdated Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            var jsonString = System.Text.Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<TransactionUpdated>(jsonString);
        }
    }

    class TransactionCreatedSerializer : ISerializer<TransactionCreated>
    {
        public byte[] Serialize(TransactionCreated data, SerializationContext context)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
        }
    }

    class TransactionUpdatedSerializer : ISerializer<TransactionUpdated>
    {
        public byte[] Serialize(TransactionUpdated data, SerializationContext context)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
        }
    }
}
