using System;

namespace SimplexLib
{
    public class Fraction
    {
        private int numerator;
        private int denominator;
        private int sign;

        public int Numerator
        {
            get => numerator;
            set => numerator = value;
        }

        public int Denominator
        {
            get => denominator;
            set => denominator = value;
        }

        public int Sign
        {
            get => sign;
            set => sign = value;
        }


        #region Конструкторы класса Fraction

        public Fraction()
        {
        }

        /// <summary>
        /// Конструктор класса Fraction:
        /// </summary>
        /// <param name="num"></param>
        /// <param name="denom"></param>
        public Fraction(int num, int denom)
        {
            if (num < 0)
            {
                sign = -1;
                numerator = -num;
            }
            else
            {
                sign = 1;
                numerator = num;
            }

            denominator = denom;
        }

        /// <summary>
        /// Конструктор класса Fraction
        /// </summary>
        /// <param name="k"></param>
        public Fraction(int k)
        {
            numerator = Math.Abs(k);
            denominator = 1;
            sign = k < 0 ? -1 : 1;
        }

        /// <summary>
        /// Конструктор класса Fraction:
        /// По входной строке с числом (возможно дробным) определяет его целую , дробную и знаковую часть. 
        /// </summary>
        /// <param name="str"></param>
        /// <exception cref="Exception"></exception>
        public Fraction(string str)
        {
            str.Replace(" ", "");
            if (str.StartsWith("-"))
            {
                sign = -1;
                str = str.Remove(0, 1);
            }
            else
                sign = 1;

            //целые числа
            if (!str.Contains(",") && !str.Contains(".") && !str.Contains("/"))
            {
                int k = Convert.ToInt32(str);
                this.numerator = k;
                this.denominator = 1;
            }

            //десятичные дроби
            else if (str.Contains(",") || str.Contains("."))
            {
                string[] str1 = str.Split(new char[] {',', '.'});
                if (str1.Length != 2)
                {
                    throw new Exception();
                }
                else
                {
                    numerator = Convert.ToInt32(str1[0] + str1[1]);
                    denominator = Convert.ToInt32(Math.Pow(10, str1[1].Length));
                }
            }
            else if (str.Contains("/"))
            {
                string[] str1 = str.Split(new char[] {'/'});
                if (str1.Length != 2 || Convert.ToInt32(str1[1]) == 0)
                {
                    throw new Exception();
                    //MessageBox.Show("Некорректное значение. Дробь с нулем в знаменателе.");
                }
                else
                {
                    try
                    {
                        numerator = Convert.ToInt32(str1[0]);
                        denominator = Convert.ToInt32(str1[1]);
                    }
                    catch (Exception e)
                    {
                        throw new Exception();
                        //MessageBox.Show("Некорректное значение. Дробь с нулем в знаменателе.");
                    }
                }
            }
            else
                throw new Exception();
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// строковое представление числа
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            Reduce();
            if (numerator == 0)
            {
                return "0";
            }

            string result;
            if (sign < 0)
            {
                result = "-";
            }
            else
            {
                result = "";
            }

            if (numerator == denominator)
            {
                return result + "1";
            }

            if (denominator == 1)
            {
                return result + numerator;
            }

            return result + numerator + "/" + denominator;
        }

        /// <summary>
        /// наибольший общий делитель двух чисел
        /// </summary>
        /// <returns></returns>
        public int GCD(int c, int b)
        {
            while (b != 0)
            {
                int d = c % b;
                c = b;
                b = d;
            }

            return c;
        }

        /// <summary>
        /// наименьшее общее кратное двух числ
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public int SCM(int a, int b)
        {
            return a * b / GCD(a, b);
        }

        /// <summary>
        /// Сокращение дроби
        /// </summary>
        /// <returns></returns>
        public Fraction Reduce()
        {
            Fraction result = this;
            if (result.numerator == 0)
            {
                return result;
            }

            int k = GCD(result.numerator, result.denominator);
            result.denominator /= k;
            result.numerator /= k;
            return result;
        }

        /// <summary>
        /// Возвращает дробь с противоположным знаком
        /// </summary>
        /// <returns></returns>
        private Fraction GetWithChangedSign()
        {
            return new Fraction(-numerator * sign, denominator);
        }
        // Мой метод Equals

        #endregion

        #region Операторы сложения и вычитания

        public static Fraction operator +(Fraction a, Fraction b)
        {
            int c = a.sign * a.numerator * b.denominator + b.sign * b.numerator * a.denominator;
            int d = a.denominator * b.denominator;
            return new Fraction(c, d);
        }

        public static Fraction operator +(int a, Fraction b)
        {
            return new Fraction(a) + b;
        }

        public static Fraction operator +(Fraction a, int b)
        {
            return b + a;
        }

        public static Fraction operator -(Fraction a, Fraction b)
        {
            Fraction c = new Fraction();
            int k = a.sign * a.numerator * b.denominator - b.sign * (b.numerator * a.denominator);
            c.numerator = Math.Abs(k);
            c.denominator = a.denominator * b.denominator;
            c.sign = (k > 0) ? 1 : -1;
            return c;
        }

        public static Fraction operator -(int a, Fraction b)
        {
            return new Fraction(a) - b;
        }

        public static Fraction operator -(Fraction a, int b)
        {
            return b - a;
        }

        #endregion

        #region Операторы деления и умножения

        public static Fraction operator /(Fraction a, Fraction b)

        {
            Fraction c = new Fraction();
            c.numerator = a.numerator * b.denominator;
            c.denominator = a.denominator * b.numerator;
            c.sign = a.sign * b.sign;
            return c;
        }

        public static Fraction operator /(int a, Fraction b)
        {
            return new Fraction(a) / b;
        }

        public static Fraction operator /(Fraction a, int b)
        {
            return b / a;
        }

        public static Fraction operator *(Fraction a, Fraction b)
        {
            Fraction c = new Fraction();
            c.numerator = a.numerator * b.numerator;
            c.denominator = a.denominator * b.denominator;
            c.sign = a.sign * b.sign;
            return c;
        }

        public static Fraction operator *(int a, Fraction b)
        {
            return new Fraction(a) * b;
        }

        public static Fraction operator *(Fraction a, int b)
        {
            return b * a;
        }

        #endregion

        #region Другие различные перегрузки операторов

        // Перегрузка оператора "унарный минус"
        public static Fraction operator -(Fraction a)
        {
            return a.GetWithChangedSign();
        }

        // Перегрузка оператора "++"
        public static Fraction operator ++(Fraction a)
        {
            return a + 1;
        }

        // Перегрузка оператора "--"
        public static Fraction operator --(Fraction a)
        {
            return a - 1;
        }

        public bool Equals(Fraction that)
        {
            Fraction a = Reduce();
            Fraction b = that.Reduce();
            return a.numerator == b.numerator &&
                   a.denominator == b.denominator &&
                   a.sign == b.sign;
        }

        // Переопределение метода Equals
        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is Fraction)
            {
                result = this.Equals(obj as Fraction);
            }

            return result;
        }

        // Переопределение метода GetHashCode
        public override int GetHashCode()
        {
            return Convert.ToInt32(this.sign * (this.numerator * this.numerator + this.denominator * this.denominator));
        }

        // Перегрузка оператора "Равенство" для двух дробей
        public static bool operator ==(Fraction a, Fraction b)
        {
            object aAsObj = a as object;
            object bAsObj = b as object;
            if (a.numerator == 0 && b.numerator == 0)
                return true;
            if (a.numerator == b.numerator &&
                a.denominator == b.denominator &&
                a.sign == b.sign)
                return true;
            if (aAsObj == null || bAsObj == null)
            {
                return aAsObj == bAsObj;
            }

            return a.Equals(b);
        }

        // Перегрузка оператора "Равенство" для дроби и числа
        public static bool operator ==(Fraction a, int b)
        {
            return a == new Fraction(b);
        }

        // Перегрузка оператора "Равенство" для числа и дроби
        public static bool operator ==(int a, Fraction b)
        {
            return new Fraction(a) == b;
        }

        // Перегрузка оператора "Неравенство" для двух дробей
        public static bool operator !=(Fraction a, Fraction b)
        {
            return !(a == b);
        }

        // Перегрузка оператора "Неравенство" для дроби и числа
        public static bool operator !=(Fraction a, int b)
        {
            return a != new Fraction(b);
        }

        // Перегрузка оператора "Неравенство" для числа и дроби
        public static bool operator !=(int a, Fraction b)
        {
            return new Fraction(a) != b;
        }

        #endregion

        #region Операторы сравнения

        // Метод сравнения двух дробей
        // Возвращает	 0, если дроби равны
        //				 1, если this больше that
        //				-1, если this меньше that
        private int CompareTo(Fraction that)
        {
            if (Equals(that))
            {
                return 0;
            }

            Fraction a = Reduce();
            Fraction b = that.Reduce();
            int m = a.numerator * a.sign * b.denominator;
            int k = b.numerator * b.sign * a.denominator;
            if (a.numerator * a.sign * b.denominator > b.numerator * b.sign * a.denominator)
            {
                //int m = a.numerator * a.sign * b.denominator;
                //int k = b.numerator * b.sign * a.denominator;
                return 1;
            }

            return -1;
        }

        // Перегрузка оператора ">" для двух дробей
        public static bool operator >(Fraction a, Fraction b)
        {
            return a.CompareTo(b) > 0;
        }

        // Перегрузка оператора ">" для дроби и числа
        public static bool operator >(Fraction a, int b)
        {
            return a > new Fraction(b);
        }

        // Перегрузка оператора ">" для числа и дроби
        public static bool operator >(int a, Fraction b)
        {
            return new Fraction(a) > b;
        }

        // Перегрузка оператора "<" для двух дробей
        public static bool operator <(Fraction a, Fraction b)
        {
            return a.CompareTo(b) < 0;
        }

        // Перегрузка оператора "<" для дроби и числа
        public static bool operator <(Fraction a, int b)
        {
            return a < new Fraction(b);
        }

        // Перегрузка оператора "<" для числа и дроби
        public static bool operator <(int a, Fraction b)
        {
            return new Fraction(a) < b;
        }

        // Перегрузка оператора ">=" для двух дробей
        public static bool operator >=(Fraction a, Fraction b)
        {
            return a.CompareTo(b) >= 0;
        }

        // Перегрузка оператора ">=" для дроби и числа
        public static bool operator >=(Fraction a, int b)
        {
            return a >= new Fraction(b);
        }

        // Перегрузка оператора ">=" для числа и дроби
        public static bool operator >=(int a, Fraction b)
        {
            return new Fraction(a) >= b;
        }

        // Перегрузка оператора "<=" для двух дробей
        public static bool operator <=(Fraction a, Fraction b)
        {
            return a.CompareTo(b) <= 0;
        }

        // Перегрузка оператора "<=" для дроби и числа
        public static bool operator <=(Fraction a, int b)
        {
            return a <= new Fraction(b);
        }

        // Перегрузка оператора "<=" для числа и дроби
        public static bool operator <=(int a, Fraction b)
        {
            return new Fraction(a) <= b;
        }

        #endregion
    }
}