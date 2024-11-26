using System.Reflection;
using Divergent.ITOps;
using Divergent.ITOps.Interfaces;
using ITOps.EndpointConfig;
using NServiceBus;

const string EndpointName = "Divergent.ITOps";

var builder = Host.CreateApplicationBuilder();
var assemblies = ReflectionHelper.GetAssemblies(".Data.dll");

builder.AddServiceDefaults();
        
// Find and register all types that end with 'Provider' so we can inject them into ShipWithFedexCommandHandler
// Those types are included by adding a reference to
//   - Divergent.Customers.Data
//   - Divergent.Shipping.Data
// Normally we deploy them together with Divergent.ITOps using our CI pipeline, but that's impossible
//   for this workshop, where we need [F5] to work.
builder.Services.Scan(s =>
{
    s.FromAssemblies(assemblies)
        .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Provider")))
        .AsImplementedInterfaces()
        .WithTransientLifetime();
});

// This loads all IRegisterServices to make sure we can access the database for each provider
var serviceRegistrars = assemblies
    .SelectMany(a => a.GetTypes())
    .Where(t => typeof(IRegisterServices).IsAssignableFrom(t))
    .Select(Activator.CreateInstance)
    .Cast<IRegisterServices>()
    .ToList();

foreach (var serviceRegistrar in serviceRegistrars)
{
    serviceRegistrar.Register(builder, builder.Services);
}

builder.ConfigureNServiceBus(EndpointName);

var app = builder.Build();

var hostEnvironment = app.Services.GetRequiredService<IHostEnvironment>();

Console.Title = hostEnvironment.ApplicationName;

app.Run();