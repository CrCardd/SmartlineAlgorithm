using Alg.DTO;
using Alg.Enums;
using Alg.Models;



public static class Algorithm
{
    #region PROPERTIES

    private static Context ctx = new("./database");
    private const UnitOfMeasure SetupCoolDownUOM = UnitOfMeasure.SHIFT; 
    private const UnitOfMeasure ProduceTimeUOM = UnitOfMeasure.MINUTE; 
    private const int CORES_PER_CELL = 2;
    private const int SETUP_COOLDOWN = 1;
    private const int SHIFTS = 3;
    private const int HOUR_PER_SHIFT = 8;

    #endregion
    #region FUNCTIONS
    // Organize solver logs
    public static Schedule Solver<T>(Func<List<Demand>, T> factory) where T : Philosopher
    {
        DateOnly crrDate = DateOnly.FromDateTime(DateTime.Today);
        List<Demand> demands = ctx.Demands
            .Where(d => d.Quantity > 0)
            .Where(d => d.Quantity - d.Product.Shop > 0)
            .ToList();
        
        List<Cell> cells = ctx.Cells;

        List<LogPayload> Logs = [];
        Schedule Schedule = new();
        
        int gap = HOUR_PER_SHIFT*(int)UnitOfMeasure.HOUR*SHIFTS / (int)SetupCoolDownUOM;

        int step = 0;
        while(demands.Any(d => d.Quantity > 0))
        {
            var log = Setup<T>(factory);
            for(int i=0; i<SETUP_COOLDOWN; i++)
            {
                step++;
                Logs.Add(log);
                if(step >= gap)
                {
                    crrDate = crrDate.AddDays(1);
                    Schedule.Days.Add(new(crrDate, Logs));

                    Logs = [];
                    step = 0;
                }
            }
        }
        if(step != gap)
        {
            crrDate.AddDays(1);
            Schedule.Days.Add(new(crrDate, Logs));
        }

        Schedule.Excess = demands.GroupBy(d => d.Product.Id)
            .Select(d => {
                int quantity = d.Sum(a => a.Quantity);
                Product find = ctx.Products.First(p => p.Id == d.Key);
                ProductPayload product = new(find.Id, find.Name);
                return new DemandPayload(product, quantity);
        }).ToList();

        return Schedule;
    }

    // Define the best setup of each cell
    public static LogPayload Setup<T>(
        Func<List<Demand>, T> factory
    ) where T : Philosopher
    {
        List<Cell> cells = ctx.Cells;
        LogPayload log = new();

        for(int i=0; i<cells.Count;i++)
        {
            CellPayload cell = new(cells[i].Id,cells[i].Name);
            T philosopher = factory(ctx.Demands);
            for(int j=0; j<CORES_PER_CELL; j++)
            {
                philosopher.Think(cells);
                if(philosopher.GetPriority is null)
                    break;
                var priority = philosopher.GetPriority;

                cell.Products.Add(new(priority.Product.Id, priority.Product.Name));
                Produce(philosopher.GetDinamicDemands, priority, (SETUP_COOLDOWN*(int)SetupCoolDownUOM) / (philosopher.GetTime * (int)ProduceTimeUOM));
                // priority.Quantity -= (SETUP_COOLDOWN*(int)SetupCoolDownUOM) / (philosopher.GetTime * (int)ProduceTimeUOM);
                philosopher.AddPriority(priority);
            }
            foreach(var dd in philosopher.GetDinamicDemands)
                ctx.Demands.First(d => d.Id == dd.Id).Quantity = dd.Quantity;

            log.Cells.Add(cell);
            if(philosopher.GetPriority is null)
                return log;
        }

        return log;
    }

    // Fix the production to when the new product's time be overpass the last 
    public static List<Demand> Fixes(List<int> priorities, int time)
    {
        var copy = Copy(ctx.Demands);
        foreach(var priorityId in priorities)
            Produce(copy, copy.First(d => d.Id == priorityId), (SETUP_COOLDOWN*(int)SetupCoolDownUOM) / (time * (int)ProduceTimeUOM));
            // copy.First(d => d.Id == priorityId).Quantity -= (SETUP_COOLDOWN*(int)SetupCoolDownUOM) / (time * (int)ProduceTimeUOM);
        return copy;
    }

    public static void Produce(List<Demand> demands, Demand demand, int quantity)
    {
        demand.Quantity -= quantity;
        // Console.WriteLine(demand.Quantity);
        if(demand.Quantity < 0)
        {
            var excess = demands
                .Where(d => d.Quantity > 0)
                .FirstOrDefault(d => d.Product.Id == demand.Product.Id);
            if(excess is not null)
            {
                Produce(demands, excess, -demand.Quantity);
                demand.Quantity = 0;
            }
        }
    } 

    // Get the best demand for the current moment - INCOMPLETE
    public static int? Logic(List<Demand> demands, List<Cell> cells, int? priorityId = null)
    {
        var pending = demands
            .Where(d => d.Quantity > 0)
            .ToList();
        if(priorityId != null)
        {
            var time = demands.First(d => d.Id == priorityId).Product.Time;
            pending = pending
                .OrderBy(d => Math.Abs(d.Product.Time-time))
                .ToList();
        }
    
        return pending.FirstOrDefault()?.Id;
    }
    
    #endregion
    #region MISC_FUNCIONS
    // Copy Demand's list
    public static List<Demand> Copy(List<Demand> list)
    {
        return list
            .Select(l => new Demand(l.Id,l.Product,l.Date,l.Quantity))
            .ToList();
    }
    #endregion
}