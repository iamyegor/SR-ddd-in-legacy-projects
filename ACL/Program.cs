using ACL.Synchronizers.CommonRepositories.Outbox;
using ACL.Synchronizers.Product;
using ACL.Utils;
using ACL.Workers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ProductSyncWorker>();
builder.Services.AddHostedService<OutboxWorker>();
builder.Services.AddHostedService<DeliverySyncWorker>();

builder.Services.AddTransient<LegacyProductSynchronizer>();

builder.Services.AddTransient<LegacyOutboxRepository>();

DapperConfiguration.ConfigureSnakeCaseMapping();

var host = builder.Build();
host.Run();
