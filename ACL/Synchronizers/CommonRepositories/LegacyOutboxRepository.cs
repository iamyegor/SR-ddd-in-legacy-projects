using System.Data.SqlClient;
using Dapper;
using Newtonsoft.Json;

namespace ACL.Synchronizers.CommonRepositories;

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
}
