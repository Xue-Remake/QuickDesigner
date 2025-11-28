using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickCalculator.Models
{
    public class Character : GameObj
    {
        public Character(string name)
        {
            //注意一下，在调用之前先写一个是否为null的判定，如果为null，则命名不合法
            Name = name;
            Attributes = new Dictionary<string, double>();
        }
        public Character() { } // 添加无参构造函数用于反序列化
    }
}
