using ACL.ConnectionStrings;
using ACL.Synchronizers.Outbox.Models;
using ACL.Synchronizers.Product.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Npgsql;

namespace ACL.Synchronizers.Outbox;

public class LegacyOutboxProductSynchronizer
{
    public void Sync()
    {
        (List<int> outboxIds, List<ProductInBubble> productsFromOutbox) = GetProductsFromOutBox();
        if (outboxIds.Count != 0)
        {
            SaveProductsInBubble(productsFromOutbox);
            RemoveProductsFromOutbox(outboxIds);
        }
    }

    private (List<int> ids, List<ProductInBubble> productsFromOutbox) GetProductsFromOutBox()
    {
        string query =
            @"
            SELECT TOP 20 id as Id, content as Content
            FROM outbox
            WHERE type = 'Product'";

        List<OutboxModel> idsAndProducts;
        using (var connection = new SqlConnection(LegacyConnectionString.Value))
        {
            idsAndProducts = connection.Query<OutboxModel>(query).ToList();
        }

        List<ProductInBubble> productsToReturn = [];
        foreach (var productJson in idsAndProducts.Select(x => x.Content))
        {
            ProductInBubble? product = JsonConvert.DeserializeObject<ProductInBubble>(productJson);

            if (product == null)
            {
                throw new Exception("Product is null");
            }

            productsToReturn.Add(product);
        }

        List<int> ids = idsAndProducts.Select(x => x.Id).ToList();

        return (ids, productsToReturn);
    }

    private void SaveProductsInBubble(List<ProductInBubble> productsToSave)
    {
        string query =
            @"
            INSERT INTO products (id, weight_in_pounds, name)
            VALUES (@ProductId, @WeightInPounds, @Name)
            ON CONFLICT (id)
            DO UPDATE SET
                id = excluded.id,
                weight_in_pounds = excluded.weight_in_pounds,
                name = excluded.name";

        using (var connection = new NpgsqlConnection(BubbleConnectionString.Value))
        {
            connection.Execute(query, productsToSave);
        }
    }

    private void RemoveProductsFromOutbox(List<int> ids)
    {
        string query =
            @"
            DELETE FROM outbox
            WHERE id = @Id";

        using (var connection = new SqlConnection(LegacyConnectionString.Value))
        {
            connection.Execute(query, ids.Select(id => new { Id = id }));
        }
    }
}
