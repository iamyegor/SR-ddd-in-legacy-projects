using ACL.ConnectionStrings;
using ACL.Synchronizers.LegacyDeliveries.Models;
using ACL.Utils;
using Dapper;
using Npgsql;

namespace ACL.Synchronizers.LegacyDeliveries.Repositories;

public class BubbleDeliveryRepository
{
    private readonly PostgreSqlGenerator _sqlGenerator;

    public BubbleDeliveryRepository(PostgreSqlGenerator sqlGenerator)
    {
        _sqlGenerator = sqlGenerator;
    }

    public void Save(List<DeliveryInBubble> deliveries)
    {
        string query = _sqlGenerator.InsertOrUpdate<DeliveryInBubble>("deliveries", "id");

        using var connection = new NpgsqlConnection(BubbleConnectionString.Value);
        connection.Execute(query, deliveries);
    }
}