using ACL.Synchronizers.Product.Models;
using AutoMapper;

namespace ACL.MappingProfiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<ProductInLegacy, ProductInBubble>()
            .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.NMB_CM))
            .ForMember(dest => dest.Name, opts => opts.MapFrom(src => (src.NM_CLM ?? "").Trim()))
            .AfterMap(
                (src, dest) =>
                {
                    if (src.WT == null && src.WT_KG == null)
                    {
                        throw new Exception("Invalid weight in product");
                    }

                    dest.WeightInPounds = src.WT ?? src.WT_KG ?? 0;
                }
            );
    }
}
