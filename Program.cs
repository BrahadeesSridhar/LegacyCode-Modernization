using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LegacyCodeModernization.Interfaces;
using LegacyCodeModernization.Repositories;
using LegacyCodeModernization.Services;
using System.Data.SqlClient;
using Dapper;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Legacy Code Modernization API", 
        Version = "v1",
        Description = "A modernized version of the legacy order processing system"
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Register application services
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<OrderProcessingService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Legacy Code Modernization API V1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the root
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

try
{
    Console.WriteLine("Starting application...");

    // Build configuration
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    Console.WriteLine("Configuration loaded.");

    // Create service collection
    var services = new ServiceCollection();

    // Configure services
    services.AddSingleton<IConfiguration>(configuration);
    services.AddLogging(builder =>
    {
        builder.AddConsole();
        builder.SetMinimumLevel(LogLevel.Debug);
    });

    // Register dependencies
    services.AddScoped<IOrderRepository, OrderRepository>();
    services.AddScoped<IProductRepository, ProductRepository>();
    services.AddScoped<OrderProcessingService>();

    Console.WriteLine("Services configured.");

    // Build service provider
    using var serviceProvider = services.BuildServiceProvider();

    // Get service instance
    var orderService = serviceProvider.GetRequiredService<OrderProcessingService>();
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

    logger.LogInformation("Services resolved successfully.");

    // Get the first product ID from the database
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine($"Using connection string: {connectionString}");

    using var connection = new SqlConnection(connectionString);
    await connection.OpenAsync();
    Console.WriteLine("Database connection opened successfully.");

    var productId = await connection.QueryFirstOrDefaultAsync<Guid>("SELECT Id FROM Products");
    Console.WriteLine($"Retrieved product ID: {productId}");

    if (productId == Guid.Empty)
    {
        throw new Exception("No products found in the database");
    }

    // Sample order processing
    var customerId = Guid.NewGuid(); // Sample customer ID
    var quantity = 2;
    var express = true;

    Console.WriteLine($"Processing order for Product ID: {productId}");
    var result = await orderService.ProcessOrderAsync(customerId, productId, quantity, express);
    
    Console.WriteLine($"Order processing result: {result.Message}");
    Console.WriteLine($"Success: {result.Success}");
}
catch (SqlException ex)
{
    Console.WriteLine($"Database Error: {ex.Message}");
    Console.WriteLine($"Error Number: {ex.Number}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner Error: {ex.InnerException.Message}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner Error: {ex.InnerException.Message}");
    }
} 