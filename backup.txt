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

                //PEGA O ITERATOR PARA PODER AVANÇAR ENTRE AS DEMANDAS QUANDO ENCERRAR ALGUMA DURANTE O TURNO
                var it = greedy.GetEnumerator();
                while (it.MoveNext() && it.Current.Quantity <= 0) ; //PROCURA A PRIMEIRA DEMANDA VALIDA OU DEFAULT SE TIVER ACABADO
                if (it.Current == null)
                    return Schedule;        //SE DEFAULT, PORTANTO ACABOU AA DEMENDAS E ENCERRA O CALCULO
                //LOOP - (CELULAS)
                for (int k = 0; k < cells.Count; k++)
                {
                    //---------------------LOG-
                    Cell c = cells[k];
                    Console.Write($"{"[" + c.Name + "] -> ",-10}");
                    string str = "";
                    //--------------------LOG-

                    List<Product> products = [];
                    Product product = it.Current.Product;
                    //LOOP - (LADOS DE PRODUÇÃO)
                    for (int l = 0; l < CORES_PER_CELL; l++)
                    {
                        //CALCULA QUANTAS PEÇAS VAO SER PRODUZIDAS DURANTE ESSE TURNO
                        int produced = (int)(HOURS_PER_SHIFT_OEE / (it.Current.Product.Time / 60.0f));
                        //SUBTRAI A PRODUÇÃO DA NECESSIDADE DA DEMANDA PARA O PROXIMO TURNO CONSIDERAR SÓ O QUE FALTA
                        it.Current.Quantity -= produced;
                        products.Add(it.Current.Product);

                        //--------------------LOG-
                        str += it.Current.Product.Name + " ";
                        //--------------------LOG-

                        //SE ACABOU A QUANTIDADE NECESSARIA DESSA DEMANDA AVANÇA PRA PRÓXIMA
                        if (it.Current.Quantity <= 0)
                        {
                            it.MoveNext(); // SEGUE PRA PROXIMA PRIORIDADE
                            if (it.Current == null) //VALIDA SE ACABOU AS DEMANDAS
                                return Schedule;
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
            Schedule.Add(new Day(crrDate, setups));
            crrDate = crrDate.AddDays(1);
        }

        return Schedule;
    }
}