var builder = DistributedApplication.CreateBuilder(args);

builder.AddOpenTelemetryCollector("collector", "config.yaml")
    .WithAppForwarding();

var rmqPassword = builder.AddParameter("messaging-password");

var broker = builder.AddRabbitMQ(name: "broker", password: rmqPassword, port: 5672)
    .WithDataVolume()
    .WithManagementPlugin()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint("management", e => e.Port = 15672);

builder.AddProject<Projects.Divergent_CompositionGateway>("composition-gateway");

builder.AddProject<Projects.Divergent_Customers>("customers-endpoint")
    .WithReference(broker)
    .WaitFor(broker);
builder.AddProject<Projects.Divergent_Customers_API>("customers-api");

builder.AddProject<Projects.Divergent_Finance>("finance-endpoint")
    .WithReference(broker)
    .WaitFor(broker);
builder.AddProject<Projects.Divergent_Finance_API>("finance-api");

builder.AddProject<Projects.Divergent_Frontend>("frontend");

builder.AddProject<Projects.Divergent_ITOps>("itops")
    .WithReference(broker)
    .WaitFor(broker);

builder.AddProject<Projects.Divergent_Sales>("sales-endpoint")
    .WithReference(broker)
    .WaitFor(broker);
builder.AddProject<Projects.Divergent_Sales_API>("sales-api")
    .WithReference(broker)
    .WaitFor(broker);

builder.AddProject<Projects.Divergent_Shipping>("shipping-endpoint")
    .WithReference(broker)
    .WaitFor(broker);

builder.AddProject<Projects.PaymentProviders>("payment-providers");

builder.Build().Run();

