namespace Alg.Models;

public class Product(int id, string name, int time, int shop) : Model(id)
{
    public string Name { get; set; } = name;
    public int Time { get; set; } = time;
    public int Shop { get; set; } = shop;
}