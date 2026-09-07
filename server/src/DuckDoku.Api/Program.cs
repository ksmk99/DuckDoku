using DuckDoku.Api;
using DuckDoku.Api.Endpoints;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.local.json", optional: true);
}

string? port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

const string CorsPolicyName = "ClientOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        string[] allowedOrigins = (builder.Configuration["AllowedOrigins"] ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddSingleton<LevelCatalogService>();
builder.Services.AddSingleton<ILevelCatalogLoader, LevelCatalogLoader>();

builder.Services.AddHostedService<LevelCatalogInitializer>();

WebApplication app = builder.Build();


if (app.Environment.IsDevelopment())
{
    using IServiceScope scope = app.Services.CreateScope();

    AppDbContext database =
        scope.ServiceProvider.GetRequiredService<AppDbContext>();

    database.Database.EnsureDeleted();
    database.Database.EnsureCreated();

    app.MapDevEndpoints();
}
else
{
    using IServiceScope scope = app.Services.CreateScope();

    AppDbContext database =
        scope.ServiceProvider.GetRequiredService<AppDbContext>();

    database.Database.EnsureCreated();
}

app.UseExceptionHandler();

app.UseCors(CorsPolicyName);

app.MapOpenApi();
app.MapScalarApiReference();

app.MapHealthEndpoints();
app.MapAuthEndpoints();
app.MapProfileEndpoints();
app.MapServerTimeEndpoints();
app.MapLevelEndpoints();

app.Run();
