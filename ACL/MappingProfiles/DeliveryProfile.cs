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
                    if (src.ProductLines.Count > 0)
                    {
                        var line = src.ProductLines[0];
                        dest.PRD_LN_1 = line.ProductId;
                        dest.PRD_LN_1_AMN = line.Amount;
                    }

                    if (src.ProductLines.Count > 1)
                    {
                        var line = src.ProductLines[1];
                        dest.PRD_LN_2 = line.ProductId;
                        dest.PRD_LN_2_AMN = line.Amount;
                    }

                    if (src.ProductLines.Count > 2)
                    {
                        var line = src.ProductLines[2];
                        dest.PRD_LN_3 = line.ProductId;
                        dest.PRD_LN_3_AMN = line.Amount;
                    }

                    if (src.ProductLines.Count > 3)
                    {
                        var line = src.ProductLines[3];
                        dest.PRD_LN_4 = line.ProductId;
                        dest.PRD_LN_4_AMN = line.Amount;
                    }
                }
            );
    }
}
