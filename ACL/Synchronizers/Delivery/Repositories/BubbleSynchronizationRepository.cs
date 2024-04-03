using System.Data;
using Dapper;
using Npgsql;

namespace ACL.Synchronizers.Delivery.Repositories;

internal class BubbleSynchronizationRepository
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

    public void SetSyncFlagFalse(string rowName, int syncRowVersion)
    {
        string query =
            @"
            update sync
            set is_sync_required = false
            where name = @rowName and row_version = @syncRowVersion";

        int rowsAffected = _connection.Execute(
            query,
            new { rowName, syncRowVersion },
            transaction: _transaction
        );

        if (rowsAffected == 0)
        {
            throw new DBConcurrencyException("Sync row version conflict");
        }
    }
}
