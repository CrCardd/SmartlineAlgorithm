using Alg.Enums;
using Alg.Models;

namespace Alg.DTO;

public record Log(
    Product Product,
    DateOnly Deadline,
    DateOnly DeliveredAt,
    int DistanceDays
);