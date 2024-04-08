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

    public void Save(IEnumerable<object> objectsToSave, string type)
    {
        var jsonListToSave = objectsToSave.Select(obj => new
        {
            Content = JsonConvert.SerializeObject(obj)
        });

        string query =
            @$"
            insert into Outbox (Content, Type)
            values (@Content, '{type}')";

        _connection.Execute(query, jsonListToSave, transaction: _transaction);
    }
}
