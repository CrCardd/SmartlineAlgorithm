using Alg.Models;

List<Product> Products = Lodaer.GetProducts("./database/Product.csv");
List<Demand> Demands = Lodaer.GetDemands("./database/Demand.csv", Products);
List<Cell> Machines = Lodaer.GetCells("./database/Cell.csv");

Demands = Demands
    .Select(d =>
    {
            d.Quantity -= d.Product.Shop;
            return d;
    })
    .Where(d => d.Quantity > 0)
    .ToList();

Algorithm.solver(Demands, Machines);

//EXCEDENTE = PEÇAS PRODUZIDAS ALÉM DO NECESSÁRIO, SE ESTIVER NEGATIVO É PORQUE FALTOU PEÇA
Console.WriteLine($"\n\n{"PRODUTO",-10} | {"EXCEDENTE",-18}");
foreach (var d in Demands)
    Console.WriteLine($"{d.Product.Name,-10} | {d.Quantity*-1,-18:F2}");