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
            values (@Content::jsonb, '{type}')";

        _connection.Execute(query, deliveriesAsJson, transaction: _transaction);
    }
}
