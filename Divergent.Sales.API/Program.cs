using Divergent.Sales.Data.Migrations;
using Divergent.Sales.Messages.Commands;
using ITOps.EndpointConfig;

var builder = WebApplication.CreateBuilder(args);

var config = new EndpointConfiguration("Sales.API");

config.SendOnly();

var transport = new RabbitMQTransport(
    RoutingTopology.Conventional(QueueType.Quorum),
    builder.Configuration.GetConnectionString("broker")
);

var routing = config.UseTransport(transport);

routing.RouteToEndpoint(typeof(SubmitOrderCommand), "Divergent.Sales");

config.UseSerialization<NewtonsoftJsonSerializer>();
config.UsePersistence<LearningPersistence>();

config.SendFailedMessagesTo("error");

config.Conventions()
    .DefiningCommandsAs(t => t.Namespace != null && t.Namespace == "Divergent.Messages" || t.Name.EndsWith("Command"))
    .DefiningEventsAs(t => t.Namespace != null && t.Namespace == "Divergent.Messages" || t.Name.EndsWith("Event"));

builder.UseNServiceBus(config);

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.Configure<LiteDbOptions>(builder.Configuration.GetSection("LiteDbOptions"))
    .Configure<LiteDbOptions>(s =>
    {
        s.DatabaseName = "sales";
        s.DatabaseInitializer = DatabaseInitializer.Initialize;
    });
builder.Services.AddSingleton<ILiteDbContext, LiteDbContext>();

builder.Services.AddCors();

builder.AddServiceDefaults();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors(policyBuilder =>
{
    policyBuilder.AllowAnyOrigin();
    policyBuilder.AllowAnyMethod();
    policyBuilder.AllowAnyHeader();
});

app.MapDefaultEndpoints();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

var hostEnvironment = app.Services.GetRequiredService<IHostEnvironment>();

Console.Title = hostEnvironment.ApplicationName;

app.Run();