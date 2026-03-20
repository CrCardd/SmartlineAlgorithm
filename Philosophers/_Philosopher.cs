using Alg.Models;

public abstract class Philosopher(List<Demand> crrDemands)
{
    #region PROPERTIES
    protected List<Demand> DinamicDemands = crrDemands.Select(d => new Demand(d.Id, d.Product, d.Date, d.Quantity)).ToList();
    protected List<int> Priorities = [];
    protected int? PriorityId = null;
    protected int Time = 0;

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
    #region FUNCTION
    public abstract void Think(List<Cell> cells);
    #endregion
}
