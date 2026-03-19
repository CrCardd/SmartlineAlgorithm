using Alg.DTO;
using Alg.Enums;
using Alg.Models;

public static class Algorithm
{
    #region PROPERTIES

    private const UnitOfMeasure SetupCoolDownUOM = UnitOfMeasure.SHIFT; 
    private const UnitOfMeasure ProduceTimeUOM = UnitOfMeasure.MINUTE; 
    private const int CORES_PER_CELL = 2;
    private const int SETUP_COOLDOWN = 1;
    private const int SHIFTS = 3;
    private const int HOUR_PER_SHIFT = 8;

    #endregion
    #region FUNCTIONS
    // Organize solver logs
    public static List<DayPayload> Solver(ref List<Demand> demands, List<Cell> cells)
    {
        DateOnly crrDate = DateOnly.FromDateTime(DateTime.Today);
        List<LogPayload> Logs = [];
        List<DayPayload> Schedule = [];
        
        int gap = HOUR_PER_SHIFT*(int)UnitOfMeasure.HOUR*SHIFTS / (int)SetupCoolDownUOM;
    
        int step = 0;
        while(demands.Any(d => d.Quantity > 0))
        {
            var log = Setup(ref demands,cells);
            for(int i=0; i<SETUP_COOLDOWN; i++)
            {
                step++;
                Logs.Add(log);
                if(step >= gap)
                {
                    crrDate.AddDays(1);
                    Schedule.Add(new(crrDate, Logs));

                    Logs = [];
                    step = 0;
                }
            }
        }
        if(step != gap)
        {
            crrDate.AddDays(1);
            Schedule.Add(new(crrDate, Logs));
        }

        return Schedule;
    }

    // Define the best setup of each cell
    public static LogPayload Setup(ref List<Demand> demands, List<Cell> cells)
    {
        LogPayload log = new();

        for(int i=0; i<cells.Count;i++)
        {
            CellPayload cell = new(cells[i].Id,cells[i].Name);
            var philosopher = new Philosophers(demands);
            for(int j=0; j<CORES_PER_CELL; j++)
            {
                philosopher.Aristoteles(cells);
                if(philosopher.GetPriority is null)
                    break;
                var priority = philosopher.GetPriority;

                cell.Products.Add(new(priority.Product.Id, priority.Product.Name));
                priority.Quantity -= (SETUP_COOLDOWN*(int)SetupCoolDownUOM) / (philosopher.GetTime * (int)ProduceTimeUOM);
                philosopher.AddPriority(priority);
            }
            demands = Copy(philosopher.GetDinamicDemands);
            log.Cells.Add(cell);
            if(philosopher.GetPriority is null)
                return log;
        }

        return log;
    }

    // Fix the production to when the new product's time be overpass the last 
    public static List<Demand> Fixes(List<Demand> demands, List<int> priorities, int time)
    {
        var copy = Copy(demands);
        foreach(var priorityId in priorities)
            copy.First(d => d.Id == priorityId).Quantity -= (SETUP_COOLDOWN*(int)SetupCoolDownUOM) / (time * (int)ProduceTimeUOM);
        return copy;
    }

    // Get the best demand for the current moment
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