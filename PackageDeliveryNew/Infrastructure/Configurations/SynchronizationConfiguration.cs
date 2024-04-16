using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PackageDeliveryNew.Infrastructure.Configurations;

public class SynchronizationConfiguration : IEntityTypeConfiguration<Synchronization>
{
    public void Configure(EntityTypeBuilder<Synchronization> builder)
    {
        builder.ToTable("sync").HasKey(s => s.Name);

        builder.Property(s => s.Name).HasColumnName("name");
        builder.Property(s => s.RowVersion).HasColumnName("row_version").HasDefaultValue(1);
        
        builder
            .Property(s => s.IsSyncRequired)
            .HasColumnName("is_sync_required")
            .HasDefaultValue(false);
    }
}
