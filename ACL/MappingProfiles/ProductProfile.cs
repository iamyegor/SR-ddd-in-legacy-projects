using System.ComponentModel.Design;
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
                    if (src.WT == null || src.WT_KG == null)
                    {
                        throw new Exception("Weight in legacy database is invalid");
                    }

                    if (src.WT != null)
                    {
                        dest.WeightInPounds = src.WT.Value;
                    }

                    if (src.WT_KG != null)
                    {
                        double poundsInKillogram = 2.20462;
                        dest.WeightInPounds = src.WT_KG.Value * poundsInKillogram;
                    }
                }
            );
    }
}
