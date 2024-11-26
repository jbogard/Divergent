using Divergent.Finance.API;
using Divergent.Finance.Data.Migrations;
using ITOps.EndpointConfig;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.Configure<LiteDbOptions>(builder.Configuration.GetSection("LiteDbOptions"))
    .Configure<LiteDbOptions>(s =>
    {
        s.DatabaseName = "finance";
        s.DatabaseInitializer = DatabaseInitializer.Initialize;
    });
builder.Services.AddSingleton<ILiteDbContext, LiteDbContext>();
            
builder.Services.AddCors();

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

app.UseRouting();

app.UseAuthorization();

app.MapDefaultEndpoints();

app.MapControllers();

var hostEnvironment = app.Services.GetRequiredService<IHostEnvironment>();

Console.Title = hostEnvironment.ApplicationName;

app.Run();
