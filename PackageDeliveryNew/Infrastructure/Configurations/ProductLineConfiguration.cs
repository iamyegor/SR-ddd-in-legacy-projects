using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PackageDeliveryNew.Deliveries;

namespace PackageDeliveryNew.Infrastructure.Configurations;

public class ProductLineConfiguration : IEntityTypeConfiguration<ProductLine>
{
    public void Configure(EntityTypeBuilder<ProductLine> builder)
    {
        builder.HasKey(pl => pl.Id);
        builder.Property(pl => pl.Id).HasColumnName("id");
        builder.Property(pl => pl.Amount).HasColumnName("amount");
        builder.HasOne(pl => pl.Product).WithMany().HasForeignKey("product_id");
        builder.Property(pl => pl.IsDeleted).HasColumnName("is_deleted");

        builder.ToTable("product_lines");
    }
}
