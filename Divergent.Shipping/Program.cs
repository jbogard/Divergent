using Divergent.ITOps.Messages.Commands;
using ITOps.EndpointConfig;

const string EndpointName = "Divergent.Shipping";

var builder = Host.CreateApplicationBuilder(args);

builder.ConfigureNServiceBus(EndpointName, routing =>
{
    routing.RouteToEndpoint(typeof(ShipWithFedexCommand), "Divergent.ITOps");
});

builder.AddServiceDefaults();

var app = builder.Build();

var hostEnvironment = app.Services.GetRequiredService<IHostEnvironment>();

Console.Title = hostEnvironment.ApplicationName;

app.Run();