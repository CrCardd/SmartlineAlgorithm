using Alg.DTO;
using Alg.Models;

public static class Algorithm
{
    public const float OEE = 0.85f;
    public const int SHIFTS = 3;
    public const int HOUR_PER_SHIFT = 8;
    public const int CORES_PER_CELL = 2;
    public const float HOURS_PER_SHIFT_OEE = HOUR_PER_SHIFT * OEE;


    public static List<Day> solver(List<Demand> demands, List<Cell> cells)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var lenght = (demands.OrderByDescending(d => d.Date).FirstOrDefault()?.Date.DayNumber ?? today.DayNumber) - today.DayNumber;
        DateOnly crrDate = today.AddDays(1);
        List<Day> Schedule = [];

        //LOOP - (DIAS ATÉ O ÚLTIMO PRAZO)
        for (int i = 0; i < lenght; i++)
        {

            //---------------------LOG-
            Console.WriteLine("\n" + new string('-', 5 + cells.Count * 20) + "\n" + crrDate);
            //---------------------LOG-
            List<List<Cell>> setups = [];

            //LOOP - (TURNOS DURANTE O DIA)
            for (int j = 0; j < SHIFTS; j++)
            {
                //---------------------LOG-
                Console.Write((j + 1) + "° | ");
                //---------------------LOG-

                List<Cell> shiftSetup = [];

                // //ORDENA POR PRIORIDADE DAS DEMANDAS NESSE TURNO EM QUESTÃO
                // var greedy = demands
                //     .Where(d => d.Quantity > 0)
                //     .OrderBy(d =>
                //         {
                //             //CALCULA O "CRITICAL RATIO", QUANTO MENOR MAIS URGENTE, PORTANTO ORDENAMOS POR ELE
                //             float available = (d.Date.DayNumber - crrDate.DayNumber) * (SHIFTS - j);
                //             float needed = d.Quantity * d.Product.Time / 60.0f / SHIFTS;
                //             return (float)(available / needed);
                //         }
                //     )
                // .ToList();

                // if (!greedy.Any())
                //     return Schedule;

                //LOOP - (CELULAS)
                for (int k = 0; k < cells.Count; k++)
                {
                    //ORDENA POR PRIORIDADE DAS DEMANDAS NESSE TURNO EM QUESTÃO
                    var greedy = demands
                        .Where(d => d.Quantity > 0)
                        .OrderBy(d =>
                            {
                                //CALCULA O "CRITICAL RATIO", QUANTO MENOR MAIS URGENTE, PORTANTO ORDENAMOS POR ELE
                                float available = (d.Date.DayNumber - crrDate.DayNumber) * (SHIFTS - j);
                                float needed = d.Quantity * d.Product.Time / 60.0f / SHIFTS;
                                return (float)(available / needed);
                            }
                        )
                    .ToList();

                    if (greedy.Count == 0)
                        return Schedule;
                    //---------------------LOG-
                    Cell c = cells[k];
                    Console.Write($"{"[" + c.Name + "] -> ",-10}");
                    string str = "";
                    //--------------------LOG-

                    List<Product> products = [];
                    int clogged = greedy.First().Product.Time;
                    Demand? crrDemand = greedy.First();
                    //LOOP - (LADOS DE PRODUÇÃO)
                    for (int l = 0; l < CORES_PER_CELL; l++)
                    {
                        //CALCULA QUANTAS PEÇAS VAO SER PRODUZIDAS DURANTE ESSE TURNO
                        int produced = (int)(HOURS_PER_SHIFT_OEE*60 / clogged);
                        //SUBTRAI A PRODUÇÃO DA NECESSIDADE DA DEMANDA PARA O PROXIMO TURNO CONSIDERAR SÓ O QUE FALTA
                        crrDemand.Quantity -= produced;
                        products.Add(crrDemand.Product);

                        //--------------------LOG-
                        str += crrDemand.Product.Name + " ";
                        //--------------------LOG-

                        //SE ACABOU A QUANTIDADE NECESSARIA DESSA DEMANDA AVANÇA PRA PRÓXIMA
                        if (crrDemand.Quantity <= 0)
                        {
                            crrDemand = greedy
                                .Where(g => g.Quantity > 0)
                                .Where(g => g.Product.Time <= crrDemand.Product.Time)
                                .FirstOrDefault();

                            if (crrDemand == null)
                                break;
                        }
                    }

                    //--------------------LOG-
                    Console.Write($"{str,-(CORES_PER_CELL * 2 + CORES_PER_CELL + 4)}");
                    //--------------------LOG-
                    Cell setup = new(c.Id, c.Name, products);
                    shiftSetup.Add(setup);
                }
                setups.Add(shiftSetup);
                //--------------------LOG-
                Console.WriteLine();
                //--------------------LOG-
            }
            //--------------------LOG-
            Console.Write("F  | ");
            foreach (var d in demands)
                Console.Write(d.Product.Name + ": " + d.Quantity + "    ");
            //--------------------LOG-
            Schedule.Add(new(crrDate, setups));
            crrDate = crrDate.AddDays(1);
        }

        return Schedule;
    }
}