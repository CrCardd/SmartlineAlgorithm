using Alg.Philosophers;

var scheduleA = Algorithm.Solver(d => new Aristoteles(d));
var scheduleP = Algorithm.Solver(d => new Plank(d));


#region UI 
/*
foreach(var day in schedule.Days)
{
    Console.WriteLine(new string('-',120));
    Console.WriteLine(day.Date);
    for(int i=0; i<day.Setups.Count; i++)
    {
        Console.Write($"\t{i+1}\t|");
        foreach(var cell in day.Setups[i].Cells)
        {
            Console.Write($"\t{cell.Name} -> ");
            foreach(var product in cell.Products)
                Console.Write($"[{product.Name}]");
        }
        Console.WriteLine();
    }
}
*/

Console.WriteLine($"\n\n{"PRODUTO",-10} | {"EXCEDENTE",-18}");
foreach (var d in scheduleA.Excess)
    Console.WriteLine($"{d.Product.Name,-10} | {d.Quantity*-1,-18:F2}");

Console.WriteLine($"\n\n{"PRODUTO",-10} | {"EXCEDENTE",-18}");
foreach (var d in scheduleP.Excess)
    Console.WriteLine($"{d.Product.Name,-10} | {d.Quantity*-1,-18:F2}");

#endregion