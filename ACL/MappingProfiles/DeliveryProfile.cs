using ACL.Synchronizers.Delivery.Models;
using AutoMapper;

namespace ACL.MappingProfiles;

public class DeliveryProfile : Profile
{
    public DeliveryProfile()
    {
        CreateMap<DeliveryInBubble, DeliveryInLegacy>()
            .ForMember(dest => dest.NMB_CLM, opts => opts.MapFrom(src => src.Id))
            .ForMember(dest => dest.ESTM_CLM, opts => opts.MapFrom(src => src.CostEstimate))
            .AfterMap(
                (src, dest) =>
                {
                    dest.PRD_LN_1 = null;
                    dest.PRD_LN_1_AMN = null;

                    dest.PRD_LN_2 = null;
                    dest.PRD_LN_2_AMN = null;

                    dest.PRD_LN_3 = null;
                    dest.PRD_LN_3_AMN = null;

                    dest.PRD_LN_4 = null;
                    dest.PRD_LN_4_AMN = null;

                    List<ProductLineInBubble> notDeletedProductLines = src
                        .ProductLines.Where(pl => !pl.IsDeleted)
                        .ToList();
                    
                    if (notDeletedProductLines.Count > 0)
                    {
                        var line = notDeletedProductLines[0];
                        dest.PRD_LN_1 = line.ProductId;
                        dest.PRD_LN_1_AMN = line.Amount;
                    }

                    if (notDeletedProductLines.Count > 1)
                    {
                        var line = notDeletedProductLines[1];
                        dest.PRD_LN_2 = line.ProductId;
                        dest.PRD_LN_2_AMN = line.Amount;
                    }

                    if (notDeletedProductLines.Count > 2)
                    {
                        var line = notDeletedProductLines[2];
                        dest.PRD_LN_3 = line.ProductId;
                        dest.PRD_LN_3_AMN = line.Amount;
                    }

                    if (notDeletedProductLines.Count > 3)
                    {
                        var line = notDeletedProductLines[3];
                        dest.PRD_LN_4 = line.ProductId;
                        dest.PRD_LN_4_AMN = line.Amount;
                    }
                }
            );
    }
}
