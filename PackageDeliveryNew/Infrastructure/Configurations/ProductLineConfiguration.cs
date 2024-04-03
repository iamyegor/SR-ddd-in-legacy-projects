// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
// using PackageDeliveryNew.Deliveries;
//
// namespace PackageDeliveryNew.Infrastructure.Configurations;
//
// public class ProductLineConfiguration : IEntityTypeConfiguration<ProductLine>
// {
//     public void Configure(EntityTypeBuilder<ProductLine> builder)
//     {
//         builder.HasKey("id");
//         builder.Property<int>("id");
//         builder.Property(pl => pl.Amount).HasColumnName("amount");
//         builder.HasOne(pl => pl.Product).WithMany().HasForeignKey("product_id");
//
//         builder.ToTable("product_lines");
//     }
// }
