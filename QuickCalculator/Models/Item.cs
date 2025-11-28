using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickCalculator.Models
{
    public class Item : GameObj
    {
        public Item() { } // 添加无参构造函数用于反序列化
        public Item(string name)
        {
            Name = name;
            Attributes = new Dictionary<string, double>();
        }
    }
}
