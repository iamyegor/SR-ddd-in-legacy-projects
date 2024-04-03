using System.Data.SqlClient;
using ACL.Synchronizers.Delivery.Models;
using ACL.Utils;
using Dapper;
using Npgsql;

namespace ACL.Synchronizers.Delivery.Repositories;

public class LegacyDeliveryRepository
{
    private const string TempTable = "#deliveries_to_sync";
    private readonly SqlConnection _connection;
    private readonly SqlTransaction _transaction;

    public LegacyDeliveryRepository(SqlConnection connection, SqlTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public LegacyDeliveryRepository() { }

    public List<DeliveryInLegacy> GetAllThatNeedSync()
    {
        string query =
            @$"
            select d.NMB_CLM, a.STR, a.CT_ST, a.ZP 
            into {TempTable}
            from DLVR_TBL d
            inner join ADDR_TBL a
            on d.NMB_CLM = a.ID_CLM

            select *
            from {TempTable}";

        return _connection.Query<DeliveryInLegacy>(query, transaction: _transaction).ToList();
    }

    public void SetSyncFlagFalseForQueriedDeliveries()
    {
        string query =
            @$"
            update d
            set d.IsSyncRequired = 0
            from [dbo].[DLVR_TBL] d
            inner join {TempTable} t on t.NMB_CLM = d.NMB_CLM";

        _connection.Execute(query, transaction: _transaction);
    }

    public void Save(List<DeliveryInLegacy> deliveriesToSave)
    {
        string query =
            @"
            update [dbo].[DLVR_TBL]
            set 
                ESTM_CLM = @ESTM_CLM,
                PRD_LN_1 = @PRD_LN_1,
                PRD_LN_1_AMN = @PRD_LN_1_AMN,
                PRD_LN_2 = @PRD_LN_2,
                PRD_LN_2_AMN = @PRD_LN_2_AMN
            where NMB_CLM = @NMB_CLM;

            if exists (select 1 from [dbo].[DLVR_TBL2] where NMB_CLM = @NMB_CLM)
            begin
                update [dbo].[DLVR_TBL2] 
                set
                    PRD_LN_3 = @PRD_LN_3,
                    PRD_LN_3_AMN =@PRD_LN_3_AMN,
                    PRD_LN_4 = @PRD_LN_4,
                    PRD_LN_4_AMN = @PRD_LN_4_AMN
                where NMB_CLM = @NMB_CLM
            end
            else
            begin
                insert into [dbo].[DLVR_TBL2] (NMB_CLM, PRD_LN_3, PRD_LN_3_AMN, PRD_LN_4, PRD_LN_4_AMN)
                values (@NMB_CLM, @PRD_LN_3, @PRD_LN_3_AMN, @PRD_LN_4, @PRD_LN_4_AMN)
            end";

        using var connection = new SqlConnection(LegacyConnectionString.Value);

        connection.Execute(query, deliveriesToSave);
    }
}
