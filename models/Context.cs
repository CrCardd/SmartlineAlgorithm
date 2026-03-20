namespace Alg.Models;

public class Context
{
    public List<Product> Products {get;set;} = []; 
    public List<Demand> Demands {get;set;} = []; 
    public List<Cell> Cells {get;set;} = []; 

    public Context(string path)
    {
        Products = GetProducts($"{path}/Product.csv");
        Demands = GetDemands($"{path}/Demand.csv", Products);
        Cells = GetCells($"{path}/Cell.csv");
    }

    public static List<Product> GetProducts(string path)
    {
        string[] lines = File.ReadAllLines(path);
        List<Product> products = [];

        foreach (var lin in lines)
        {
            var items = lin.Split(";");

            int id = int.Parse(items[0]);
            string name = items[1];
            int shop = int.Parse(items[2]);
            int time = int.Parse(items[3]);

            products.Add(
                new(id, name, time, shop)
            );
        }
        return products;
    }
    public static List<Demand> GetDemands(string path, List<Product> products)
    {
        string[] lines = File.ReadAllLines(path);
        List<Demand> demands = [];

        foreach (var lin in lines)
        {
            var items = lin.Split(";");

            int id = int.Parse(items[0]);
            int productId = int.Parse(items[1]);


            var d = items[2].Split("/");
            int day = int.Parse(d[0]);
            int month = int.Parse(d[1]);
            int year = int.Parse(d[2]);
            DateOnly date = new(year, month, day);

            int qnt = int.Parse(items[3]);

            Product? product = products.FirstOrDefault(p => p.Id == productId);

            if (product == null)
                continue;

            demands.Add(new Demand(id, product, date, qnt));
        }
        return demands;
    }
    public static List<Cell> GetCells(string path)
    {
        string[] lines = File.ReadAllLines(path);
        List<Cell> machines = [];

        foreach (var lin in lines)
        {
            var items = lin.Split(";");

            int id = int.Parse(items[0]);
            string name = items[1];

            machines.Add(
                new(id, name)
            );
        }
        return machines;
    }
}