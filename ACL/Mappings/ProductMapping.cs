using ACL.Synchronizers.Product.Models;
using Mapster;

namespace ACL.Mappings;

public class ProductMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<ProductInLegacy, ProductInBubble>()
            .Map(dest => dest.Id, src => src.NMB_CM)
            .Map(dest => dest.Name, src => (src.NM_CLM ?? "").Trim())
            .AfterMapping(
                (src, dest) =>
                {
                    if (src.WT == null && src.WT_KG == null)
                    {
                        throw new Exception("Weight in legacy database is invalid");
                    }

                    if (src.WT != null)
                    {
                        dest.WeightInPounds = src.WT.Value;
                    }

                    if (src.WT_KG != null)
                    {
                        double poundsInKilogram = 2.20462;
                        dest.WeightInPounds = src.WT_KG.Value * poundsInKilogram;
                    }
                }
            );
    }
}
