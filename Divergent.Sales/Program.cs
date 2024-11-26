using Divergent.Sales.Data.Migrations;
using ITOps.EndpointConfig;
using NServiceBus;

const string EndpointName = "Divergent.Sales";

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var builder = Host.CreateApplicationBuilder();

builder.Services.Configure<LiteDbOptions>(configuration.GetSection("LiteDbOptions"))
            .Configure<LiteDbOptions>(s =>
            {
                s.DatabaseName = "sales";
                s.DatabaseInitializer = DatabaseInitializer.Initialize;
            });
builder.Services.AddSingleton<ILiteDbContext, LiteDbContext>();

builder.AddServiceDefaults();

builder.ConfigureNServiceBus(EndpointName);

var app = builder.Build();

var hostEnvironment = app.Services.GetRequiredService<IHostEnvironment>();

Console.Title = hostEnvironment.ApplicationName;

app.Run();