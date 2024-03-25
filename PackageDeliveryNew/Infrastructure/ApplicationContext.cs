using Microsoft.EntityFrameworkCore;
using PackageDeliveryNew.Deliveries;

namespace PackageDeliveryNew.Infrastructure;

public class ApplicationContext : DbContext
{
    public ApplicationContext() { }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options) { }

    public virtual DbSet<Delivery> Deliveries => Set<Delivery>();
    public virtual DbSet<Product> Products => Set<Product>();
    public virtual DbSet<ProductLine> ProductLines => Set<ProductLine>();
    public DbSet<Synchronization> Synchronization => Set<Synchronization>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            "Server=localhost;Database=PDTest;User Id=packageDelivery;Trusted_Connection=True;TrustServerCertificate=True;"
        );

        optionsBuilder.AddInterceptors(new MarkIsSyncRequiredInterceptor());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Delivery>(builder =>
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).ValueGeneratedNever();
            builder.ComplexProperty(d => d.Destination);
            builder.OwnsMany(
                d => d.ProductLines,
                ownsManyBuilder =>
                {
                    ownsManyBuilder.HasKey("Id");
                    ownsManyBuilder.Property<int>("Id");
                    ownsManyBuilder.ToTable("Delivery_ProductLines");
                }
            );

            builder.Property(d => d.IsSyncNeeded).HasDefaultValue(false);
            builder.ToTable("Deliveries");
        });

        modelBuilder.Entity<Product>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Synchronization>(builder =>
        {
            builder.HasKey(s => s.Name);

            builder.HasData(new Synchronization("Delivery"));
        });
    }
}

public class Synchronization
{
    public string Name { get; set; }
    public bool IsSyncRequired { get; set; }

    public Synchronization(string name, bool isSyncRequired = false)
    {
        Name = name;
        IsSyncRequired = isSyncRequired;
    }
}
