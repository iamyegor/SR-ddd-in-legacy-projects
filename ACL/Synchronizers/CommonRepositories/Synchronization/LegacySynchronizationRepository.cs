using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace ACL.Synchronizers.CommonRepositories.Synchronization;

public class LegacySynchronizationRepository
{
    public byte[] GetRowVersionFor(string name, SqlTransaction transaction)
    {
        string query = "select row_version from sync where name = @name";

        SqlConnection connection = transaction.Connection!;
        return connection.QuerySingle<byte[]>(query, new { name }, transaction: transaction);
    }

    public void SetSyncFlagsFalseFor(string name, byte[] rowVersion, SqlTransaction transaction)
    {
        string query =
            @"
            update sync
            set is_sync_required = 0
            where name=@name and row_version=@rowVersion";

        SqlConnection connection = transaction.Connection!;
        int rowsAffected = connection.Execute(
            query,
            new { name, rowVersion },
            transaction: transaction
        );

        if (rowsAffected == 0)
        {
            throw new DBConcurrencyException($"Conflict in {name} row in Synchronization table");
        }
    }
}
