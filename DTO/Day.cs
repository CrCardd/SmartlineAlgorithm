using Alg.Models;

namespace Alg.DTO;

public record Day(
    DateOnly date,
    List<List<Cell>> setups
);