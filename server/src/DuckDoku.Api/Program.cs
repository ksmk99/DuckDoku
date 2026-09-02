using DuckDoku.Api;
using DuckDoku.Api.Endpoints;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Development.local.json", optional: true);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

WebApplication app = builder.Build();


if (app.Environment.IsDevelopment())                                              
{                                                                                 
    using IServiceScope scope = app.Services.CreateScope();                       
                                                                                    
    AppDbContext database =                                                       
        scope.ServiceProvider.GetRequiredService<AppDbContext>();                         
                                                                                    
    database.Database.EnsureDeleted();                                            
    database.Database.EnsureCreated();                                            
}  

app.UseExceptionHandler();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapHealthEndpoints();
app.MapAuthEndpoints();
app.MapProfileEndpoints();
app.MapServerTimeEndpoints();

app.Run();
