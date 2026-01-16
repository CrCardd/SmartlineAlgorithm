using Alg.DTO;
using Alg.Enums;
using Alg.Models;

public static class Algorithm
{
    private const UnitOfMeasure UOM = UnitOfMeasure.MINUTE; 
    private const int HOUR_PER_SHIFT = 8;
    private const float OEE = 0.70f; // BANHEIRO, DESCANSO, ETC (SEM CONSIDERAR TEMPO DE SETUP) - EM PORCENTAGEM DO TURNO
    private const float SETUP = 2.0f; // TEMPO QUE O SETUP LEVA - EM HORAS
    private const float HOURS_PER_SHIFT_OEE = HOUR_PER_SHIFT * OEE;
    private const int SHIFTS = 3;
    private const int CORES_PER_CELL = 2;


    public static List<Day> solver(List<Demand> demands, List<Cell> cells)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        DateOnly crrDate = today.AddDays(1);
        List<Day> Schedule = [];
        List<Log> Logs = [];

        //LOOP - (DIAS ATÉ O ÚLTIMO PRAZO)
        // var lenght = (demands.OrderByDescending(d => d.Date).FirstOrDefault()?.Date.DayNumber ?? today.DayNumber) - today.DayNumber;
        // for (int i = 0; i < lenght; i++)
        //LOOP - (ATÉ ENCERRAR TODAS AS DEMANDAS)
        while (demands.Any(d => d.Quantity > 0))
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

                //LOOP - (CELULAS)
                for (int k = 0; k < cells.Count; k++)
                {
                    //ORDENA POR PRIORIDADE DAS DEMANDAS NESSE TURNA NESSA MAQUINA, PARA ATUALIZARMOS CONFORME A PRODUÇÃO
                    var greedy = demands
                        .Where(d => d.Quantity > 0)
                        .OrderBy(d =>
                            {
                                //CALCULA O "CRITICAL RATIO", QUANTO MENOR MAIS URGENTE, PORTANTO ORDENAMOS POR ELE
                                // float available = ((d.Date.DayNumber - crrDate.AddDays(-1).DayNumber) * SHIFTS) + (SHIFTS - j);  // REVISAR
                                // float needed = d.Quantity * d.Product.Time / 60.0f / HOURS_PER_SHIFT_OEE;
                                // return (float)(available / needed);

                                //TALVEZ CONSIDERAR ORDENAR APENAS POR DATA
                                return d.Date;
                            }
                        )
                    .ToList();

                    if (greedy.Count == 0)
                        continue;
                        // return Schedule;

                    //---------------------LOG-
                    Cell c = cells[k];
                    Console.Write($"{"[" + c.Name + "] -> ",-10}");
                    string str = "";
                    //--------------------LOG-

                    List<Product> products = [];
                    int clogged = greedy.First().Product.Time; //SALVA O TEMPO DA DEMANDA PRIORITARIA, ELA QUE VAI DITAR O TEMPO DE PRODUÇÃO DESSA MÁQUINA
                    Demand? crrDemand = greedy.First(); //INICIA COM A PRIMEIRA DEMANDA, A MAIS URGENTE

                    //LOOP - (LADOS DE PRODUÇÃO)
                    for (int l = 0; l < CORES_PER_CELL; l++)
                    {
                        //CALCULA QUANTAS PEÇAS VAO SER PRODUZIDAS DURANTE ESSE TURNO
                        int produced = (int)(HOURS_PER_SHIFT_OEE * (int)UOM / clogged);

                        //           DEVE SER ANALISADO SE O ULTIMO PRODUTO QUE ESSA MÁQUINA PRODUZIU 
                        //           É O MESMO QUE VAI PRODUZIR AGORA, SE FOR, REMOVER DO OEE O TEMPO 
                        //           QUE SERIA GASTO COM UM SETUP, JA QUE NÃO SERÁ NECESSÁRIO ALTERAR 
                        //           O PRODUTO DA MÁQUINA.
                        //                            |
                        //                            |
                        //                           \/
                        // int produced = (int)(HOURS_PER_SHIFT_OEE * (int)UOM / clogged);


                        //SUBTRAI A PRODUÇÃO DA NECESSIDADE DA DEMANDA PARA O PROXIMO TURNO CONSIDERAR SÓ O QUE FALTA
                        crrDemand.Quantity -= produced;
                        products.Add(crrDemand.Product);

                        //--------------------LOG-
                        str += crrDemand.Product.Name + " ";
                        //--------------------LOG-

                        //SE ACABOU A QUANTIDADE NECESSARIA DESSA DEMANDA AVANÇA PRA PRÓXIMA
                        if (crrDemand.Quantity <= 0)
                        {
                            Logs.Add(new(crrDemand.Product, crrDemand.Date, crrDate, (crrDemand.Date.DayNumber - crrDate.DayNumber)));

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

        Console.WriteLine($"\n\n{"PRODUCT", -10} | {"DEADLINE", -10} | {"DELIVERED AT", -12} | {"STATUS", -6} | {"DISTANCE DAYS", -15}");
        Console.WriteLine(new string('-', 70));
        foreach (var l in Logs)
            Console.WriteLine($"{l.Product.Name, -10} | {l.Deadline, -10} | {l.DeliveredAt, -12} | {(l.DistanceDays < 0 ? "LATE" : l.DistanceDays > 0 ? "AHEAD" : "OK"), -6} | {l.DistanceDays, -15}");

        return Schedule;
    }
}