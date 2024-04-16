using System.Data;
using Dapper;
using Npgsql;

namespace ACL.Synchronizers.CommonRepositories.Synchronization;

public class BubbleSynchronizationRepository
{
    public int GetRowVersionFor(string rowName, NpgsqlTransaction transaction)
    {
        string query = "select row_version from sync where name = @name";

        NpgsqlConnection connection = transaction.Connection!;
        return connection.QuerySingle<int>(query, new { rowName }, transaction: transaction);
    }

    public void SetSyncFlagFalse(string rowName, int rowVersion, NpgsqlTransaction transaction)
    {
        string query =
            @"
            update sync
            set is_sync_required = 0
            where name=@name and row_version=@rowVersion";

        NpgsqlConnection connection = transaction.Connection!;
        int rowsAffected = connection.Execute(
            query,
            new { rowName, rowVersion },
            transaction: transaction
        );

        if (rowsAffected == 0)
        {
            throw new DBConcurrencyException($"Conflict in {rowName} row in Synchronization table");
        }
    }
}