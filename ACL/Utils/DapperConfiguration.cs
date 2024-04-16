using System.Reflection;
using ACL.Extensions;
using ACL.Synchronizers.Product.Models;
using Dapper;
using LegacyDeliveries = ACL.Synchronizers.LegacyDeliveries.Models;

namespace ACL.Utils;

public static class DapperConfiguration
{
    public static void ConfigureSnakeCaseMapping()
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();

        foreach (var modelType in types)
        {
            if (modelType == typeof(ProductInLegacy))
            {
                continue;
            }

            if (modelType == typeof(LegacyDeliveries.DeliveryInLegacy))
            {
                continue;
            }

            SqlMapper.SetTypeMap(
                modelType,
                new CustomPropertyTypeMap(
                    modelType,
                    (type, columnName) => // columns are expected to be in snake case
                        type.GetProperties()
                            .FirstOrDefault(prop => prop.Name.ToSnakeCase() == columnName)!
                )
            );
        }
    }
}
