using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SimplexLib
{
    public class Table
    {
        private Matrix matrix;

        private List<Fraction>
            basis; //тот базис который нам нужен.//список x* - угловой точки ,где будут содержаться коэфиценты базисных переменных ,остальные будут равны нулю 
        
        
        // private List<Fraction> basisVariables; 

        //private List<Fraction> freeVariables = new List<Fraction>(); //список свободных коэф


        public List<Fraction> function = new List<Fraction>(); //функция из коэфициэнтов.

        public bool Answer = false; //Есть ли ответ или нет

        public bool NoAnswer = false; //Есть ли ошибки или беск.колво решений.
        public int MainColumn = 0;
        public int MainRow = 0;

        public bool ArtificialOver = false; //Есть ли ответ или нет
        
        Dictionary<int, List<int>> r_and_c = new Dictionary<int, List<int>>();


        Dictionary<int, int> position_x = new Dictionary<int, int>();

        /// <summary>
        /// /////////////////////////////////Симлпекс Таблица и Искуссвенный базис Таблица///////////////////////////////////////////////
        /// </summary>
        public List<int> IndexFreeVariables = new List<int>(); // Индексы свободных переменных
        public List<int> IndexBasisVariables = new List<int>(); // Индексы базисных переменных
        public List<List<Fraction>> TableSimplex = new List<List<Fraction>>(); //Симплекс таблица
        public List<Fraction> lastStroka = new List<Fraction>(); //последняя строка ,симплекс таблицы, функция.
        
        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        public Table(List<Fraction> Basis, bool isBasis, List<Fraction> Function, Matrix Matrix, bool Artif)
        {
            matrix = Matrix;
            basis = Basis;
            function = Function;
            
            if (!Artif)
            {
                InitIndexVariables();
                if (isBasis) //если базис есть (не нулевой)
                {
                    MatrixWithBasis();
                    var basis1 = new List<Fraction>();
                    foreach (var indexB in IndexBasisVariables)
                    {
                        basis1.Add(basis[indexB]);
                    }

                    foreach (var index in IndexFreeVariables)
                    {
                        basis1.Add(new Fraction(0));
                    }

                    matrix.GaussMethod(basis1);
                    ChangeMatrixToDefault();

                    Console.WriteLine(matrix.ToString());
                }
                else
                {
                    matrix.GaussMethod(basis);
                }

                basis = new List<Fraction>();
                InitBasis(); //инициализирует базис
                //инициализируетИндексыСвободныхИбазисныхПеременных
                InitLastStroka(); //инициализируем последнюю строку таблицы, которая содержит коэф функции
                InitTableSimplex();
            }

            if (Artif)
            {
                InitTableArtificial();
            }
            
        }
        
        /// <summary>
        /// Пошаговый симплекс метод
        /// </summary>
        /// <param name="next"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public void StepSimplexTable(bool next, int row, int column)
        {
            List<Fraction> HelpVector = new List<Fraction>();
            List<Fraction> Multi = new List<Fraction>();
            Fraction valueMain = new Fraction(0);

            MainColumn = column;
            MainRow = row;

            if (next)
            {
                MainColumn = findMainCol();
                Console.WriteLine(MainColumn);
                MainRow = findMainRow(MainColumn);
                Console.WriteLine(MainRow);
            }

            SearchAllMainElement(); //Поиск всех опорных элементов
            if (!next) //Если выбор не автоматический.
            {
                MainColumn = -1;
                MainRow = -1;
                foreach (var i in r_and_c[row])
                {
                    if (i == column)
                    {
                        MainColumn = column;
                        MainRow = row;
                        break;
                    }
                }
            }

            if (Answer || MainColumn == -1)
                return;
            if (NoAnswer) // Когда бесконечное кол-во решений или ответа нет!.
            {
                return;
            }


            valueMain = TableSimplex[MainRow][MainColumn];

            Console.WriteLine($"Row={MainRow}, MainColumn = {MainColumn}, MainElement= {valueMain}");

            //Создание вспомогательного вектора
            for (var i = 0; i < TableSimplex[0].Count; i++)
            {
                if (i != MainColumn)
                    HelpVector.Add(TableSimplex[MainRow][i] / valueMain);
                else
                {
                    HelpVector.Add(new Fraction(0));
                }
            }

            //Создание мульти вектора
            for (var i = 0; i < TableSimplex.Count; i++)
            {
                if (i != MainRow)
                    Multi.Add(TableSimplex[i][MainColumn]);
                else
                {
                    Multi.Add(new Fraction(0));
                }
            }


            //Cоздание новой таблицы
            List<List<Fraction>> NewTableSimplex = new List<List<Fraction>>();
            List<Fraction> newLastStroka = new List<Fraction>();


            for (var i = 0; i < TableSimplex.Count; i++)
            {
                List<Fraction> list = new List<Fraction>();
                for (var j = 0; j < TableSimplex[0].Count; j++)
                {
                    if (j == MainColumn && i == MainRow)
                    {
                        list.Add(1 / valueMain); //если обрабатывается опорный элемент
                        continue;
                    }

                    if (i == MainRow) //если обрабатывается опорная строка
                    {
                        list.Add(HelpVector[j]);
                        continue;
                    }

                    if (j == MainColumn) //если обрабатывается опорная строка
                    {
                        list.Add(-Multi[i] / valueMain);
                        continue;
                    }

                    list.Add(TableSimplex[i][j] - (Multi[i] * HelpVector[j]));
                }

                NewTableSimplex.Add(list);
            }

            for (var j = 0; j < TableSimplex[0].Count; j++)
            {
                if (j == MainColumn) //если обрабатывается опорная строка
                {
                    newLastStroka.Add(-lastStroka[MainColumn] / valueMain);
                    continue;
                }

                newLastStroka.Add(lastStroka[j] - (lastStroka[MainColumn] * HelpVector[j]));
            }

            //Меняем базисные индексы и сам базис
            int val = IndexBasisVariables[MainRow];
            IndexBasisVariables[MainRow] = IndexFreeVariables[MainColumn];
            IndexFreeVariables[MainColumn] = val;
            basis = new List<Fraction>();
            for (int i = 0; i < function.Count; i++)
            {
                basis.Add(new Fraction(0));
            }

            int k = 0;
            foreach (var indexBasisVariable in IndexBasisVariables)
            {
                basis[indexBasisVariable] = NewTableSimplex[k][NewTableSimplex[0].Count - 1];
                k++;
            }
            ////////////////////////////////////////


            //Меняем значения таблицы...
            TableSimplex = NewTableSimplex;
            lastStroka = newLastStroka;
        }


        public void StepArtificialTable(bool next, int row, int column)
        {
            List<Fraction> HelpVector = new List<Fraction>();
            List<Fraction> Multi = new List<Fraction>();
            Fraction valueMain = new Fraction(0);
            if (ArtificialOver)
                StepSimplexTable(next,row,column);
            MainColumn = column;
            MainRow = row;

            if (next)
            {
                MainColumn = findMainCol();
                Console.WriteLine(MainColumn);
                MainRow = findMainRow(MainColumn);
                Console.WriteLine(MainRow);
            }

            Answer = false;
            SearchAllMainElement(); //Поиск всех опорных элементов
            if (!next) //Если выбор не автоматический.
            {
                MainColumn = -1;
                MainRow = -1;
                foreach (var i in r_and_c[row])
                {
                    if (i == column)
                    {
                        MainColumn = column;
                        MainRow = row;
                        break;
                    }
                }
            }

            if (MainColumn == -1 || ArtificialOver)
            {
                lastStroka = new List<Fraction>();
                InitLastStroka1();
                //lastStroka[lastStroka.Count - 1] = -lastStroka[lastStroka.Count - 1];
                /*for (var i = 0; i < lastStroka.Count; i++)
                {
                    lastStroka[i] *= -1;
                }*/
                return;
            }
            if (NoAnswer) // Когда бесконечное кол-во решений или ответа нет!.
            {
                return;
            }


            valueMain = TableSimplex[MainRow][MainColumn];

            Console.WriteLine($"Row={MainRow}, MainColumn = {MainColumn}, MainElement= {valueMain}");

            //Создание вспомогательного вектора
            for (var i = 0; i < TableSimplex[0].Count; i++)
            {
                if (i != MainColumn)
                    HelpVector.Add(TableSimplex[MainRow][i] / valueMain);
                else
                {
                    HelpVector.Add(new Fraction(0));
                }
            }

            //Создание мульти вектора
            for (var i = 0; i < TableSimplex.Count; i++)
            {
                if (i != MainRow)
                    Multi.Add(TableSimplex[i][MainColumn]);
                else
                {
                    Multi.Add(new Fraction(0));
                }
            }


            //Cоздание новой таблицы
            List<List<Fraction>> NewTableSimplex = new List<List<Fraction>>();
            List<Fraction> newLastStroka = new List<Fraction>();


            for (var i = 0; i < TableSimplex.Count; i++)
            {
                List<Fraction> list = new List<Fraction>();
                for (var j = 0; j < TableSimplex[0].Count; j++)
                {
                    if (j == MainColumn && i == MainRow)
                    {
                        list.Add(1 / valueMain); //если обрабатывается опорный элемент
                        continue;
                    }

                    if (i == MainRow) //если обрабатывается опорная строка
                    {
                        list.Add(HelpVector[j]);
                        continue;
                    }

                    if (j == MainColumn) //если обрабатывается опорная строка
                    {
                        list.Add(-Multi[i] / valueMain);
                        continue;
                    }

                    list.Add(TableSimplex[i][j] - (Multi[i] * HelpVector[j]));
                }

                NewTableSimplex.Add(list);
            }

            for (var j = 0; j < TableSimplex[0].Count; j++)
            {
                if (j == MainColumn) //если обрабатывается опорная строка
                {
                    newLastStroka.Add(-lastStroka[MainColumn] / valueMain);
                    continue;
                }

                newLastStroka.Add(lastStroka[j] - (lastStroka[MainColumn] * HelpVector[j]));
            }

            //Меняем базисные индексы и сам базис
            int val = IndexBasisVariables[MainRow];
            IndexBasisVariables[MainRow] = IndexFreeVariables[MainColumn];
            IndexFreeVariables[MainColumn] = val;
            int v = basis.Count;
            basis = new List<Fraction>();
            for (int i = 0; i < v; i++)
            {
                basis.Add(new Fraction(0));
            }

            
            int k = 0;
            foreach (var indexBasisVariable in IndexBasisVariables)
            {
                basis[indexBasisVariable] = NewTableSimplex[k][NewTableSimplex[0].Count - 1];
                k++;
            }
            ////////////////////////////////////////
            //Меняем значения таблицы...
            TableSimplex = NewTableSimplex;
            lastStroka = newLastStroka;
            if (val >= function.Count) //если это дополнительная переменная (искусственная) , то удаляем столбец в таблице с ней  .
                    DeleteColumn(MainColumn);
        }
        /*public void StepArtificialTable(bool next)
        {
            if (ArtificialOver)
                StepSimplexTable(next, row, column);
            List<Fraction> HelpVector = new List<Fraction>();
            List<Fraction> Multi = new List<Fraction>();
            Fraction valueMain = new Fraction(0);

            MainColumn = column;
            MainRow = row;

            if (next)
            {
                MainColumn = findMainCol();
                Console.WriteLine(MainColumn);
                MainRow = findMainRow(MainColumn);
                Console.WriteLine(MainRow);
            }

            SearchAllMainElement(); //Поиск всех опорных элементов
            if (!next) //Если выбор не автоматический.
            {
                MainColumn = -1;
                MainRow = -1;
                foreach (var i in r_and_c[row])
                {
                    if (i == column)
                    {
                        MainColumn = column;
                        MainRow = row;
                        break;
                    }
                }
            }

            if (MainColumn == -1 || ArtificialOver)
            {
                lastStroka = new List<Fraction>();
                InitLastStroka();
                lastStroka[lastStroka.Count - 1] = -lastStroka[lastStroka.Count - 1];
                return;
            }
                
            if (NoAnswer) // Когда бесконечное кол-во решений или ответа нет!.
            {
                return;
            }


            valueMain = TableSimplex[MainRow][MainColumn];

            Console.WriteLine($"Row={MainRow}, MainColumn = {MainColumn}, MainElement= {valueMain}");

            //Создание вспомогательного вектора
            for (var i = 0; i < TableSimplex[0].Count; i++)
            {
                if (i != MainColumn)
                    HelpVector.Add(TableSimplex[MainRow][i] / valueMain);
                else
                {
                    HelpVector.Add(new Fraction(0));
                }
            }

            //Создание мульти вектора
            for (var i = 0; i < TableSimplex.Count; i++)
            {
                if (i != MainRow)
                    Multi.Add(TableSimplex[i][MainColumn]);
                else
                {
                    Multi.Add(new Fraction(0));
                }
            }


            //Cоздание новой таблицы
            List<List<Fraction>> NewTableSimplex = new List<List<Fraction>>();
            List<Fraction> newLastStroka = new List<Fraction>();


            for (var i = 0; i < TableSimplex.Count; i++)
            {
                List<Fraction> list = new List<Fraction>();
                for (var j = 0; j < TableSimplex[0].Count; j++)
                {
                    if (j == MainColumn && i == MainRow)
                    {
                        list.Add(1 / valueMain); //если обрабатывается опорный элемент
                        continue;
                    }

                    if (i == MainRow) //если обрабатывается опорная строка
                    {
                        list.Add(HelpVector[j]);
                        continue;
                    }

                    if (j == MainColumn) //если обрабатывается опорная строка
                    {
                        list.Add(-Multi[i] / valueMain);
                        continue;
                    }

                    list.Add(TableSimplex[i][j] - (Multi[i] * HelpVector[j]));
                }

                NewTableSimplex.Add(list);
            }

            for (var j = 0; j < TableSimplex[0].Count; j++)
            {
                if (j == MainColumn) //если обрабатывается опорная строка
                {
                    newLastStroka.Add(-lastStroka[MainColumn] / valueMain);
                    continue;
                }

                newLastStroka.Add(lastStroka[j] - (lastStroka[MainColumn] * HelpVector[j]));
            }

            
            
            //Меняем базисные индексы и сам базис и удаляем лишний стобец
            int val = IndexBasisVariables[MainRow];//
            IndexBasisVariables[MainRow] = IndexFreeVariables[MainColumn];
            IndexFreeVariables[MainColumn] = val;
            
            List<Fraction> newbasis = new List<Fraction>();
            for (int i = 0; i < basis.Count; i++)
            {
                newbasis.Add(new Fraction(0));
            }
            
            
            TableSimplex = NewTableSimplex;
            lastStroka = newLastStroka;
            
           
            
            int k = 0;
            foreach (var indexBasisVariable in IndexBasisVariables)
            {
                newbasis[indexBasisVariable] = TableSimplex[k][TableSimplex[0].Count - 1];
                k++;
            }

            basis = newbasis;
            
            if (val >= function.Count) //если это дополнительная переменная (искусственная) , то удаляем столбец в таблице с ней  .
                DeleteColumn(MainColumn);//удаляется этот столбец.
            ////////////////////////////////////////


            //Меняем значения таблицы...

        }*/


        public void SearchAllMainElement() //Поиск всех опорных элементов в таблице
        {
            List<int> fractions = new List<int>();
            
            r_and_c = new Dictionary<int, List<int>>();
            for (int i = 0; i < basis.Count; i++)
            {
                r_and_c.Add(i,new List<int>());
            }
            
            for (var i = 0; i < lastStroka.Count; i++)
            {
                if (lastStroka[i] < 0)
                    r_and_c[findMainRow(MainColumn)].Add(i);
            }
        }

        private int findMainCol()
        {
            int mainCol = 0;
            for (int j = 1; j < lastStroka.Count - 1; j++)
            {
                if (lastStroka[j] < lastStroka[mainCol])
                    mainCol = j;
            }
            if (lastStroka[mainCol] > 0)
                Answer = true;
            else
            {
                Answer = false;
            }
            ArtificialOver = true;
            for (int j = 0; j < lastStroka.Count; j++)
            {
                if (lastStroka[j] != 0)
                {
                    ArtificialOver = false;
                    break;
                }
            }
            return mainCol;
        }

        private int findMainRow(int mainCol)
        {
            int mainRow = -1;

            int last = TableSimplex[0].Count - 1;

            for (int i = 0; i < TableSimplex.Count; i++)
                if (TableSimplex[i][mainCol] > 0)
                {
                    mainRow = i;
                    break;
                }

            if (mainRow == -1)
            {
                NoAnswer = true;
            }

            for (int i = mainRow + 1; i < TableSimplex.Count; i++)
                if ((TableSimplex[i][mainCol] > 0) && ((TableSimplex[i][last] / TableSimplex[i][mainCol]) <
                                                       (TableSimplex[mainRow][last] / TableSimplex[mainRow][mainCol])))
                    mainRow = i;

            return mainRow;
        }

        private void DeleteColumn(int Column)
        {
            for (var i = 0; i < TableSimplex.Count; i++)
            {
                TableSimplex[i].RemoveAt(Column);
            }
            lastStroka.RemoveAt(Column);
            basis.RemoveAt(IndexFreeVariables[Column]);
           
            
            for (var i = 0; i < IndexBasisVariables.Count; i++)
            {
                if (IndexFreeVariables[Column] < IndexBasisVariables[i])
                    IndexBasisVariables[i]-=1;
            }

            IndexFreeVariables.RemoveAt(Column);
            
        }


        #region InitFunction -функции, которые инициализируют поля
        public void InitTableArtificial()
        {
            basis = new List<Fraction>();
            InitBasis();
            InitIndexVariables();
            InitTableSimplex();
            InitLastStrokaArtificial();
            Console.WriteLine(ToString());
        }
        public void InitTableSimplex()
        {
            for (var i = 0; i < IndexBasisVariables.Count; i++)
            {
                TableSimplex.Add(OutStrTable(i));
            }
        }
        
        //Инициализируется последняя строка
        public void InitLastStroka()
        {
            Fraction temp = new Fraction(0);
            foreach (var i in IndexFreeVariables)
            {
                for (var j = 0; j < matrix.values.Count; j++)
                {
                    temp += function[IndexBasisVariables[j]] * matrix.values[j][i];
                }

                temp -= function[i];
                lastStroka.Add(temp);
                temp = new Fraction(0);
            }

            int t = 0;
            foreach (var j in IndexBasisVariables)
            {
                temp += function[j] * basis[j];
                t++;
            }

            lastStroka.Add(temp);
            temp = new Fraction(0);
            if (lastStroka.Count == 0)
            {
                for (var i = 0; i < basis.Count; i++)
                {
                    temp += function[i] * basis[i];
                }

                lastStroka.Add(temp);
            }
        }
        
        public void InitLastStroka1()
        {
            Fraction temp = new Fraction(0);
            for (var i = 0; i < IndexFreeVariables.Count; i++)
            {
                for (var j = 0; j < TableSimplex.Count; j++)
                {
                    temp += function[IndexBasisVariables[j]] * TableSimplex[j][i];
                }

                temp -= function[IndexFreeVariables[i]];
                lastStroka.Add(temp);
                temp = new Fraction(0);
            }
            
            int t = 0;
            foreach (var j in IndexBasisVariables)
            {
                temp += function[j] * basis[j];
                t++;
            }

            lastStroka.Add(temp);
            temp = new Fraction(0);
            if (lastStroka.Count == 0)
            {
                for (var i = 0; i < basis.Count; i++)
                {
                    temp += function[i] * basis[i];
                }

                lastStroka.Add(temp);
            }
        }

        public void InitLastStrokaArtificial()
        {
            Fraction temp = new Fraction(0);
            for (var i = 0; i < TableSimplex[0].Count; i++)
            {
                for (int j = 0; j < TableSimplex.Count; j++)
                {
                    temp += TableSimplex[j][i];
                }
                lastStroka.Add(-temp);
                temp = new Fraction(0);
            }
        }

        public void InitBasis()
        {
            Fraction variable = new Fraction(0);
            for (int i = 0; i < matrix.values[0].Count - 1; i++)
            {
                for (int j = 0; j < matrix.values.Count; j++)
                {
                    if (matrix.values[j][i] == new Fraction(0))
                        continue;
                    if (matrix.values[j][i] == new Fraction(1) && variable == new Fraction(0))
                    {
                        variable = matrix.values[j][matrix.values[0].Count - 1];
                        continue;
                    }
                    else if (matrix.values[j][i] == new Fraction(1) && variable != new Fraction(0))
                    {
                        variable = new Fraction(0);
                        break;
                    }

                    if (matrix.values[j][i] != new Fraction(0))
                    {
                        variable = new Fraction(0);
                        break;
                    }
                }

                basis.Add(variable);
                variable = new Fraction(0);
            }
        }
        
        
        /// <summary>
        /// Инициализирует индексы переменных ,которые принадлежат свободных и базисным переменным.
        /// </summary>
        public void InitIndexVariables()
        {
            for (int i = 0; i < basis.Count; i++)
            {
                if (basis[i] != 0)
                    IndexBasisVariables.Add(i);
                else
                {
                    IndexFreeVariables.Add(i);
                }
            }
        }
        
        #endregion
        

        /// <summary>
        /// Подготавливает матрицу к Методу гаусса, сдвигает в матрице столбцы , под базис x* = (j0,j1,...jn,0,0,....,0*m), где n кол-во базисных переменных, а m - общее кол-во переменных.
        /// </summary>
        public void MatrixWithBasis()
        {
            for (var i = 0; i < basis.Count; i++)
            {
                position_x[i] = i;
            }

            int count = 0;
            foreach (var indexBasis in IndexBasisVariables)
            {
                SwitchColumn(count, IndexBasisVariables[count]);
                count++;
            }

            foreach (var VARIABLE in position_x)
            {
                Console.WriteLine(VARIABLE);
            }
        }

        /// <summary>
        /// Восстанавливает матрицу
        /// </summary>
        public void ChangeMatrixToDefault()
        {
            for (int i = 0; i < basis.Count; i++)
            {
                foreach (var VARIABLE in position_x)
                {
                    if (VARIABLE.Value == i)
                    {
                        SwitchColumn(i, VARIABLE.Key);
                        break;
                    }
                }
            }

            foreach (var VARIABLE in position_x)
            {
                Console.WriteLine(VARIABLE);
            }
        }

        /// <summary>
        /// меняет местами a и b столбец
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void SwitchColumn(int a, int b)
        {
            Fraction temp = new Fraction(0);
            for (var i = 0; i < matrix.values.Count; i++)
            {
                temp = matrix.values[i][a];
                matrix.values[i][a] = matrix.values[i][b];
                matrix.values[i][b] = temp;
            }

            var t = position_x[a];
            position_x[a] = position_x[b];
            position_x[b] = t;
        }
        
        /// <summary>
        /// Выводит лист коэфицентов свободных переменных и в конце значения базисной переменной. Проще говоря выводит по индексу преобразованную строчку таблицы , в виде листа из коэф
        /// </summary>
        /// <returns></returns>
        public List<Fraction> OutStrTable(int IndexBasis)
        {
            List<Fraction> temp = new List<Fraction>();
            foreach (var element in IndexFreeVariables)
            {
                temp.Add(matrix.values[IndexBasis][element]);
            }

            temp.Add(matrix.values[IndexBasis][matrix.values[0].Count - 1]);
            return temp;
        }

        public List<Fraction> OutTable(int IndexBasis)
        {
            List<Fraction> temp = new List<Fraction>();

            foreach (var VARIABLE in TableSimplex[IndexBasis])
            {
                temp.Add(VARIABLE);
            }

            return temp;
        }


        //Строковое представление таблицы 
        public override string ToString()
        {
            return "Симплекс таблица: \n" +
                   $"Угловая точка x*: = {StrBasis()}\n" +
                   $"Table:\n" +
                   $"{StrTable()}\n" +
                   $"{StrFunction()}";
        }
        
        #region Cтроковые вспомогательные методы:

        /// <summary>
        /// Выводит строковое представление таблицы
        /// </summary>
        /// <returns></returns>
        public string StrTable()
        {
            string temp = "__|" + StrFreeVariable() + " |\n";
            int i = 0;
            foreach (var indexBasis in IndexBasisVariables)
            {
                temp += $"{indexBasis + 1}x|";
                foreach (var element in OutTable(i))
                {
                    temp += $"{element}|";
                }

                i++;
                temp += "\n";
            }

            //TODO доделать вывод функции ,чтобы выводилась последняя строка...
            temp += "f|";
            foreach (var t in lastStroka)
                temp += $"{t}|";

            return temp;
        }


        /// <summary>
        /// Возращает строковое предстваление базиса
        /// </summary>
        /// <returns></returns>
        public string StrBasis()
        {
            string temp = "";

            foreach (var element in IndexBasisVariables)
            {
                temp += $"x{element + 1}={basis[element]};";
            }

            return temp;
        }


        /// Возращает строковое предстваление свободных переменных
        /// </summary>
        /// <returns></returns>
        public string StrFreeVariable()
        {
            string temp = "";

            foreach (var element in IndexFreeVariables)
            {
                temp += $"x{element + 1}|";
            }

            return temp;
        }


        /// <summary>
        /// Возращает строковое представление свободных переменных...
        /// </summary>
        /// <returns></returns>
        public string StrFunction()
        {
            string temp = "f=";


            for (var i = 0; i < function.Count; i++)
            {
                if (function[i] != 0)
                {
                    if (function[i].Sign == 1)
                        temp += "+";
                    temp += $"{function[i]}x";
                }
            }

            return temp;
        }

        #endregion
        
    }

    /*public class Table : ICloneable
    {
        private Matrix matrix;
        private List<Fraction> basis;
        
        private List<int> basisVariables;
        private List<int> freeVariables;
        
        private List<Fraction> answer;
        private List<Fraction> function;
        
        public bool isAnswer;
        public bool isArtifical;
        public List<bool> ArtificalBasis;
        private List<Fraction> old_function;
        int kindOfAnswer = -1;

        internal List<Fraction> Answer
        {
            get => answer;
            set => answer = value;
        }

        internal Matrix Matrix
        {
            get => matrix;
            set => matrix = value;
        }

        public List<int> BasisVariables
        {
            get => basisVariables;
            set => basisVariables = value;
        }

        public List<int> FreeVariables
        {
            get => freeVariables;
            set => freeVariables = value;
        }

        public List<Fraction> Function
        {
            get => function;
            set => function = value;
        }

        public List<Fraction> Basis
        {
            get => basis;
            set => basis = value;
        }

        public int KindOfAnswer
        {
            get => kindOfAnswer;
            set => kindOfAnswer = value;
        }

        public List<Fraction> Old_function
        {
            get => old_function;
            set => old_function = value;
        }

        public Table()
        {
        }

        /// <summary>
        /// Конструктор симлекс-таблицы
        /// </summary>
        /// <param name="table1">таблица</param>
        /// <param name="isFirst">первый</param>
        /// <param name="artifical">сущ иск базис</param>
        public Table(Table table1, bool isFirst, bool artifical)
        {
            int i;
            Fraction l;
            matrix = table1.matrix;
            basis = table1.basis;
            function = table1.function;
            old_function = table1.old_function;
            basisVariables = table1.basisVariables;
            freeVariables = table1.freeVariables;
            kindOfAnswer = table1.kindOfAnswer;
            answer = table1.answer;
            ArtificalBasis = table1.ArtificalBasis;
            if (artifical)
            {
                isArtifical = true;
                if (isFirst)
                {
                    ArtificalBasis = new List<bool>();
                    isArtifical = true;
                    for (i = 0; i < matrix.values[0].Count - 1 - matrix.values.Count; i++)
                    {
                        ArtificalBasis.Add(false);
                    }

                    for (int j = i; j < matrix.values[0].Count - 1; j++)
                    {
                        ArtificalBasis.Add(true);
                    }
                }
            }

            isAnswer = true;

            List<List<Fraction>> values = matrix.values;

            if (isFirst)
            {
                answer = new List<Fraction>();
                basisVariables = new List<int>();
                freeVariables = new List<int>();
                for (i = 0; i < basis.Count; i++)
                {
                    if (basis[i] != 0)
                    {
                        basisVariables.Add(i);
                    }
                    else freeVariables.Add(i);
                }

                for (i = 0; i < values.Count(); i++)
                {
                    for (int j = 0; j < values.Count(); j++)
                    {
                        values[i].Remove(values[i][0]);
                    }
                }

                values.Add(new List<Fraction>());

                answer = new List<Fraction>();
                for (i = 0; i < values[0].Count() - 1; i++)
                {
                    answer.Add(new Fraction(0));
                    for (int j = 0; j < values.Count - 1; j++)
                    {
                        Fraction z = values[j][i];
                        Fraction w = function[basisVariables[j]];
                        answer[i] += values[j][i] * function[basisVariables[j]];
                    }

                    l = new Fraction(this.function[freeVariables[i]].ToString() + "");
                    l.Sign *= -1;
                    answer[i] += l;
                    answer[i].Sign *= -1;
                }

                int m = i;
                answer.Add(new Fraction(0));
                for (int j = 0; j < values.Count - 1; j++)
                {
                    answer[m] += values[j][m] * this.function[basisVariables[j]];
                }

                answer[values[0].Count() - 1].Sign *= -1;

                values[values.Count - 1] = answer;
            }

            this.matrix = new Matrix(values);
            //MessageBox.Show(matrix.ToString());
        }

        /// <summary>
        /// Метод для пошагового обхода сиплекс метода.На вход подается индексы опорного элемента
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public Table StepSimplexMethod(int row, int col)
        {
            //вектор базисных переменных
            //ищем опорный элемент
            if (row == -1 && col == -1)
            {
                int j = 0, i = 0;
                col = 0;
                row = 0;
                Fraction min = new Fraction(-1);
                bool edge = true;
                for (i = 0; i < matrix.values[0].Count; i++)
                {
                    edge = true;
                    for (j = 0; j < matrix.values.Count; j++)
                    {
                        if (matrix.values[j][i] >= 0)
                        {
                            edge = false;
                            break;
                        }
                    }

                    if (edge)
                    {
                        isAnswer = true;
                        kindOfAnswer = 0;
                        return this;
                    }
                }

                col = FindMinCol();
                if (col == -1)
                {
                    isAnswer = true;
                    kindOfAnswer = 1;
                    return this;
                }

                //ищем минимальное соотношение в столбце
                row = FindRow(col);
                if (row == -1)
                {
                    isAnswer = true;
                    kindOfAnswer = 0;
                    return this;
                }
            }

            if (row != -1 && col != -1)
                isAnswer = false;
            if (row == -1 || col == -1)
            {
                isAnswer = true;
                kindOfAnswer = 0;
                return this;
            }

            //обработать строку
            List<List<Fraction>> table = new List<List<Fraction>>();
            List<Fraction> table_row;
            for (int i = 0; i < matrix.values.Count; i++)
            {
                table_row = new List<Fraction>();
                table.Add(table_row);
            }

            List<Fraction> support_row = new List<Fraction>();//вспомогательная строка
            for (int i = 0; i < matrix.values[row].Count; i++)
            {
                if (i == col)
                {
                    support_row.Add(1 / matrix.values[row][col]);
                    continue;
                }
                else
                {
                    support_row.Add(matrix.values[row][i] / matrix.values[row][col]);
                }
            }

            table[row] = support_row;
            //обработать все остальное
            for (int i = 0; i < matrix.values.Count; i++)
            {
                if (i == row)
                    continue;

                for (int j = 0; j < matrix.values[0].Count; j++)
                {
                    if (j == col)
                    {
                        Fraction z = new Fraction(matrix.values[row][col].ToString());
                        z.Sign *= -1;
                        Fraction m = matrix.values[i][j];
                        Fraction t = matrix.values[i][j] / z;
                        table[i].Add(t);
                        continue;
                    }

                    Fraction temp = matrix.values[i][col] * table[row][j];
                    temp -= matrix.values[i][j];
                    temp.Sign *= -1;
                    table[i].Add(temp);
                }
            }

            matrix = new Matrix(table);
            int k = basisVariables[row];
            basisVariables[row] = freeVariables[col];
            freeVariables[col] = k;


            //поменять местами переменные в базисе и свободную
            if (isArtifical && ArtificalBasis[freeVariables[col]])
            {
                for (int i = 0; i < matrix.values.Count; i++)
                    matrix.values[i].RemoveAt(col);
                freeVariables.RemoveAt(col);
            }

            return this;
        }

        public int FindMinCol()
        {
            bool isAnswer = true;
            int last_row = matrix.values.Count - 1;
            int min_col = -1, i = 0;
            int max = matrix.values[last_row].Count - 1;
            for (i = 0; i < max; i++)
            {
                if (min_col == -1)
                {
                    Fraction x = matrix.values[last_row][i];
                    Fraction m = new Fraction("-0");
                    if (matrix.values[last_row][i] < new Fraction("-0") && matrix.values[last_row][i].Numerator != 0)
                    {
                        min_col = i;
                        isAnswer = false;
                    }
                }
                else if (matrix.values[last_row][i] <= matrix.values[last_row][min_col] &&
                         matrix.values[last_row][i] < 0)
                {
                    Fraction m = matrix.values[last_row][i];
                    min_col = i;
                    isAnswer = false;
                }
            }

            if (isAnswer)
            {
                kindOfAnswer = 1;
                return -1;
            }

            isAnswer = true;
            for (i = 0; i < matrix.values.Count; i++)
            {
                if (matrix.values[i][min_col] > 0)
                {
                    isAnswer = false;
                    break;
                }
            }

            if (isAnswer)
            {
                kindOfAnswer = 0;
                return -1;
            }

            return min_col;
        }

        public int IsEndOfArtificialBasis()
        {
            int max_row = matrix.values.Count - 1;
            int max_col = matrix.values[max_row].Count - 1;
            for (int i = 0; i < max_col; i++)
            {
                if (matrix.values[max_row][i].Sign == -1)
                    return 0;
            }

            if (matrix.values[max_row][max_col].Numerator != 0)
            {
                return -1;
            }

            return 1;
        }

        public int FindRow(int col)
        {
            if (col == -1)
                return -1;
            int i, j, row = -1;
            Fraction min = new Fraction("-1");
            Fraction m = new Fraction();
            for (i = 0; i < matrix.values.Count - 1; i++)
            {
                if (matrix.values[i][col] > 0)
                {
                    if (min == -1)
                    {
                        min = matrix.values[i][matrix.values[i].Count - 1] / matrix.values[i][col];
                        row = i;
                    }
                    else
                    {
                        m = matrix.values[i][matrix.values[i].Count - 1] / matrix.values[i][col];
                        if (m < min && (m >= 0 || m.Numerator == 0))
                        {
                            min = m;
                            row = i;
                        }
                    }
                }
            }

            //for (j = i + 1; j < matrix.values.Count - 1; j++)
            //{
            //    if (matrix.values[j][matrix.values[i].Count - 1] / matrix.values[j][col] < min && matrix.values[j][matrix.values[i].Count - 1] / matrix.values[j][col] > 0)
            //    {
            //        min = matrix.values[j][matrix.values[i].Count - 1] / matrix.values[j][col];
            //        row = j;
            //    }
            //}
            return row;
        }

        public void MakeFunction()
        {
            int i;
            answer = new List<Fraction>();
            for (i = 0; i < matrix.values[0].Count() - 1; i++)
                //for (i = 0; i < freeVariables.Count() - 1; i++)
            {
                answer.Add(new Fraction(0));
                for (int j = 0; j < matrix.values.Count - 1; j++)
                {
                    Fraction z = matrix.values[j][i];
                    Fraction w = old_function[basisVariables[j]];
                    answer[i] += matrix.values[j][i] * old_function[basisVariables[j]];
                }

                Fraction l = new Fraction(this.old_function[freeVariables[i]].ToString() + "");
                l.Sign *= -1;
                answer[i] += l;
                answer[i].Sign *= -1;
            }

            int m = i;
            answer.Add(new Fraction(0));
            for (int j = 0; j < matrix.values.Count - 1; j++)
            {
                answer[m] += matrix.values[j][m] * this.old_function[basisVariables[j]];
            }

            answer[matrix.values[0].Count() - 1].Sign *= -1;
            matrix.values[matrix.values.Count - 1] = answer;
        }

        public object Clone()
        {
            return new Table
            {
                Answer = this.Answer,
                Matrix = this.Matrix,
                BasisVariables = this.BasisVariables,
                FreeVariables = this.FreeVariables,
                Function = this.Function,
                Basis = this.Basis,
                KindOfAnswer = this.KindOfAnswer,
                Old_function = this.Old_function,
                ArtificalBasis = this.ArtificalBasis,
                isAnswer = this.isAnswer,
                isArtifical = this.isArtifical
            };
        }
    }*/
}