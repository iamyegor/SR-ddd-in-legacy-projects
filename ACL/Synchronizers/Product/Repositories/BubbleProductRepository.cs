using ACL.Synchronizers.Product.Models;
using ACL.Utils;
using Dapper;
using Npgsql;

namespace ACL.Synchronizers.Product.Repositories;

public class BubbleProductRepository
{
    public void Save(List<ProductInBubble> products)
    {
        using var connection = new NpgsqlConnection(BubbleConnectionString.Value);
        
        string query =
            @"        
            insert into products (id, name, weight_in_pounds)
            values (@Id, @Name, @WeightInPounds)
            on conflict (id)
            do update set
                id = excluded.id,
                name = excluded.name,
                weight_in_pounds = excluded.weight_in_pounds";

        connection.Execute(query, products);
    }
}
