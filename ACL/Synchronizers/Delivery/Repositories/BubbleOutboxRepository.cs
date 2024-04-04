using ACL.ConnectionStrings;
using ACL.Synchronizers.Delivery.Models;
using Dapper;
using Newtonsoft.Json;
using Npgsql;

namespace ACL.Synchronizers.Delivery.Repositories;

public class BubbleOutboxRepository
{
    private readonly NpgsqlConnection _connection;
    private readonly NpgsqlTransaction _transaction;

    public BubbleOutboxRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public void Save(List<DeliveryInLegacy> deliveriesToSave, string type)
    {
        var deliveriesAsJson = deliveriesToSave.Select(delivery => new
        {
            Content = JsonConvert.SerializeObject(delivery)
        });

        string query =
            @$"
            insert into outbox (content, type)
            values (@Content, '{type}')";

        _connection.Execute(query, deliveriesAsJson, transaction: _transaction);
    }

    public (List<int>, List<T>) Get<T>(string type)
    {
        string query =
            @"
            select id as Id, content as Content
            from outbox
            where type = @type";

        using var connection = new NpgsqlConnection(BubbleConnectionString.Value);

        List<OutboxRow> outboxRows = connection.Query<OutboxRow>(query).ToList();

        List<T> objectsToReturn = [];
        foreach (var json in outboxRows.Select(r => r.Content))
        {
            T? deserializedObject = JsonConvert.DeserializeObject<T>(json);

            if (deserializedObject == null)
            {
                throw new Exception("Couldn't deserialize outbox entry");
            }

            objectsToReturn.Add(deserializedObject);
        }

        List<int> ids = outboxRows.Select(r => r.Id).ToList();
        return (ids, objectsToReturn);
    }
}
