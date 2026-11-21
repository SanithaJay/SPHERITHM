
using Spherithm.Application.Abstractions;
using Spherithm.Application.Services;
using Spherithm.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Application services
builder.Services.AddScoped<IPlatformStatusService, PlatformStatusService>();

// Infrastructure services: database and repositories
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
