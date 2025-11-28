using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace QuickCalculator.Models
{
    /// <summary>
    /// 快速计算器主类，负责协调角色、道具和公式的计算流程。
    /// 提供设置角色、道具和公式的方法，以及执行局部计算和最终计算的逻辑。
    /// 
    /// 主要方法：
    /// SetCharacter1/2 - 设置角色并加载数据
    /// SetItem1/2 - 设置道具并加载数据
    /// SetFormula1/2/3 - 设置计算公式
    /// tmpCalculate - 执行角色与道具的局部计算，返回临时角色
    /// finalCalculate - 执行两个临时角色的最终交互计算
    /// all_Calculate - 执行完整的计算流程
    /// </summary>
    public class Calculator
    {
        public Character character1, character2;
        public Item item1, item2;
        public Formula Formula1, Formula2, Formula3;

        public Calculator()
        {
            character1 = new Character("null");
            character2 = new Character("null");
            item1 = new Item("null");
            item2 = new Item("null");
            Formula1 = new Formula(null, null);
            Formula2 = new Formula(null, null);
            Formula3 = new Formula(null, null);
        }

        //设置的方法
        public void SetCharacter1(string nameOfchar, string fileName)
        {
            //从文件中读取
            character1 = DataManager.FindCharacter(nameOfchar, fileName);
        }
        public void Setcharacter2(string nameOfchar, string fileName)
        {
            character2 = DataManager.FindCharacter(nameOfchar, fileName);
        }
        public void Setitem1(string nameOfitem, string fileName)
        {
            item1 = DataManager.FindItem(nameOfitem, fileName);
        }
        public void Setitem2(string nameOfitem, string fileName)
        {
            item2 = DataManager.FindItem(nameOfitem, fileName);
        }
        public void Setformula1(string nameOfformula, string fileName)
        {
            Formula1 = DataManager.FindFormula(nameOfformula, fileName);
        }
        public void Setformula2(string nameOfformula, string fileName)
        {
            Formula2 = DataManager.FindFormula(nameOfformula, fileName);
        }
        public void Setformula3(string nameOfformula, string fileName)
        {
            Formula3 = DataManager.FindFormula(nameOfformula, fileName);
        }

        //计算局部的方法，返回一个临时Character类
        public Character tmpCalculate(Character chara, Item item, Formula form)
        {
            Character tmpCharacter = new Character("null");
            if (form.Distinction == false)
            {
                throw new Exception("This formula is not computable.");//公式不可算的错误
            }
            Stack<double> vals = new Stack<double>();
            Stack<string> Operator = new Stack<string>();
            string[] symbols = form.Form.Split(' ');
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
                        v = vals.Pop() / v;
                    }
                    else if (op.Equals("sqrt"))
                    {
                        v = Math.Sqrt(v);
                    }
                    vals.Push(v);
                }
                else if (double.TryParse(symbols[i], out double result))
                {
                    vals.Push(result);
                }
                else //遇见关键词
                {
                    string Search = symbols[i];
                    if (chara.Attributes.ContainsKey(Search)) //先找Character中是否存在属性
                    {
                        double replace = chara.Attributes[Search];
                        vals.Push(replace);
                    }
                    else if (item.Attributes.ContainsKey(Search)) //再找Item中是否存在属性
                    {
                        double replace = item.Attributes[Search];
                        vals.Push(replace);
                    } //关键词均不匹配
                    else
                    {
                        throw new Exception("The target attribute is not present in the processed data.");
                        //处理的数据中没有目标属性
                    }
                }
            }//for结束
            string target = form.Target;
            if (!chara.Attributes.ContainsKey(target))
            {
                throw new Exception("The target does not exist in the character attributes.");
                //角色属性中不存在目标
            }
            double updata = vals.Pop();
            tmpCharacter = chara;
            tmpCharacter.SetAttribute(target, updata);
            return tmpCharacter;
        }//方法结束

        //最终计算，计算两个tmp角色的公式交互，产生的量是一个值
        public double finalCalculate(Character tmp1, Character tmp2, Formula form)
        {
            double result;
            if (form.Distinction == false)
            {
                throw new Exception("This formula is not computable.");
                //公式不可算的错误
            }

            //----------开始公式处理----------
            Stack<double> vals = new Stack<double>();
            Stack<string> Operator = new Stack<string>();
            string[] symbols = form.Form.Split(' ');
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
                        v = vals.Pop() / v;
                    }
                    else if (op.Equals("sqrt"))
                    {
                        v = Math.Sqrt(v);
                    }
                    vals.Push(v);
                }
                else if (double.TryParse(symbols[i], out double r))
                {
                    vals.Push(r);
                }
                else //遇见关键词
                {
                    string Search = symbols[i];
                    //首先查看是否存在指定标注
                    string[] dis = symbols[i].Split('_');
                    if (dis.Length == 2)
                    {
                        //如果有first前缀标注，就从tmp1中查找属性并替换
                        if (dis[0] == "first")
                        {
                            if (tmp1.Attributes.ContainsKey(dis[1]))
                                vals.Push(tmp1.Attributes[dis[1]]);
                            else
                                throw new Exception("Failed to locate specified attribute: Character One does not possess this attribute.");
                            //指定属性查找失败：角色一不存在这个给属性
                        }
                        if (dis[0] == "second")
                        {
                            if (tmp2.Attributes.ContainsKey(dis[1]))
                                vals.Push(tmp2.Attributes[dis[1]]);
                            else
                                throw new Exception("Failed to locate specified attribute: Character One does not possess this attribute.");
                            //指定属性查找失败：角色一不存在这个给属性
                        }
                    }
                    else
                    {
                        //没有前缀标记，按顺序查找
                        if (tmp1.Attributes.ContainsKey(Search)) //先找tmp1中是否存在属性
                        {
                            double replace = tmp1.Attributes[Search];
                            vals.Push(replace);
                        }
                        else if (tmp2.Attributes.ContainsKey(Search)) //再找tmp2中是否存在属性
                        {
                            double replace = tmp2.Attributes[Search];
                            vals.Push(replace);
                        } //关键词均不匹配
                        else
                        {
                            throw new Exception("The target attribute is not present in the processed data.");
                            //处理的数据中没有目标属性
                        }
                    }
                }
            }//for结束
            //----------公式处理结束----------
            result = vals.Pop();
            return result;
        }//方法结束

        //执行全部计算
        public double all_Calculate() 
        {
            //直接执行所有的计算
            Character tmp1, tmp2;
            double result;
            tmp1 = tmpCalculate(character1, item1, Formula1);
            tmp2 = tmpCalculate(character2, item2, Formula2);
            result = finalCalculate(tmp1, tmp2, Formula3);
            return result;
        }
    }
}
