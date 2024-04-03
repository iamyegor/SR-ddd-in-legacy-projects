using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PackageDeliveryNew.Infrastructure.Configurations;

public class SynchronizationConfiguration : IEntityTypeConfiguration<Synchronization>
{
    public void Configure(EntityTypeBuilder<Synchronization> builder)
    {
        builder.ToTable("sync").HasKey(s => s.Name);
        builder.Property(s => s.Name).HasColumnName("name").HasMaxLength(100);
        builder
            .Property(s => s.IsSyncRequired)
            .HasDefaultValue(false)
            .HasColumnName("is_sync_required");
        builder.Property(s => s.RowVersion).HasDefaultValue(1).HasColumnName("row_version");
    }
}
