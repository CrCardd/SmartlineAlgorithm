namespace Alg.Models;

public class Cell(int id, string name, List<Product>? products)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public List<Product>? ProductsToProduct { get; set; } = products;
}