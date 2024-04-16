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
}
