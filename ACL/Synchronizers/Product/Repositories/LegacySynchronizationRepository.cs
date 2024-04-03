using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ACL.Synchronizers.Product;

internal class LegacySynchronizationRepository
{
    private readonly SqlConnection _connection;
    private readonly SqlTransaction _transaction;

    public LegacySynchronizationRepository(SqlConnection connection, SqlTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public byte[] GetRowVersionFor(string name)
    {
        string query =
            @"
            select RowVersion from Synchronization 
            where name = @name";

        return _connection.QuerySingle<byte[]>(query, new { name }, transaction: _transaction);
    }

    public void SetSyncFlagFalseFor(string name, byte[] rowVersion)
    {
        string query =
            @"
            update Synchronization
            set IsSyncRequired
            where Name=@name and RowVersion=@rowVersion";

        int rowsAffected = _connection.Execute(
            query,
            new { name, rowVersion },
            transaction: _transaction
        );

        if (rowsAffected == 0)
        {
            throw new DBConcurrencyException($"Conflict in {name} row in Synchronization table");
        }
    }
}
