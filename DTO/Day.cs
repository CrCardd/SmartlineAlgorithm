using Alg.Models;

namespace Alg.DTO;

public record DayPayload(
    DateOnly Date,
    List<LogPayload> Setups
);