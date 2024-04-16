using ACL.Synchronizers.CommonRepositories;
using ACL.Synchronizers.Product;
using ACL.Workers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ProductSyncWorker>();
builder.Services.AddHostedService<OutboxWorker>();

builder.Services.AddTransient<LegacyProductSynchronizer>();

builder.Services.AddTransient<LegacyOutboxRepository>();

var host = builder.Build();
host.Run();
