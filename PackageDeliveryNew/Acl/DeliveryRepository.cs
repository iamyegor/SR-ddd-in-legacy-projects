using Dapper;
using Microsoft.Data.SqlClient;
using PackageDeliveryNew.Deliveries;
using PackageDeliveryNew.Utils;

namespace PackageDeliveryNew.Acl;

public class DeliveryRepository
{
    public Delivery? GetById(int id)
    {
        DeliveryFromLegacy? deliveryFromLegacy = GetDeliveryFromLegacyById(id);
        if (deliveryFromLegacy != null)
        {
            return MapDeliveryFromLegacyToNewDelivery(deliveryFromLegacy);
        }

        return null;
    }

    private Delivery MapDeliveryFromLegacyToNewDelivery(DeliveryFromLegacy deliveryFromLegacy)
    {
        if (deliveryFromLegacy.CT_ST == null || !deliveryFromLegacy.CT_ST.Contains(' '))
        {
            throw new Exception("Invalid city and state");
        }

        string[] cityAndState = deliveryFromLegacy.CT_ST.Split(' ');

        Address address = new Address(
            street: (deliveryFromLegacy.STR ?? "").Trim(),
            city: cityAndState[0],
            state: cityAndState[1],
            zipCode: (deliveryFromLegacy.ZP ?? "").Trim()
        );

        return new Delivery(deliveryFromLegacy.NMB_CLM, address);
    }

    private DeliveryFromLegacy? GetDeliveryFromLegacyById(int id)
    {
        using (var connection = new SqlConnection(ConnectionString.Value))
        {
            string query =
                @"
                SELECT d.NMB_CLM, a.*
                FROM [dbo].[DLVR_TBL] d
                INNER JOIN [dbo].[ADDR_TBL] a ON a.DLVR = d.NMB_CLM
                WHERE d.NMB_CLM = @Id
                ";

            return connection.Query<DeliveryFromLegacy>(query, new { Id = id }).SingleOrDefault();
        }
    }
}

public class DeliveryFromLegacy
{
    public int NMB_CLM { get; set; }
    public string? STR { get; set; }
    public string? CT_ST { get; set; }
    public string? ZP { get; set; }
}
