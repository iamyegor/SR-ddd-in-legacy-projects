using ACL.Synchronizers.Delivery.Models;
using Dapper;
using Npgsql;

namespace ACL.Synchronizers.Delivery;

public class BubbleProductLineRepository
{
    private const string TempTable = "product_lines_to_sync";
    private readonly NpgsqlConnection _connection;
    private readonly NpgsqlTransaction _transaction;

    public BubbleProductLineRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public List<ProductLineInBubble> GetAllThatNeedSync()
    {
        string query =
            @$"
            select 
                product_id as ProductId,
                amount as Amount,
                delivery_id as DeliveryId,
                is_deleted as IsDeleted
            into temp {TempTable}
            from product_lines
            where delivery_id in (select id from deliveries where is_sync_needed = true)

            select *
            from {TempTable}";

        return _connection.Query<ProductLineInBubble>(query, transaction: _transaction).ToList();
    }

    public void DeleteQueriedThatNeedDelete()
    {
        string query =
            @$"
            delete from product_lines
            where is_deleted = true and id in (select id from {TempTable})";

        _connection.Execute(query, transaction: _transaction);
    }
}
