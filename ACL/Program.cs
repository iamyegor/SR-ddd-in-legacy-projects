using System.Reflection;
using ACL.ConnectionStrings;
using ACL.Synchronizers.BubbleDeliveries;
using ACL.Synchronizers.CommonRepositories;
using ACL.Synchronizers.CommonRepositories.Outbox;
using ACL.Synchronizers.CommonRepositories.Synchronization;
using ACL.Synchronizers.LegacyDeliveries;
using ACL.Synchronizers.Product;
using ACL.Synchronizers.Product.Repositories;
using ACL.Utils;
using ACL.Workers;
using Mapster;
using Serilog;
using BubbleDeliveries = ACL.Synchronizers.BubbleDeliveries.Repositories;
using LegacyDeliveries = ACL.Synchronizers.LegacyDeliveries.Repositories;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ProductSyncWorker>();
builder.Services.AddHostedService<OutboxWorker>();
builder.Services.AddHostedService<DeliverySyncWorker>();

// Legacy Product
builder.Services.AddTransient<LegacyProductSynchronizer>();
builder.Services.AddTransient<LegacyOutboxProductSynchronizer>();
builder.Services.AddTransient<BubbleProductRepository>();
builder.Services.AddTransient<LegacyProductRepository>();

// Legacy Delivery
builder.Services.AddTransient<LegacyDeliverySynchronizer>();
builder.Services.AddTransient<LegacyOutboxDeliverySynchronizer>();
builder.Services.AddTransient<LegacyDeliveries.BubbleDeliveryRepository>();
builder.Services.AddTransient<LegacyDeliveries.LegacyDeliveryRepository>();

// Bubble Delivery
builder.Services.AddTransient<BubbleDeliverySynchronizer>();
builder.Services.AddTransient<BubbleOutboxDeliverySynchronizer>();
builder.Services.AddTransient<BubbleDeliveries.BubbleDeliveryRepository>();
builder.Services.AddTransient<BubbleDeliveries.LegacyDeliveryRepository>();

// Common
builder.Services.AddTransient<BubbleSynchronizationRepository>();
builder.Services.AddTransient<LegacySynchronizationRepository>();
builder.Services.AddTransient<BubbleOutboxRepository>();
builder.Services.AddTransient<LegacyOutboxRepository>();
builder.Services.AddTransient<PostgreSqlGenerator>();

DapperConfiguration.ConfigureSnakeCaseMapping();

var config = TypeAdapterConfig.GlobalSettings;
config.Scan(Assembly.GetExecutingAssembly());

LegacyConnectionString.Value = builder.Configuration.GetConnectionString("Legacy")!;
BubbleConnectionString.Value = builder.Configuration.GetConnectionString("Bubble")!;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

var host = builder.Build();
host.Run();
