using ACL.ConnectionStrings;
using ACL.Synchronizers.Delivery.Models;
using Dapper;
using Npgsql;

namespace ACL.Synchronizers.Delivery.Repositories;

public class BubbleDeliveryRepository
{
    private const string TempTable = "deliveries_to_sync";
    private readonly NpgsqlConnection? _connection;
    private readonly NpgsqlTransaction? _transaction;

    public BubbleDeliveryRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public BubbleDeliveryRepository() { }

    public List<DeliveryInBubble> GetAllThatNeedSync()
    {
        ArgumentNullException.ThrowIfNull(_connection);
        ArgumentNullException.ThrowIfNull(_transaction);

        string query =
            @$"
            select id as Id, cost_estimate as CostEstimate
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
        ArgumentNullException.ThrowIfNull(_connection);
        ArgumentNullException.ThrowIfNull(_transaction);
        
        string query =
            @$"
            update deliveries
            set is_sync_needed = false
            where id in (select id from {TempTable})";

        _connection.Execute(query, transaction: _transaction);
    }

    public void Save(List<DeliveryInBubble> deliveriesFromOutbox)
    {
        using var connection = new NpgsqlConnection(BubbleConnectionString.Value);
        string query =
            @"
            insert into deliveries (
                id, 
                destination_city, 
                destination_state, 
                destination_street, 
                destination_zip_code
            ) 
            values (
                @Id, 
                @DestinationCity, 
                @DestinationState, 
                @DestinationStreet, 
                @DestinationZipCode
            )
            on conflict (id)
            do update set
                id = excluded.id,
                destination_city = excluded.destination_city,
                destination_state = excluded.destination_state,
                destination_street = excluded.destination_street,
                destination_zip_code = excluded.destination_zip_code";

        connection.Execute(query, deliveriesFromOutbox);
    }
}
