using System.Data;
using Dapper;
using Npgsql;

namespace ACL.Synchronizers.Delivery;

public class BubbleSynchronizationRepository
{
    private readonly NpgsqlConnection _connection;
    private readonly NpgsqlTransaction _transaction;

    public BubbleSynchronizationRepository(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction
    )
    {
        _connection = connection;
        _transaction = transaction;
    }

    public int GetRowVersionFor(string rowName)
    {
        string query =
            @"
            select row_version
            from sync
            where name = @rowName";

        return _connection.QuerySingle<int>(query, new { rowName }, transaction: _transaction);
    }

    public void SetSyncFlagFalse(string rowName, int rowVersion)
    {
        string query =
            @"
            update sync
            set is_sync_required = false
            where name = @rowName and row_version = @rowVersion";

        int rowsAffected = _connection.Execute(
            query,
            new { rowName, rowVersion },
            transaction: _transaction
        );

        if (rowsAffected == 0)
        {
            throw new DBConcurrencyException($"row_version conflict in {rowName} row in sync table");
        }
    }
}
