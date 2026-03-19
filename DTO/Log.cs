
namespace Alg.DTO;

public class LogPayload
{
    public List<CellPayload> Cells {get;set;} = [];
}

public class CellPayload(int id, string name){
    public int Id {get;set;} = id;
    public string Name {get;set;} = name;
    public List<ProductPayload> Products {get;set;} = [];
};
public record ProductPayload(
    int Id,
    string Name
);