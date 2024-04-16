using ACL.ConnectionStrings;
using ACL.Synchronizers.Product.Models;
using ACL.Utils;
using Dapper;
using Npgsql;

namespace ACL.Synchronizers.Product.Repositories;

public class BubbleProductRepository
{
    private readonly PostgreSqlGenerator _sqlGenerator;

    public BubbleProductRepository(PostgreSqlGenerator sqlGenerator)
    {
        _sqlGenerator = sqlGenerator;
    }

    public void Save(List<ProductInBubble> products)
    {
        string query = _sqlGenerator.InsertOrUpdate<ProductInBubble>("products", "id");

        using var context = new NpgsqlConnection(BubbleConnectionString.Value);
        context.Execute(query, products);
    }
}
