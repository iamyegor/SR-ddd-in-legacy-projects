using ACL.ConnectionStrings;
using ACL.Synchronizers.Product.Models;
using ACL.Synchronizers.Product.Repositories;
using AutoMapper;
using Microsoft.Data.SqlClient;

namespace ACL.Synchronizers.Product;

public class LegacyProductSynchronizer
{
    private readonly IMapper _mapper;

    public LegacyProductSynchronizer(IMapper mapper)
    {
        _mapper = mapper;
    }

    public void Sync()
    {
        if (!IsSyncNeeded())
        {
            return;
        }

        using var connection = new SqlConnection(LegacyConnectionString.Value);
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            List<ProductInLegacy> productsFromLegacy = GetUpdatedProductsFromLegacy(
                connection,
                transaction
            );

            List<ProductInBubble> mappedProducts = _mapper.Map<List<ProductInBubble>>(
                productsFromLegacy
            );

            var outboxRepository = new LegacyOutboxRepository(connection, transaction);
            outboxRepository.Save(mappedProducts, "Product");

            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
        }
    }

    private bool IsSyncNeeded()
    {
        throw new NotImplementedException();
    }

    private List<ProductInLegacy> GetUpdatedProductsFromLegacy(
        SqlConnection connection,
        SqlTransaction transaction
    )
    {
        throw new NotImplementedException();
    }
}
