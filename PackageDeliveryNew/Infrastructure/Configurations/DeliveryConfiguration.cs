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
        builder.HasMany(d => d.ProductLines).WithOne().HasForeignKey("delivery_id");

        builder
            .Property(d => d.IsSyncNeeded)
            .HasDefaultValue(false)
            .HasColumnName("is_sync_needed");

        builder.Property(d => d.CostEstimate).HasColumnName("cost_estimate");

        builder.ToTable("deliveries");
    }
}
