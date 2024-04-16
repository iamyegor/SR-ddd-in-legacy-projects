using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PackageDeliveryNew.Deliveries;
using PackageDeliveryNew.Infrastructure.Configurations;
using PackageDeliveryNew.Infrastructure.Interceptors;

namespace PackageDeliveryNew.Infrastructure;

public class ApplicationContext : DbContext
{
    public ApplicationContext() { }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options) { }

    public virtual DbSet<Delivery> Deliveries => Set<Delivery>();
    public virtual DbSet<Product> Products => Set<Product>();
    public virtual DbSet<ProductLine> ProductLines => Set<ProductLine>();
    public DbSet<Synchronization> Sync => Set<Synchronization>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Username=postgres;Password=yegor;Database=sr_package_delivery_new"
        );

        optionsBuilder.AddInterceptors(new SyncInterceptor());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetAssembly(typeof(IEntityConfigurationAssembly))!
        );
    }
}

public class Synchronization
{
    public string Name { get; set; }
    public bool IsSyncRequired { get; set; }
    public int RowVersion { get; set; }
}
