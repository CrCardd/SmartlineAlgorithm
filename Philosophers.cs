using Alg.Models;

public class Philosophers(List<Demand> frozenDemands)
{
    #region PROPERTIES
    private List<Demand> FrozenDemands = frozenDemands.Select(d => new Demand(d.Id, d.Product, d.Date, d.Quantity)).ToList();
    private List<Demand> DinamicDemands = frozenDemands.Select(d => new Demand(d.Id, d.Product, d.Date, d.Quantity)).ToList();
    private List<int> Priorities = [];
    private int? PriorityId = null;
    private int Time = 0;

    #endregion
    #region METHODS
    public Demand? GetPriority
        => PriorityId is null ? null : DinamicDemands.First(d => d.Id == PriorityId);
    public int GetTime
        => Time;
    public void AddPriority(Demand demand)
        => Priorities.Add(demand.Id);
    public List<Demand> GetDinamicDemands
        => DinamicDemands;
    
    #endregion
    #region FUNCTIONS

    // PRIORIZE PRODUCTION
    public void Aristoteles(List<Cell> cells)
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

    // PRIORIZE TETRIS
    public void Plank(List<Cell> cells)
    {
        PriorityId = Algorithm.Logic(DinamicDemands, cells, PriorityId);
        if(PriorityId is null) return;
        if(GetPriority!.Product.Time > Time)
        {
            Time = GetPriority.Product.Time;
            DinamicDemands = Algorithm.Fixes(FrozenDemands, Priorities, Time);
            PriorityId = Algorithm.Logic(DinamicDemands,cells,PriorityId);
        }
    }

    // public void Leibniz(){}
    // public void Bohr(){}
    #endregion
}





