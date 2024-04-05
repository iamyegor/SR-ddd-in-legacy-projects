using ACL.Synchronizers.Delivery.Models;
using Dapper;
using Npgsql;

namespace ACL.Synchronizers.Delivery;

public class BubbleDeliveryRepository
{
    private const string TempTable = "deliveries_to_sync";
    private readonly NpgsqlConnection _connection;
    private readonly NpgsqlTransaction _transaction;

    public BubbleDeliveryRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public List<DeliveryInBubble> GetAllThatNeedSync()
    {
        string query =
            @$"
            select id as Id, cost_estimate as Id
            into temp {TempTable}
            from deliveries
            where is_sync_needed = true
            for update;

            select *
            from {TempTable}";

        return _connection.Query<DeliveryInBubble>(query, transaction: _transaction).ToList();
    }

    public void SetSyncFlagsFalseForQueried()
    {
        string query =
            @$"
            update deliveries
            set is_sync_needed = false
            where id in (select id from {TempTable})";

        _connection.Execute(query, transaction: _transaction);
    }
}
