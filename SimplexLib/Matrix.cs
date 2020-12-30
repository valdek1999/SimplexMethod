using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SimplexLib
{
    public class Matrix : ICloneable
    {
        public List<List<Fraction>> values;//матрица которая представлена из дробных чисел
        //public List<List<Fraction>> basis;
        public bool isBasis;
        public bool isOnes;
        public Matrix()
        {
        }

        public Matrix(List<List<Fraction>> table)
        {
            this.values = table;
            
            
        }
        
        /// <summary>
        /// Методом гаусса на основе базиса решает матрицу
        /// </summary>
        /// <param name="basis"></param>
        /// <returns></returns>
        public int GaussMethod(List<Fraction> basis)
        {
            isBasis = false;
            isOnes = true;
            foreach (Fraction fraction in basis)//если каждый элемент базиса, в случае если нет базиса - то isBasise = false,isOnes = true
            {
                if (fraction != 0)//если какой-то элемент не равен нулю
                {
                    isBasis = true;
                    if (fraction != 1)
                        isOnes = false;
                }
            }

            if (isBasis)
            {
                //решается матрица , если нельзя привести к ступенчатому виду то возвращается -1, иначе 1 
                if (SolveMatrix(basis, isOnes) == -1)
                    return -1;
                else return 1;
            }
            else
            {
                if (SolveMatrix() == -1)
                    return -1;
                else return 1;
            }
        }
        
        /// <summary>
        /// поиск максимального элемента с сортировкой по строке
        /// </summary>
        /// <param name="SortIndex"></param>
        private void SortRows(int SortIndex)
        {
            Fraction MaxElement = values[SortIndex][SortIndex];
            int MaxElementIndex = SortIndex;
            for (int i = SortIndex + 1; i < values.Count(); i++)
            {
                if (values[i][SortIndex] > MaxElement)
                {
                    MaxElement = values[i][SortIndex];
                    MaxElementIndex = i;
                }
            }//ищет в стобце максимальный элемент

            //теперь найден максимальный элемент ставим его на верхнее место
            if (MaxElementIndex > SortIndex) //если это не первый элемент
            {
                for (int i = 0; i < values[0].Count(); i++)
                {
                    Fraction Temp = values[MaxElementIndex][i];
                    values[MaxElementIndex][i] = values[SortIndex][i];
                    values[SortIndex][i] = Temp;
                }
            }
        }

        /// <summary>
        /// Решения матрицы когда нет базиса
        /// </summary>
        /// <returns></returns>
        public int SolveMatrix()
        {
            for (int i = 0; i < Math.Min(values.Count(), values[0].Count); i++)
            {
                SortRows(i);
                Fraction m = values[i][i];
                for (int j = 0; j < values[0].Count; j++)
                {
                    values[i][j] /= m;
                }

                //прямой ход метода Гаусса
                for (int j = 0; j < values.Count(); j++)
                {
                    if (i != j && m != 0)
                    {
                        Fraction MultElement = values[j][i];
                        for (int k = i; k < values[0].Count(); k++)
                        {
                            values[j][k] -= values[i][k] * MultElement;
                        }
                    }
                }
            }
            
            int z = values[0].Count() - 1;
            for (int i = 0; i < values.Count() - 1; i++)
            {
                if (values[i][z].Sign == -1)
                    return -1;
            }

            //ищем решение
            for (int i = (values.Count() - 1); i >= 0; i--)
            {
                for (int j = (values.Count() - 1); j > i; j--)
                    if (values[i][i] == 0)
                        if (values[values[0].Count()][i] == 0)
                            return 2; //множество решений
                        else
                            return 1; //нет решения
            }

            return 0;
        }

        /// <summary>
        /// Решение матрицы когда есть базис
        /// Если матрица не соответствует базису возращает -1
        /// Если у матрицы нет решений возращает 1, множество решений 2
        /// </summary>
        /// <param name="basis"></param>
        /// <param name="isOnes"></param>
        /// <returns></returns>
        public int SolveMatrix(List<Fraction> basis, bool isOnes)
        {
            int i;
            //переставить столбцы.
            List<Fraction> newBasis = new List<Fraction>();
            List<List<Fraction>> newMatrix = new List<List<Fraction>>();
            
            //создание новой матрицы перевернутой newMatrix, копии values.
            for (int k = 0; k < values[0].Count; k++)
            {
                newMatrix.Add(new List<Fraction>());
                for (int l = 0; l < values.Count; l++)
                {
                    newMatrix[k].Add(new Fraction());
                    newMatrix[k][l] = values[l][k];
                }
            }

            List<List<Fraction>> matrix = new List<List<Fraction>>();

            #region Создание матрицы matrix на основе NewMatrix
            for (i = 0; i < values[0].Count - 1; i++)
            {
                if (basis[i] != 0)
                {
                    matrix.Add(newMatrix[i]);//вставляет строку матрицы ,если у базиса у него x[i] != 0
                    newBasis.Add(new Fraction(i));//создает базис от x* =(0,1,...,values[0].Count - 2)
                }
            }

            for (i = 0; i < values[0].Count - 1; i++)
            {
                if (basis[i] == 0)
                {
                    matrix.Add(newMatrix[i]);
                    newBasis.Add(new Fraction(i));
                }
            }
            matrix.Add(newMatrix[i]);//вставляется последняя строка 
            #endregion
            

            for (int k = 0; k < values.Count; k++)
            {
                for (int l = 0; l < values[0].Count; l++)
                {
                    values[k][l] = matrix[l][k];
                }
            }//перевернули матриц

            for (i = 0; i < values.Count(); i++)
            {
                SortRows(i);//пройдя по каждому стобцу , находится максимальный элемент и он переставляется вверх на каждую последующую вниз строчку 
                Fraction m = values[i][i];//тот самый максимальный элемент в столбце
                if (m.Numerator == 0)
                {
                    return -1;//ошибка
                }

                for (int j = 0; j < values[0].Count; j++)
                {
                    values[i][j] /= m;
                }//вся i-я строка делится на максимальный элемент

                //прямой ход метода Гаусса
                for (int j = 0; j < values.Count(); j++)
                {
                    if (i != j && m != 0)
                    {
                        Fraction MultElement = values[j][i];
                        for (int k = i; k < values[0].Count(); k++)
                        {
                            values[j][k] -= values[i][k] * MultElement;
                        }
                    }
                    
                }
            }

            //если остался отрицательный свободный член, то вектор не подходит для решения
            int z = values[0].Count() - 1;
            for (i = 0; i < values.Count() - 1; i++)
            {
                if (values[i][z].Sign == -1)
                    return -1;
            }

            //ищем решение
            for (i = (values.Count() - 1); i >= 0; i--)
            {
                for (int j = (values.Count() - 1); j > i; j--)
                    if (values[i][i] == 0)
                        if (values[values[0].Count()][i] == 0)
                            return 2; //множество решений
                        else
                            return 1; //нет решения
            }

            var s = ToString();
            if (isOnes)
            {
                return 0;
            }
            
            if (!isOnes)//проверка на соответствие базиса к таблице
            {
                int j = 0;
                //проверка на соответсвие
                for (i = 0; i < basis.Count; i++)
                {
                    if (basis[i] == 0)
                        continue;
                    else if (basis[i] != values[j][values[j].Count - 1])
                        return -1;
                    j++;
                }
            }

            return 0;
        }
        
        public void Gauss(List<List<Fraction>> elements, bool? CornerDot, Fraction[] variable_visualization)
        {
            for (int global = 0; global < elements.Count; global++)
            {
                for (int i = global; i < elements.Count; i++)
                {
                    if (i == global)
                    {
                        //проверяем возможность выражения переменной
                        Fraction first_elem = elements[i][global];
                        bool responce = true; //можно ли вообще выразить
                        if (first_elem == 0)
                        {
                            responce = false;
                            for (int k = i + 1; k < elements.Count; k++)
                                if (elements[k][global] != 0)
                                {
                                    responce = true;
                                    first_elem = elements[k][global];
                                    Fraction temp;
                                    //смена строк
                                    for (int j = 0; j < elements[0].Count; j++)
                                    {
                                        temp = elements[i][j];
                                        elements[i][j] = elements[k][j];
                                        elements[k][j] = temp;
                                    }
                                    break;
                                }
                        }

                        //если не получилось выразить переменную и была задана начальная угловая точка
                        if ((responce == false) && (CornerDot == true))
                            throw new Exception("Невозможно выразить одну или несколько базисных переменных. Возможно неверно введены коэффициенты.");
                        //если не получилось выразить переменную и НЕ была задана начальная угловая точка
                        else if ((responce == false) && (CornerDot == false))
                        {
                            //то ищем в других столбцах
                            bool check = false;
                            for (int column = global + 1; column < elements[0].Count; column++)
                            {
                                for (int row = i; row < elements.Count; row++)
                                {
                                    if (elements[row][column] != 0)
                                    {
                                        check = true;
                                        first_elem = elements[row][column];
                                        Fraction temp; //вспомогательная переменная

                                        //смена строк
                                        for (int j = 0; j < elements[0].Count; j++)
                                        {
                                            //для элементов матрицы
                                            temp = elements[i][j];
                                            elements[i][j] = elements[row][j];
                                            elements[row][j] = temp;
                                        }

                                        //смена столбцов
                                        for(int k = 0; k < elements.Count; k++)
                                        {
                                            //для элементов матрицы
                                            temp = elements[k][global];
                                            elements[k][global] = elements[k][column];
                                            elements[k][column] = temp;
                                        }

                                        //для массива визуализации
                                        temp = variable_visualization[global];
                                        variable_visualization[global] = variable_visualization[column];
                                        variable_visualization[column] = temp;
                                    }

                                    if (check)
                                        break;
                                }
                                if (check)
                                    break;
                            }

                            //Такого случая возможно не может быть. Поэтому это излишне.
                            if (check == false)
                                throw new Exception("Невозможно выразить переменные. Возможно неверно введены коэффициенты.");
                        }



                        for (int j = 0; j < elements[0].Count; j++)
                            elements[i][j] /= first_elem;
                    }
                    else
                    {
                        Fraction first_elem = elements[i][global];
                        for (int j = 0; j < elements[0].Count; j++)
                        {
                            elements[i][j] -= elements[global][j] * first_elem;
                        }
                    }
                }
            }

            values = elements;
        }

        /// <summary>
        /// Выражение базисных переменных.
        /// </summary>
        /// <param name="elements">Двумерный массив коэффициентов системы линейных ограничений-равенств.</param>
        /// <param name="number">Количество базисных переменных.</param>
        public static void HoistingMatrix(List<List<double>> elements, int number)
        {
            for (int global = 1; global < number; global++)
            {
                for (int i = global - 1; i >= 0; i--)
                {
                    double first_elem = elements[i][global];
                    for (int j = global; j < elements[0].Count; j++)
                    {
                        elements[i][j] -= elements[global][j] * first_elem;
                    }
                }
            }
        }

        public override String ToString()//представление матрицы
        {
            String S = "";
            for (int i = 0; i < values.Count; i++)
            {
                S += "\r\n";
                for (int j = 0; j < values[0].Count; j++)
                {
                    S += values[i][j].ToString() + "\t";
                }
            }

            return S;
        }
        
        public object Clone()
        {
            return new Matrix
            {
                values = this.values
            };
        }
    }
}