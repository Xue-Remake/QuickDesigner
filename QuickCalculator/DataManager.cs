using QuickCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;



namespace QuickCalculator
{
    /// <summary>
    /// 数据文件管理类，用于保存和读取Character和Item的JSON数据文件。
    /// SaveCharacters(List<Character> characters, string filePath)   保存角色列表到JSON文件
    /// SaveItems(List<Item> items, string filePath)                  保存道具列表到JSON文件
    /// LoadCharacters(string filePath)                               读取JSON文件中的角色数据，返回List<Character>
    /// LoadItems(string filePath)                                    读取JSON文件中的道具数据，返回List<Item>
    /// FindCharacter(string name, string filePath)                   从JSON文件中查找指定名称的Character实例
    /// FindItem(string name, string filePath)                        从JSON文件中查找指定名称的Item实例
    /// ChangeCharacter_Remove(string name, string filePath, string attribute)            移除角色的指定属性
    /// ChangeItem_Remove(string name, string filePath, string attribute)                 移除道具的指定属性
    /// ChangeCharacter_Add(string name, string filePath, string attribute, double value) 为角色添加属性
    /// ChangeItem_Add(string name, string filePath, string attribute, double value)      为道具添加属性
    /// ChangeCharacter_Set(string name, string filePath, string attribute, double value) 设置角色的属性值
    /// ChangeItem_Set(string name, string filePath, string attribute, double value)      设置道具的属性值
    /// SaveFormula(List<Formula> formulas, string filePath)                     保存公式列表到JSON文件
    /// LoadFormulas(string filePath)                                            读取JSON文件中的公式数据，返回List<Formula>
    /// FindFormula(string name, string filePath)                                从JSON文件中查找指定名称的Formula实例
    /// </summary>


    //配置JSON序列化选项
    public static class DataManager
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true, //添加这个选项以包含字段
            PropertyNamingPolicy = null
        };
        //保留数据到文件的完整流程
        public static void SaveCharacters(List<Character> characters, string filePath)
        {
            //1.序列化：C#对象到JSON字符串
            string jsonString = JsonSerializer.Serialize(characters, _jsonOptions);
            //2.写入文本：字符串到磁盘文件
            File.WriteAllText(filePath, jsonString);
        }
        public static void SaveItems(List<Item> items, string filePath)
        {
            string jsonString = JsonSerializer.Serialize(items, _jsonOptions);
            File.WriteAllText(filePath, jsonString);
        }

        //从文件读取数据的完整流程
        public static List<Character> LoadCharacters(string filePath)
        {
            //1.检查文件是否存在
            if (!File.Exists(filePath))
            {
                Console.WriteLine("文件不存在，返回空列表");
                return new List<Character>();
            }
            try
            {
                //2.读取文件：磁盘文件到字符串
                string jsonString = File.ReadAllText(filePath);
                //3.反序列化：JSON字符串到C#对象
                List<Character> characters = JsonSerializer.Deserialize<List<Character>>(jsonString, _jsonOptions);
                return characters ?? new List<Character>(); //如果反序列化失败则返回空列表
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取文件时出错：{ex.Message}");
                return new List<Character>();
            }
        }
        public static List<Item> LoadItems(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("文件不存在，返回空列表");
                return new List<Item>();
            }
            try
            {
                string jsonString = File.ReadAllText(filePath);
                List<Item> items = JsonSerializer.Deserialize<List<Item>>(jsonString, _jsonOptions);
                return items ?? new List<Item>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取文件时出错：{ex.Message}");
                return new List<Item>();
            }
        }

        //查找特定的Character，以name索引
        public static Character FindCharacter(string name, string filePath)
        {
            //先读取整个文件，反序列化后再查找
            var allCharacters = LoadCharacters(filePath);
            var Discrimination = allCharacters.Find(c => c.Name == name);
            if (Discrimination == null)
            {
                Console.WriteLine("错误，不存在Name = " + "\"" + name + "\"" + "的数据");
                Console.WriteLine("默认返回一个空对象");
                return new Character("null");
                //这里返回了一个名为null的对象
                //因为用户试图创造null的命名实例时不会调用构造函数而是抛出错误，所以这样的处理是安全的
            }
            return allCharacters.Find(c => c.Name == name);
        }
        public static Item FindItem(string name, string filePath)
        {
            var allItems = LoadItems(filePath);
            var Discrimination = allItems.Find(c => c.Name == name);
            if (Discrimination == null)
            {
                Console.WriteLine("错误，不存在Name = " + "\"" + name + "\"" + "的数据");
                Console.WriteLine("默认返回一个空对象");
                return new Item("null");
            }
            return allItems.Find(c => c.Name == name);
        }

        public static void ChangeCharacter_Remove(string name, string filePath, string attribute)
        //改变数据中以name索引的Character：移除特定属性
        {
            var allCharacters = LoadCharacters(filePath);
            var targetcharacter = allCharacters.Find(c => c.Name == name);
            if (targetcharacter == null)
            {
                Console.WriteLine("错误，不存在Name = " + "\"" + name + "\"" + "的数据");
                return;
            }
            if (!targetcharacter.Attributes.ContainsKey(attribute))
            {
                Console.WriteLine($"错误，{name}中不存在属性{attribute}");
                return;
            }
            targetcharacter.Attributes.Remove(attribute);
            //标记，储存更新的数据
            SaveCharacters(allCharacters, filePath);
            Console.WriteLine($"属性\"{attribute}\"已从{name}中移除");
        }

        public static void ChangeItem_Remove(string name, string filePath, string attribute)
        {
            var allItems = LoadItems(filePath);
            var targetitem = allItems.Find(c => c.Name == name);
            if (targetitem == null)
            {
                Console.WriteLine("错误，不存在Name = " + "\"" + name + "\"" + "的数据");
                return;
            }
            if (!targetitem.Attributes.ContainsKey(attribute))
            {
                Console.WriteLine($"错误，{name}中不存在属性{attribute}");
                return;
            }
            targetitem.Attributes.Remove(attribute);
            SaveItems(allItems, filePath);
            Console.WriteLine($"属性\"{attribute}\"已从{name}中移除");
        }

        //改变数据中name索引的Character：添加特定属性
        public static void ChangeCharacter_Add(string name, string filePath, string attribute, double value)
        {
            var allCharacters = LoadCharacters(filePath);
            var target = allCharacters.Find(c => c.Name == name);
            if (target == null)
            {
                Console.WriteLine("错误，不存在Name = " + "\"" + name + "\"" + "的数据");
                return;
            }
            if (target.Attributes.ContainsKey(attribute))
            {
                Console.WriteLine($"错误，{name}中已存在属性{attribute}");
                return;
            }
            target.Attributes.Add(attribute, value);
            SaveCharacters(allCharacters, filePath);
            Console.WriteLine($"属性\"{attribute}\"，值：{value}，已被加入到{name}中");
        }
        public static void ChangeItem_Add(string name, string filePath, string attribute, double value)
        {
            var allItems = LoadItems(filePath);
            var target = allItems.Find(c => c.Name == name);
            if (target == null)
            {
                Console.WriteLine("错误，不存在Name = " + "\"" + name + "\"" + "的数据");
                return;
            }
            if (target.Attributes.ContainsKey(attribute))
            {
                Console.WriteLine($"错误，{name}中已存在属性{attribute}");
                return;
            }
            target.Attributes.Add(attribute, value);
            SaveItems(allItems, filePath);
            Console.WriteLine($"属性\"{attribute}\"，值：{value}，已被加入到{name}中");
        }

        //改变数据中name索引的Character：设置特定属性
        public static void ChangeCharacter_Set(string name, string filePath, string attribute, double value)
        {
            var allCharacters = LoadCharacters(filePath);
            var target = allCharacters.Find(c => c.Name == name);
            if (target == null)
            {
                Console.WriteLine("错误，不存在Name = " + "\"" + name + "\"" + "的数据");
                return;
            }
            if (!target.Attributes.ContainsKey(attribute))
            {
                Console.WriteLine($"错误，{name}中不存在属性{attribute}");
                return;
            }
            target.Attributes[attribute] = value;
            SaveCharacters(allCharacters, filePath);
            Console.WriteLine($"{name}的属性\"{attribute}\"值已被改为：{value}");
        }
        public static void ChangeItem_Set(string name, string filePath, string attribute, double value)
        {
            var allItems = LoadItems(filePath);
            var target = allItems.Find(c => c.Name == name);
            if (target == null)
            {
                Console.WriteLine("错误，不存在Name = " + "\"" + name + "\"" + "的数据");
                return;
            }
            if (!target.Attributes.ContainsKey(attribute))
            {
                Console.WriteLine($"错误，{name}中不存在属性{attribute}");
                return;
            }
            target.Attributes[attribute] = value;
            SaveItems(allItems, filePath);
            Console.WriteLine($"{name}的属性\"{attribute}\"值已被改为：{value}");
        }

        //处理公式的方法
        //保存公式
        public static void SaveFormula(List<Formula> formulas, string filePath)
        {
            string jsonString = JsonSerializer.Serialize(formulas, _jsonOptions);
            File.WriteAllText(filePath, jsonString);
        }

        //读取公式
        public static List<Formula> LoadFormulas(string filePath)
        {
            //1.检查文件是否存在
            if (!File.Exists(filePath))
            {
                Console.WriteLine("文件不存在，返回空列表");
                return new List<Formula>();
            }
            try
            {
                //2.读取文件：磁盘文件到字符串
                string jsonString = File.ReadAllText(filePath);
                //3.反序列化：JSON字符串到C#对象
                List<Formula> formulas = JsonSerializer.Deserialize<List<Formula>>(jsonString, _jsonOptions);
                return formulas ?? new List<Formula>(); //如果反序列化失败则返回空列表
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取文件时出错：{ex.Message}");
                return new List<Formula>();
            }
        }

        //找寻特定公式
        public static Formula FindFormula(string name, string filePath)
        {
            List<Formula> allformulas = LoadFormulas(filePath);
            var Discrimination = allformulas.Find(c => c.Name == name);
            if (Discrimination == null)
            {
                Console.WriteLine("错误，不存在Name = " + "\"" + name + "\"" + "的数据");
                Console.WriteLine("默认返回一个空对象");
                return new Formula(null, null);
            }
            return allformulas.Find(c => c.Name == name);
        }

        //查询会话中的临时数据在文件中是否名称重复
        public static bool IsRepetitive<T>(string name, string filePath)
        {
            //使用泛型方法，根据类型T来决定查询数据的类别
            if (typeof(T) == typeof(Character))
            {
                var allCharacters = LoadCharacters(filePath);
                return allCharacters.Any(c => c.Name == name);
            }
            else if (typeof(T) == typeof(Item))
            {
                var allItems = LoadItems(filePath);
                return allItems.Any(i => i.Name == name);
            }
            else if (typeof(T) == typeof(Formula))
            {
                var allFormulas = LoadFormulas(filePath);
                return allFormulas.Any(f => f.Name == name);
            }
            else
            {
                throw new ArgumentException("Unsupported type for repetition check.");
            }
        }
    }
}
