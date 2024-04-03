using System.Text.Json.Serialization;
using ACL.ConnectionStrings;
using ACL.Synchronizers.Product.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace ACL.Synchronizers.Product.Repositories;

public class LegacyOutboxRepository
{
    private readonly SqlConnection _connection;
    private readonly SqlTransaction _transaction;

    public LegacyOutboxRepository(SqlConnection connection, SqlTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public void Save(List<ProductInBubble> productsToSave, string type)
    {
        var productsAsJson = productsToSave.Select(p => new
        {
            Content = JsonConvert.SerializeObject(p)
        });

        string query =
            @$"
            insert into Outbox (Content, Type)
            values (@Content, '{type}')";

        _connection.Execute(query, productsAsJson, transaction: _transaction);
    }

    public (List<int> ids, List<T>) Get<T>(string product)
    {
        using var connection = new SqlConnection(LegacyConnectionString.Value);

        string query =
            @"
            select Id, Content
            from Outbox
            where Type = @product";

        List<OutboxRow> outboxRows = connection
            .Query<OutboxRow>(query, new { product }, transaction: _transaction)
            .ToList();

        List<T> objectsToReturn = [];
        foreach (var json in outboxRows.Select(r => r.Content))
        {
            T? deserializedObject = JsonConvert.DeserializeObject<T>(json);

            if (deserializedObject == null)
            {
                throw new Exception("Couldn't deserialize outbox element");
            }

            objectsToReturn.Add(deserializedObject);
        }

        List<int> ids = outboxRows.Select(r => r.Id).ToList();
        return (ids, objectsToReturn);
    }

    public void Remove(List<int> ids)
    {
        using var connection = new SqlConnection(LegacyConnectionString.Value);
        
        string query =
            @"
            delete from Outbox
            where Id = @id";

        connection.Execute(query, ids.Select(id => new { id }));
    }
}
