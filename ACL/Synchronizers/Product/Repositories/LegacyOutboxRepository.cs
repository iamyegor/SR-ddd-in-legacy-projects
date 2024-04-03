using System.Text.Json.Serialization;
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
        throw new NotImplementedException();
    }

    public void Remove(List<int> ids)
    {
        throw new NotImplementedException();
    }
}
