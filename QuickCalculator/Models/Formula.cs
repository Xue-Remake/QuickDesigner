using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickCalculator.Models
{
    /// <summary>
    /// 公式类，用于表示和验证数学公式的可计算性。
    /// 该类能够对包含基本运算符和函数的公式进行语法检查和运行时验证，
    /// 通过将变量替换为随机数来模拟计算过程，确保公式在给定条件下可安全执行。
    /// </summary>
    /// <remarks>
    /// 主要功能：
    /// 1. 存储公式名称和表达式内容
    /// 2. 通过随机数替换变量进行可算性检验
    /// 3. 检测除零错误和负数平方根等运行时异常
    /// 4. 验证最终计算结果的有效性
    /// 
    /// 方法简介：
    /// Formula(string name, string form) - 构造函数，初始化公式名称和表达式
    /// inspection() - 执行可算性检验，验证公式是否可以安全计算
    /// NewRandom(double min, double max) - 生成指定范围内的随机数，用于变量替换
    /// </remarks>
    public class Formula
    {
        public string Name;
        public string Target; //公式的改变值，是一个关键字
        public bool Distinction; //判断值，判断公式能否被解析
        public string Form; //公式的内容(未解析)

        //简单的随机数生成器
        public static double NewRandom(double min, double max)
        {
            Random r = new Random();
            double random = min + r.NextDouble() * (max - min);
            return random;
        }

        public Formula() { }//无参数构造函数

        public Formula(string name, string form)
        {
            Distinction = false; //未经过可算性检验，默认为false
            Name = name;
            Form = form;
            Target = "null"; 
            //"null"代表公式没有实例改变值，用于两个临时角色的最终计算
        }
        public Formula(string name, string form, string target)
        {
            Distinction = false; //未经过可算性检验，默认为false
            Name = name;
            Form = form;
            Target = target;
        }

        //可算性检验方法，将关键字全部替换成随机数进行计算
        public void inspection()
        {
            Stack<double> vals = new Stack<double>();
            Stack<string> Operator = new Stack<string>();
            string[] symbols = Form.Split(' ');
            for (int i = 0; i < symbols.Length; i++)
            {
                if (symbols[i].Equals("(")) ;
                else if (symbols[i].Equals("+")) Operator.Push(symbols[i]);
                else if (symbols[i].Equals("-")) Operator.Push(symbols[i]);
                else if (symbols[i].Equals("*")) Operator.Push(symbols[i]);
                else if (symbols[i].Equals("/")) Operator.Push(symbols[i]);
                else if (symbols[i].Equals("sqrt")) Operator.Push(symbols[i]);

                //检测到右括号，开始处理
                else if (symbols[i].Equals(")"))
                {
                    string op = Operator.Pop();
                    double v = vals.Pop();
                    if (op.Equals("+")) v = vals.Pop() + v;
                    else if (op.Equals("-")) v = vals.Pop() - v;
                    else if (op.Equals("*")) v = vals.Pop() * v;
                    else if (op.Equals("/"))
                    {
                        // 检查除数是否为0
                        if (Math.Abs(v) < double.Epsilon)
                        {
                            Distinction = false;
                            return;
                        }
                        v = vals.Pop() / v;
                    }
                    else if (op.Equals("sqrt"))
                    {
                        // 检查平方根参数是否为负数
                        if (v < 0)
                        {
                            Distinction = false;
                            return;
                        }
                        v = Math.Sqrt(v);
                    }
                    vals.Push(v);
                }
                else if (double.TryParse(symbols[i], out double result))
                {
                    vals.Push(result);
                }
                else
                {
                    string Search = symbols[i];
                    double replace = NewRandom(1, 1000);
                    vals.Push(replace);
                }
            }//for结束
            // 处理栈中剩余的运算符
            while (Operator.Count > 0)
            {
                string op = Operator.Pop();
                double v = vals.Pop();

                if (op.Equals("+")) v = vals.Pop() + v;
                else if (op.Equals("-")) v = vals.Pop() - v;
                else if (op.Equals("*")) v = vals.Pop() * v;
                else if (op.Equals("/"))
                {
                    if (Math.Abs(v) < double.Epsilon)
                    {
                        Distinction = false;
                        return;
                    }
                    v = vals.Pop() / v;
                }
                else if (op.Equals("sqrt"))
                {
                    if (v < 0)
                    {
                        Distinction = false;
                        return;
                    }
                    v = Math.Sqrt(v);
                }
                vals.Push(v);
            }

            // 最终栈中应该只有一个结果
            if (vals.Count == 1)
            {
                double finalResult = vals.Pop();

                // 验证结果是否有效（不是无穷大或NaN）
                if (!double.IsInfinity(finalResult) && !double.IsNaN(finalResult))
                {
                    Distinction = true;
                    Console.WriteLine($"公式 '{Name}' 检验通过，计算结果: {finalResult}");
                }
                else
                {
                    Distinction = false;
                    Console.WriteLine($"公式 '{Name}' 检验失败：计算结果无效");
                }
            }
            else
            {
                Distinction = false;
                Console.WriteLine($"公式 '{Name}' 检验失败：表达式不完整");
            }
        }//方法结束
    }
}

