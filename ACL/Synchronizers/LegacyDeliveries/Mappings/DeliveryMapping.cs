using ACL.Synchronizers.LegacyDeliveries.Models;
using Mapster;

namespace ACL.Synchronizers.LegacyDeliveries.Mappings;

public class DeliveryMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<DeliveryInLegacy, DeliveryInBubble>()
            .Map(dest => dest.Id, src => src.NMB_CLM)
            .Map(dest => dest.DestinationStreet, src => (src.STR ?? "").Trim())
            .Map(dest => dest.DestinationZipCode, src => (src.ZP ?? "").Trim())
            .AfterMapping(
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
    }
}
