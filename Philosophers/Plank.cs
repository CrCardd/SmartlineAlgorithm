using Alg.Models;

namespace Alg.Philosophers;

public class Plank(List<Demand> crrDemands) : Philosopher(crrDemands)
{
    public override void Think(List<Cell> cells)
    {
        // DECREASE EXCESS
        PriorityId = Algorithm.Logic(DinamicDemands, cells, PriorityId);
        if(PriorityId is null) return;
        if(GetPriority!.Product.Time > Time)
        {
            Time = GetPriority.Product.Time;
            DinamicDemands = Algorithm.Fixes(Priorities, Time);
            PriorityId = Algorithm.Logic(DinamicDemands,cells,PriorityId);
        }
    }
}