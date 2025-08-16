using System.Net.Http.Json;

namespace ClientApp.Services;

public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http) => _http = http;

    public async Task<Product[]> GetProductsAsync(CancellationToken ct = default)
    {
        // Activity 2 fix: use /api/productlist (route updated server-side)
        // EnsureSuccessStatusCode + explicit read to safely handle malformed JSON
        using var resp = await _http.GetAsync("/api/productlist", ct);
        resp.EnsureSuccessStatusCode();
        var products = await resp.Content.ReadFromJsonAsync<Product[]>(cancellationToken: ct);
        return products ?? Array.Empty<Product>();
    }
}

// Strongly-typed models matching the back-end JSON
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public double Price { get; set; }
    public int Stock { get; set; }
    public Category Category { get; set; } = new();
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}
