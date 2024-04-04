using ACL.ConnectionStrings;
using ACL.Workers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ProductSynchronizerWorker>();

LegacyConnectionString.Value = builder.Configuration.GetConnectionString("Legacy")!;
BubbleConnectionString.Value = builder.Configuration.GetConnectionString("Bubble")!;

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var host = builder.Build();
host.Run();
