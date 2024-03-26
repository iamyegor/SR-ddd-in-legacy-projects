using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PackageDeliveryNew.Deliveries;
using PackageDeliveryNew.Infrastructure.Configurations;

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
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Username=postgres;Password=yegor;Database=PackageDeliveryNew"
        );

        optionsBuilder.AddInterceptors(new SyncNeededInterceptor());
        optionsBuilder.AddInterceptors(new SoftDeleteInterceptor());
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

    public Synchronization(string name, bool isSyncRequired = false)
    {
        Name = name;
        IsSyncRequired = isSyncRequired;
    }
}
