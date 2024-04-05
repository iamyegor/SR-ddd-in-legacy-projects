using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PackageDeliveryNew.Deliveries;

namespace PackageDeliveryNew.Infrastructure.Configurations;

public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedNever().HasColumnName("id");
        builder.ComplexProperty(
            d => d.Destination,
            propertyBuilder =>
            {
                propertyBuilder.Property(d => d.State).HasColumnName("destination_state");
                propertyBuilder.Property(d => d.Street).HasColumnName("destination_street");
                propertyBuilder.Property(d => d.City).HasColumnName("destination_city");
                propertyBuilder.Property(d => d.ZipCode).HasColumnName("destination_zip_code");
            }
        );
        builder.OwnsMany(
            d => d.ProductLines,
            ownsManyBuilder =>
            {
                ownsManyBuilder.ToTable("product_lines").HasKey(pl => pl.Id);
                ownsManyBuilder.Property(pl => pl.Id).HasColumnName("id");
                ownsManyBuilder.Property(pl => pl.Amount).HasColumnName("amount");
                ownsManyBuilder.HasOne(pl => pl.Product).WithMany().HasForeignKey("product_id");
                ownsManyBuilder.WithOwner().HasForeignKey("delivery_id");
                ownsManyBuilder.Navigation(pl => pl.Product).AutoInclude();
            }
        );
        builder.Property(d => d.CostEstimate).HasColumnName("cost_estimate");
        builder
            .Property(d => d.IsSyncNeeded)
            .HasDefaultValue(false)
            .HasColumnName("is_sync_needed");

        builder.ToTable("deliveries");
    }
}
