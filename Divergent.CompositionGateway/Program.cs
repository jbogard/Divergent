using ITOps.ViewModelComposition;
using ITOps.ViewModelComposition.Gateway;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRouting();
builder.Services.AddViewModelComposition();
builder.Services.AddCors();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseCors(policyBuilder =>
{
    policyBuilder.AllowAnyOrigin();
    policyBuilder.AllowAnyMethod();
    policyBuilder.AllowAnyHeader();
});

app.RunCompositionGatewayWithDefaultRoutes();

var hostEnvironment = app.Services.GetRequiredService<IHostEnvironment>();

Console.Title = hostEnvironment.ApplicationName;

app.Run();