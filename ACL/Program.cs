using ACL;
using ACL.ConnectionStrings;
using ACL.Workers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ProductSynchronizerWorker>();

LegacyConnectionString.Value = builder.Configuration.GetConnectionString("Legacy")!;

var host = builder.Build();
host.Run();