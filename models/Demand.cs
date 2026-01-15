namespace Alg.Models;

public class Demand(int id, Product product, DateOnly date, int qnt) : Model(id)
{
    public Product Product {get;set;} = product;
    public DateOnly Date {get;set;} = date;
    public int Quantity {get;set;} = qnt;
}