using Alg.Models;

namespace Alg.Philosophers;

public class Aristoteles(List<Demand> frozenDemands) : Philosopher(frozenDemands)
{
    // INCREASE EXCESS
    public override void Think(List<Cell> cells)
    {
        var priorityIdTemp = Algorithm.Logic(DinamicDemands, cells, PriorityId);
        var timeTemp = Time;
        if(priorityIdTemp is null) return;
        if(DinamicDemands.First(d => d.Id == priorityIdTemp).Product.Time > timeTemp)
        {
            timeTemp = DinamicDemands.First(d => d.Id == priorityIdTemp).Product.Time;
            DinamicDemands = Algorithm.Fixes(FrozenDemands, Priorities, timeTemp);
            priorityIdTemp = Algorithm.Logic(DinamicDemands,cells,PriorityId);
            if(priorityIdTemp is null) return;
            if(DinamicDemands.First(d => d.Id == priorityIdTemp).Product.Time < timeTemp)
            {
                timeTemp = DinamicDemands.First(d => d.Id == priorityIdTemp).Product.Time;
                DinamicDemands = Algorithm.Fixes(FrozenDemands, Priorities, timeTemp);
            }
        }
        Time = timeTemp;
        PriorityId = priorityIdTemp;
    }
}