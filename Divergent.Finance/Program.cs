using Divergent.Finance.Data.Migrations;
using Divergent.Finance.PaymentClient;
using ITOps.EndpointConfig;
using NServiceBus;

const string EndpointName = "Divergent.Finance";

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<ReliablePaymentClient>();

builder.Services.Configure<LiteDbOptions>(configuration.GetSection("LiteDbOptions"))
            .Configure<LiteDbOptions>(s =>
            {
                s.DatabaseName = "finance";
                s.DatabaseInitializer = DatabaseInitializer.Initialize;
            });
builder.Services.AddSingleton<ILiteDbContext, LiteDbContext>();
    
builder.ConfigureNServiceBus(EndpointName);

var host = builder.Build();

var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();

Console.Title = hostEnvironment.ApplicationName;

host.Run();