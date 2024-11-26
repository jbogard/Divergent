using Divergent.Customers.Data.Migrations;
using ITOps.EndpointConfig;
using NServiceBus;

const string EndpointName = "Divergent.Customers";

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
    
builder.Services.Configure<LiteDbOptions>(configuration.GetSection("LiteDbOptions"))
            .Configure<LiteDbOptions>(s =>
            {
                s.DatabaseName = "customers";
                s.DatabaseInitializer = DatabaseInitializer.Initialize;
            });
builder.Services.AddSingleton<ILiteDbContext, LiteDbContext>();
 
builder.ConfigureNServiceBus(EndpointName);

var host = builder.Build();

var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();

Console.Title = hostEnvironment.ApplicationName;

host.Run();