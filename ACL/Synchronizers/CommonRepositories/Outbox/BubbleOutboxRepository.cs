using ACL.ConnectionStrings;
using Dapper;
using Newtonsoft.Json;
using Npgsql;

namespace ACL.Synchronizers.CommonRepositories.Outbox;

public class BubbleOutboxRepository
{
    public void Save<T>(IEnumerable<T> objectsToSave, NpgsqlTransaction transaction)
    {
        var jsonListToSave = objectsToSave.Select(obj => new
        {
            Content = JsonConvert.SerializeObject(obj)
        });

        string type = typeof(T).Name;
        string query = $"insert into outbox (content, type) values (@Content::jsonb, '{type}')";

        NpgsqlConnection connection = transaction.Connection!;
        connection.Execute(query, jsonListToSave, transaction: transaction);
    }

    public (List<int> ids, List<T> deliverieFromOutbox) Get<T>()
    {
        string type = typeof(T).Name;
        string query = "select id, content from outbox where type = @type";

        using var connection = new NpgsqlConnection(BubbleConnectionString.Value);
        List<OutboxRow> outboxRows = connection.Query<OutboxRow>(query, new { type }).ToList();

        List<T> objectsToReturn = [];
        foreach (var json in outboxRows.Select(r => r.Content))
        {
            T? deserializeObject = JsonConvert.DeserializeObject<T>(json);

            if (deserializeObject == null)
            {
                throw new Exception("Couldn't deserialize outbox entry");
            }

            objectsToReturn.Add(deserializeObject);
        }

        List<int> ids = outboxRows.Select(r => r.Id).ToList();

        return (ids, objectsToReturn);
    }

    public void Remove(List<int> ids)
    {
        string query = "delete from outbox where id = any(@ids)";

        using var connection = new NpgsqlConnection(BubbleConnectionString.Value);
        connection.Execute(query, new { ids });
    }
}
