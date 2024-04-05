using ACL.Synchronizers.Delivery.Models;
using AutoMapper;

namespace ACL.MappingProfiles;

public class DeliveryProfile : Profile
{
    public DeliveryProfile()
    {
        CreateMap<DeliveryInLegacy, DeliveryInBubble>()
            .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.NMB_CLM))
            .ForMember(
                dest => dest.DestinationStreet,
                opts => opts.MapFrom(src => (src.STR ?? "").Trim())
            )
            .ForMember(
                dest => dest.DestinationZipCode,
                opts => opts.MapFrom(src => (src.ZP ?? "").Trim())
            )
            .AfterMap(
                (src, dest) =>
                {
                    if (string.IsNullOrWhiteSpace(src.CT_ST) || src.CT_ST.IndexOf(' ') == -1)
                    {
                        throw new Exception(
                            $"Delivery with id {src.NMB_CLM} has invalid city and state"
                        );
                    }

                    string[] cityAndState = src.CT_ST.Split(' ', 2);

                    dest.DestinationCity = cityAndState[0];
                    dest.DestinationState = cityAndState[1];
                }
            );

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
