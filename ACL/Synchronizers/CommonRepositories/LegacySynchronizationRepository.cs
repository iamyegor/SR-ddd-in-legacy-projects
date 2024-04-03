using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace ACL.Synchronizers.CommonRepositories;

internal class LegacySynchronizationRepository
{
    private readonly SqlConnection _connection;
    private readonly SqlTransaction _transaction;

    public LegacySynchronizationRepository(SqlConnection connection, SqlTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public byte[] GetRowVersionFor(string type)
    {
        string syncVersionQuery =
            @"
            select RowVersion
            from [dbo].[Synchronization] 
            where Name = @type";

        return _connection.QuerySingle<byte[]>(
            syncVersionQuery,
            new { type },
            transaction: _transaction
        );
    }

    public void SetSyncFlagFalse(string type, byte[] syncVersion)
    {
        string removeIsSyncRequiredFlagQuery =
            @"
            update [dbo].[Synchronization]
            set IsSyncRequired = 0
            where Name = @type and RowVersion = @syncVersion;";

        int rowsAffected = _connection.Execute(
            removeIsSyncRequiredFlagQuery,
            new { type, syncVersion },
            transaction: _transaction
        );

        if (rowsAffected == 0)
        {
            throw new DBConcurrencyException("Synchronization table conflict");
        }
    }
}
