using ACL.ConnectionStrings;
using ACL.Synchronizers.Delivery;
using ACL.Synchronizers.Outbox;
using ACL.Synchronizers.Product;
using ACL.Workers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<DeliverySynchronizerWorker>();
builder.Services.AddHostedService<ProductSynchronizerWorker>();
builder.Services.AddHostedService<OutboxProcessingWorker>();

builder.Services.AddSingleton<FromLegacyToBubbleDeliverySynchronizer>();
builder.Services.AddSingleton<FromBubbleToLegacyDeliverySynchronizer>();
builder.Services.AddSingleton<FromLegacyToBubbleProductSynchronizer>();

builder.Services.AddSingleton<BubbleOutboxDeliverySynchronizer>();
builder.Services.AddSingleton<LegacyOutboxDeliverySynchronizer>();
builder.Services.AddSingleton<LegacyOutboxProductSynchronizer>();

LegacyConnectionString.Set(builder.Configuration.GetConnectionString("Legacy")!);
BubbleConnectionString.Set(builder.Configuration.GetConnectionString("Bubble")!);

var host = builder.Build();
host.Run();
