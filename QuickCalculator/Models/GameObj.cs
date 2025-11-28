using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace QuickCalculator.Models
{
    /**
     * 角色和道具类的基类
     * string Name
     * Dictionary _attributes
     * AddAttribute(string name, double value) 增加属性的方法
     * RemoveAttribute(string name)            移除属性的方法
     * GetAttribute(string attributename)      查看指定属性的值
     * SetAttribute(string attributename, double value)  设置属性值的方法
     */
    public class GameObj
    {

        public string Name { get; set; } = ""; // 改为属性
        public Dictionary<string, double> Attributes { get; set; }
        public GameObj() 
        {
            Attributes = new Dictionary<string, double>();
        }

        //添加属性的方法，用于创建实例时添加
        public void AddAttribute(string name, double value)
        {
            Attributes[name] = value;
        }

        //去除属性的方法
        public void RemoveAttribute(string name)
        {
            if (!Attributes.ContainsKey(name))
            {
                Console.WriteLine("错误，移除的属性不存在");
                return;
            }
            Attributes.Remove(name);
        }

        //查看指定属性的值
        public double GetAttribute(string attributename)
        {
            if (!Attributes.ContainsKey(attributename))
            {
                Console.WriteLine("不存在属性：" + attributename + "，默认返回0");
                return 0;
            }
            return Attributes[attributename];
        }

        //设置属性值的方法
        public void SetAttribute(string attributename, double value)
        {
            if (Attributes.ContainsKey(attributename))
                Attributes[attributename] = value;
            else
                throw new KeyNotFoundException($"Attribute '{attributename}' not found.");
        }

        //索引器方式访问
        public double this[string attributename]
        {
            get => GetAttribute(attributename);
            set => SetAttribute(attributename, value);
        }
    }
}
