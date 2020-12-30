using System;
using System.Collections.Generic;
using System.Linq;
using SimplexLib;

namespace ConsoleSimplex
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Кол-во ограничений min, max");
            
            List<Fraction> function = new List<Fraction>()
                {new Fraction(-5), new Fraction(4), new Fraction(-1), new Fraction(-3),new Fraction(-5)};
            string choise = "";
            //Если нужен минимум домнажаем на -1.
            Console.WriteLine("Выберите min, max");
            choise = Console.ReadLine();
            
            if(choise == "min")
            for (var i = 0; i < function.Count; i++)
            {
                function[i] *= -1;
            }
            
            List<List<Fraction>> matrx = new List<List<Fraction>>()
            {
                new List<Fraction>()
                {
                    new Fraction(1), new Fraction(3), new Fraction(1), new Fraction(2), new Fraction(5)
                },
                new List<Fraction>()
                {
                    new Fraction(2), new Fraction(0), new Fraction(-1), new Fraction(1), new Fraction(1)
                }
            };
            
            List<List<Fraction>> m = new List<List<Fraction>>()
            {
                new List<Fraction>()
                {
                    new Fraction(3), new Fraction(-1), new Fraction(0), new Fraction(2), new Fraction(1),new Fraction(1),new Fraction(0),new Fraction(0),new Fraction(5)
                },
                new List<Fraction>()
                {
                    new Fraction(2), new Fraction(-3), new Fraction(1), new Fraction(2), new Fraction(1),new Fraction(0),new Fraction(1),new Fraction(0),new Fraction(6)
                },
                new List<Fraction>()
                {
                    new Fraction(3), new Fraction(-1), new Fraction(1), new Fraction(3), new Fraction(2),new Fraction(0),new Fraction(0),new Fraction(1),new Fraction(9)
                }
            };

            Matrix matrix = new Matrix(matrx);
            Matrix matrix2 = new Matrix(m);
            
            List<Fraction> basis = new List<Fraction>
                {new Fraction(0), new Fraction(1), new Fraction(0), new Fraction(1)};
            
            List<Fraction> basis1 = new List<Fraction>
                {new Fraction(0), new Fraction(0), new Fraction(0), new Fraction(0), new Fraction(0),new Fraction(5),new Fraction(6),new Fraction(9)};
            
            Table table = new Table(basis,true, function, matrix,false);
            Table table1 = new Table(basis1,true, function, matrix2,true);
            int r, c;
            
            
            while (true)
            {
                if (table.Answer)
                {
                    Console.WriteLine(table1.ToString());
                    table1.StepArtificialTable(true, 0, 0);
                    break;
                }
                if (table.NoAnswer)
                {
                    table1.StepArtificialTable(true, 0, 0);
                    Console.WriteLine("нет решения");
                    break;
                }

                Console.WriteLine("next - 'n', любой символ выбрать опорный элемент самому");
                string text = Console.ReadLine();
                if (text.Equals("n"))
                {
                    table1.StepArtificialTable(true, 0, 0);
                    Console.WriteLine(table1.ToString());
                    continue;
                }
                
                r = Convert.ToInt32(Console.ReadLine());
                c= Convert.ToInt32(Console.ReadLine());
                table1.StepArtificialTable(false, r, c);
                Console.WriteLine(table1.ToString());

            }
            
            if(false)
            while (true)
            {
                if (table.Answer)
                {
                    Console.WriteLine(table.ToString());
                    table.StepSimplexTable(true, 0, 0);
                    break;
                }
                if (table.NoAnswer)
                {
                    table.StepSimplexTable(true, 0, 0);
                    Console.WriteLine("нет решения");
                    break;
                }

                Console.WriteLine("next - 'n', любой символ выбрать опорный элемент самому");
                string text = Console.ReadLine();
                if (text.Equals("n"))
                {
                    table.StepSimplexTable(true, 0, 0);
                    Console.WriteLine(table.ToString());
                    continue;
                }
                
                r = Convert.ToInt32(Console.ReadLine());
                c= Convert.ToInt32(Console.ReadLine());
                table.StepSimplexTable(false, r, c);
                Console.WriteLine(table.ToString());

            }
            
            
        }
    }
}