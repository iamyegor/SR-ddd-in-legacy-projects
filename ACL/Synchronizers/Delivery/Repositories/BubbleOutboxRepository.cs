using ACL.Synchronizers.Product.Models;
using ACL.Utils;
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

    public BubbleOutboxRepository() { }

    public (List<int>, List<T> objectsToReturn) Get<T>(string type)
    {
        using var connection = new NpgsqlConnection(BubbleConnectionString.Value);

        string query =
            @"
            select 
                id as Id, 
                content as Content
            from outbox
            where type = @type";

        List<OutboxRow> outboxRows = connection.Query<OutboxRow>(query, new { type }).ToList();

        List<T> objectsToReturn = [];
        foreach (var json in outboxRows.Select(o => o.Content))
        {
            T? deserializedObject = JsonConvert.DeserializeObject<T>(json);
            if (deserializedObject == null)
            {
                throw new Exception("Couldn't deserialize outbox content");
            }

            objectsToReturn.Add(deserializedObject);
        }

        List<int> ids = outboxRows.Select(r => r.Id).ToList();

        return (ids, objectsToReturn);
    }

    public void Save(IEnumerable<object> objectsToSave, string type)
    {
        var contentsList = objectsToSave.Select(obj => new
        {
            Content = JsonConvert.SerializeObject(obj)
        });

        string query =
            @$"
            insert into outbox (content, type)
            values (@Content::jsonb, '{type}')";

        _connection.Execute(query, contentsList, transaction: _transaction);
    }

    public void Remove(List<int> ids)
    {
        using var context = new NpgsqlConnection(BubbleConnectionString.Value);

        string query =
            @"
            delete from outbox
            where id = @id";

        context.Execute(query, ids.Select(id => new { id }));
    }
}
