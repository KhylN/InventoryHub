namespace ServerApp.Dtos;

public record ProductDto(int Id, string Name, double Price, int Stock, CategoryDto Category);
