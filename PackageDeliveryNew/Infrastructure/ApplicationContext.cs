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

    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Synchronization> Synchronization => Set<Synchronization>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Username=postgres;Password=yegor;Database=sr_package_delivery_new"
        ).EnableSensitiveDataLogging();

        optionsBuilder.AddInterceptors(new SyncInterceptor());
        optionsBuilder.AddInterceptors(new SoftDeleteInterceptor());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetAssembly(typeof(IEntityConfigurationAssembly))!
        );
    }
}
