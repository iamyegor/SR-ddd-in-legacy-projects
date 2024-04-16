using System.Data.SqlClient;
using ACL.ConnectionStrings;
using Dapper;
using Newtonsoft.Json;

namespace ACL.Synchronizers.CommonRepositories.Outbox;

public class LegacyOutboxRepository
{
    public void Save<T>(IEnumerable<T> objectsToSave, SqlTransaction transaction)
    {
        string type = typeof(T).Name;
        var jsonListToSave = objectsToSave.Select(obj => new
        {
            Content = JsonConvert.SerializeObject(obj)
        });

        string query = $"insert into outbox (content, type) values (@Content, '{type}')";

        SqlConnection connection = transaction.Connection!;
        connection.Execute(query, jsonListToSave, transaction: transaction);
    }

    public (List<int>, List<T>) Get<T>()
    {
        string type = typeof(T).Name;
        string query = "select id, content from outbox where type = @type";

        using var connection = new SqlConnection(LegacyConnectionString.Value);
        List<OutboxRow> outboxRows = connection.Query<OutboxRow>(query, new { type }).ToList();

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

    public void Remove(List<int> ids)
    {
        string query = "delete from outbox where id = @id";

        using var connection = new SqlConnection(LegacyConnectionString.Value);
        connection.Execute(query, ids.Select(id => new { id }));
    }
}
