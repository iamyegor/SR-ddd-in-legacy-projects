using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PackageDeliveryNew.Infrastructure.Configurations;

public class SynchronizationConfiguration : IEntityTypeConfiguration<Synchronization>
{
    public void Configure(EntityTypeBuilder<Synchronization> builder)
    {
        builder.HasKey(s => s.Name);

        builder.HasData(new Synchronization("Delivery"));
        builder.Property(s => s.IsSyncRequired).HasColumnName("is_sync_required");
        builder.Property(s => s.Name).HasColumnName("name");
        builder.ToTable("sync");
    }
}
