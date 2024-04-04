using ACL.ConnectionStrings;
using ACL.Synchronizers.Delivery.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ACL.Synchronizers.Delivery.Repositories;

public class LegacyDeliveryRepository
{
    public void Save(List<DeliveryInLegacy> deliveries)
    {
        using var connection = new SqlConnection(LegacyConnectionString.Value);

        string query =
            @"
            update DLVR_TBL
            set ESTM_CLM = @ESTM_CLM,
                PRD_LN_1 = @PRD_LN_1,
                PRD_LN_1_AMN = @PRD_LN_1_AMN,
                PRD_LN_2 = @PRD_LN_2,
                PRD_LN_2_AMN = @PRD_LN_2_AMN
            where NMB_CLM = @NMB_CLM

            if exists (select 1 from DLVR_TBL2 where NMB_CLM = @NMB_CLM)
            begin
                update DLVR_TBL2
                set PRD_LN_3 = @PRD_LN_3,
                    PRD_LN_3_AMN = @PRD_LN_3_AMN,
                    PRD_LN_4 = @PRD_LN_4,
                    PRD_LN_4_AMN = @PRD_LN_4_AMN
                where NMB_CLM = @NMB_CLM
            end
            else
            begin
                insert into DLVR_TBL2 (NMB_CLM, PRD_LN_3, PRD_LN_3_AMN, PRD_LN_4, PRD_LN_4_AMN) 
                values (@NMB_CLM, @PRD_LN_3, @PRD_LN_3_AMN, @PRD_LN_4, @PRD_LN_4_AMN)
            end";

        connection.Execute(query, deliveries);
    }
}
