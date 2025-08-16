using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// CORS: allow the WASM client during development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(p => p
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

// Memory cache for simple performance win
builder.Services.AddMemoryCache();

// Optional: Swagger for quick inspection
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(); // enable the default CORS policy

// --- keep all top-level statements BEFORE any type declarations ---

// Seed data (local function is a top-level statement, so it must be before types)
static ProductDto[] SeedProducts() => new[]
{
    new ProductDto(1, "Laptop", 1200.50, 25, new CategoryDto(101, "Electronics")),
    new ProductDto(2, "Headphones", 50.00, 100, new CategoryDto(102, "Accessories")),
    new ProductDto(3, "Mouse", 19.99, 250, new CategoryDto(102, "Accessories"))
};

// Activity 1: original endpoint
app.MapGet("/api/products", () => SeedProducts());

// Activity 2: updated endpoint (route change to /api/productlist)
app.MapGet("/api/productlist", ([FromServices] IMemoryCache cache) =>
{
    // Simple cache to avoid recomputing/allocating on every request
    // (also a nice hook to later fetch from DB and cache for 30s)
    const string key = "productlist";
    if (!cache.TryGetValue(key, out ProductDto[]? products))
    {
        products = SeedProducts();
        cache.Set(key, products, TimeSpan.FromMinutes(1));
    }
    return Results.Ok(products);
});

app.Run();

// --- after this point, ONLY type/namespace declarations ---

public record CategoryDto(int Id, string Name);
public record ProductDto(int Id, string Name, double Price, int Stock, CategoryDto Category);
