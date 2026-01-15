namespace Alg.Models;

public class Day(DateOnly date, List<List<Cell>> setups)
{
    public DateOnly Date = date;
    public List<List<Cell>> Setups { get; set; } = setups;
}