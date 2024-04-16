using ACL.Synchronizers.Delivery.Models;
using Mapster;

namespace ACL.Mappings;

public class DeliveryMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<DeliveryInBubble, DeliveryInLegacy>()
            .Map(dest => dest.NMB_CLM, src => src.Id)
            .Map(dest => dest.ESTM_CLM, src => src.CostEstimate)
            .AfterMapping(
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
