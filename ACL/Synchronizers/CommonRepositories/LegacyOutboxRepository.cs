using System.Data.SqlClient;
using ACL.Synchronizers.Product.Models;
using ACL.Utils;
using Dapper;
using Newtonsoft.Json;

namespace ACL.Synchronizers.CommonRepositories;

public class LegacyOutboxRepository
{
    private readonly SqlConnection? _connection;
    private readonly SqlTransaction? _transaction;

    public LegacyOutboxRepository(SqlConnection connection, SqlTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public LegacyOutboxRepository() { }

    public void Save(IEnumerable<object> objectsToSave, string type)
    {
        if (_connection == null || _transaction == null)
        {
            throw new Exception("Invalid connection or transaction");
        }

        var objectsToInert = objectsToSave.Select(obj => new
        {
            Content = JsonConvert.SerializeObject(obj)
        });

        string query =
            @$"
            insert into Outbox (Content, Type) 
            values (@Content, '{type}');";

        _connection.Execute(query, objectsToInert, transaction: _transaction);
    }

    public (List<int>, List<T>) Get<T>(string type)
    {
        string query =
            @"
            select Id, Content
            from Outbox
            where Type = @type";

        using var connection = new SqlConnection(LegacyConnectionString.Value);

        List<OutboxRow> outboxRows = connection.Query<OutboxRow>(query, new { type }).ToList();

        List<T> objectsToReturn = [];
        foreach (var json in outboxRows.Select(r => r.Content))
        {
            T? deserializedObject = JsonConvert.DeserializeObject<T>(json);

            if (deserializedObject == null)
            {
                throw new Exception("Invalid outbox object");
            }

            objectsToReturn.Add(deserializedObject);
        }

        List<int> idsToReturn = outboxRows.Select(r => r.Id).ToList();
        return (idsToReturn, objectsToReturn);
    }

    public void Remove(List<int> ids)
    {
        string query =
            @"
            delete from Outbox
            where Id = @id";

        using var connection = new SqlConnection(LegacyConnectionString.Value);

        connection.Execute(query, ids.Select(id => new { id }));
    }
}
