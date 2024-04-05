using ACL.Synchronizers.Product.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ACL.Synchronizers.Product.Repositories;

public class LegacyProductRepository
{
    private const string TempTable = "#products_to_sync";
    private readonly SqlConnection _connection;
    private readonly SqlTransaction _transaction;

    public LegacyProductRepository(SqlConnection connection, SqlTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public List<ProductInLegacy> GetAllThatNeedSync()
    {
        string query =
            @$"select 
                NMB_CM, NM_CLM, WT, WT_KG 
            into {TempTable} 
            from PRD_TBL with (updlock)
            where IsSyncNeeded = 1
            
            select *
            from {TempTable}";

        return _connection.Query<ProductInLegacy>(query, transaction: _transaction).ToList();
    }

    public void SetSyncFlagFalseForQueriedProducts()
    {
        string query =
            @$"
            update p
            set p.IsSyncNeeded = 1
            from PRD_TBL p
            inner join {TempTable} t on t.NMB_CM = p.NMB_CM";

        _connection.Execute(query, transaction: _transaction);
    }
}
