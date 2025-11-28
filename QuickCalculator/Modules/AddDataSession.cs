using QuickCalculator.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static QuickCalculator.FileManager;

namespace QuickCalculator.Modules
{
    /// <summary>
    /// 添加数据会话类，处理数据添加的相关操作。
    /// 管理数据的储存
    /// </summary>

    public class AddDataSession : BaseSession
    {
        //默认路径
        private readonly string DataDirectory = "GameObj_data";
        private readonly string FormulaDirectory = "Formula_data";
        private readonly string CharacterDataPath = "GameObj_data/Default_Character.json"; // 默认角色数据文件路径
        private readonly string ItemDataPath = "GameObj_data/Default_Item.json"; // 默认道具数据文件路径
        private readonly string FormulaDataPath = "Formula_data/Default_Formula.json"; // 默认公式数据文件路径
        //private string FormulaGroupDataPath = "Formula_data/FormulaGroupData.json"; // 公式组功能待实现
        public override string SessionName => "计算器-添加数据模式";
        public override async Task<BaseSession> ExecuteAsync()
        {
            DisPlayHeader();
            DisplayAddDataMenu();

            Console.WriteLine("====正在校验默认路径=====");
            // 检查并创建默认文件夹和文件
            bool _checkdatadictionary = CheckDirectoryExists(DataDirectory);
            bool _checkformuladictionary = CheckDirectoryExists(FormulaDirectory);
            if (!_checkdatadictionary)
            {
                CreateDirectory(DataDirectory);
                Console.WriteLine($"已创建默认数据文件夹：{DataDirectory}");
            }
            if (!_checkformuladictionary)
            {
                CreateDirectory(FormulaDirectory);
                Console.WriteLine($"已创建默认公式文件夹：{FormulaDirectory}");
            }
            bool _checkcharacterdatafile = CheckFileExists(CharacterDataPath);
            bool _checkitemdatafile = CheckFileExists(ItemDataPath);
            bool _checkformuladatafile = CheckFileExists(FormulaDataPath);
            if (!_checkcharacterdatafile)
            {
                CreateFile("CharacterData.json", CharacterDataPath);
                // 初始化为空列表的JSON文件
                DataManager.SaveCharacters(new List<Character>(), CharacterDataPath);
                Console.WriteLine($"已创建默认角色数据文件：{CharacterDataPath}");
            }
            if (!_checkitemdatafile)
            {
                CreateFile("ItemData.json", ItemDataPath);
                // 初始化为空列表的JSON文件
                DataManager.SaveItems(new List<Item>(), ItemDataPath);
                Console.WriteLine($"已创建默认道具数据文件：{ItemDataPath}");
            }
            if (!_checkformuladatafile)
            {
                CreateFile("FormulaData.json", FormulaDataPath);
                // 初始化为空列表的JSON文件
                DataManager.SaveFormula(new List<Formula>(), FormulaDataPath);
                Console.WriteLine($"已创建默认公式数据文件：{FormulaDataPath}");
            }
            Console.WriteLine("====默认路径校验完成=====\n");

            string choice = ReadInput("请选择操作");

            switch (choice)
            {
                case "chara":
                    await Addcharacter(); // 添加角色数据
                    return this;
                case "item":
                    await Additem(); // 添加道具数据
                    return this;
                case "formu":
                    await AddFormula(); // 添加公式数据
                    return this;
                case "back":
                    return null;
                default:
                    Console.WriteLine("无效输入，请重试。");
                    WaitForConfirmation();
                    return this;
            }
        }

        private void DisplayAddDataMenu()
        {
            Console.WriteLine("1. chara      -添加新角色");
            Console.WriteLine("2. item       -添加新道具");
            Console.WriteLine("3. formu      -添加新公式");
            Console.WriteLine("4. back       -返回主菜单");
            Console.WriteLine(); // 空行分隔
        }

        private async Task Addcharacter()
        {
            Console.WriteLine("=== 添加新角色 ===");
            //用户输入角色名称
            string name = ReadInput("请输入角色名称");
            while (true)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("角色名称不能为空，请重新输入。");
                    name = ReadInput("请输入角色名称");
                }
                else if (name == "null")
                {
                    Console.WriteLine("角色名称不合法，具体原因详见帮助会话");
                }
                else { break; }
            }

            //创建一个未储存角色
            Character tmpCharacter = new Character
            {
                Name = name,
                Attributes = new Dictionary<string, double>()
            };

            //用户输入角色属性名称
            while (true)
            {
                //循环：用户输入属性名称-->用户设置属性值-->...->用户输入"done"结束添加属性-->保存角色数据
                string attributeName = ReadInput("请输入属性名称（输入 'done' 完成添加）");
                if (string.IsNullOrWhiteSpace(attributeName) || attributeName == "null")
                {
                    Console.WriteLine("属性名称错误：为空或为\"null\"");
                    continue;
                }
                if (attributeName.ToLower() == "done")
                {
                    Console.WriteLine("属性添加结束");
                    break;
                }
                string attributeValueInput = ReadInput($"请输入属性 '{attributeName}' 的数值");
                double.TryParse(attributeValueInput, out double val);
                if (!double.TryParse(attributeValueInput, out val))
                {
                    Console.WriteLine("属性值输入错误，请输入合法的数字。");
                    continue;
                }
                double attributeValue = val;
                tmpCharacter.AddAttribute(attributeName, attributeValue);
                Console.WriteLine($"已添加属性：{attributeName} = {attributeValue}");
            }
            Console.WriteLine($"已创建角色实例：Name:{tmpCharacter.Name}");
            foreach (var attr in tmpCharacter.Attributes)
            {
                Console.WriteLine($"属性：{attr.Key} = {attr.Value}");
            }

            //是否保存角色数据(Y/N)
            string saveChoice = ReadInput("是否保存该角色数据？(Y/N)").ToLower();
            if (!(saveChoice == "y"))
            {
                Console.WriteLine("角色数据未保存，返回添加数据菜单。");
                WaitForConfirmation();
                return;
            }
            //是否进行角色分类(Y/N)
            Console.WriteLine("默认保存路径：GameObj_data/CharacterData.json");
            string SavePath = CharacterDataPath;
            string classifyChoice = ReadInput("是否对该角色进行分类？(Y/N)").ToLower();
            if (classifyChoice == "y")
            {
                string category = ReadInput("请输入角色分类名称");
                if (string.IsNullOrWhiteSpace(category) || category == "null")
                {
                    Console.WriteLine("分类名称错误：为空或为\"null\"，将保存至默认路径。");
                }
                //检查文件夹是否存在，不存在则创建
                else if (!System.IO.Directory.Exists("GameObj_data"))
                {
                    System.IO.Directory.CreateDirectory("GameObj_data");
                    SavePath = $"GameObj_data/{category}_Character.json";
                    Console.WriteLine($"已创建文件夹并设置保存路径为：{SavePath}");
                }
                else
                {
                    SavePath = $"GameObj_data/{category}_Character.json";
                    Console.WriteLine($"已设置保存路径为：{SavePath}");
                }
            }
            //保存角色数据到JSON文件
            //读取整个JSON文件，将新角色添加到列表中，然后写回文件
            List<Character> characters = DataManager.LoadCharacters(SavePath);
            // 检查重名（不区分大小写，忽略首尾空白）
            string newNameKey = tmpCharacter.Name?.Trim() ?? string.Empty;
            var existingIndex = characters.FindIndex(c => string.Equals(c.Name?.Trim(), newNameKey, StringComparison.OrdinalIgnoreCase));
            if (existingIndex >= 0)
            {
                Console.WriteLine($"检测到已存在同名角色: {characters[existingIndex].Name}");
                while (true)
                {
                    string dupChoice = ReadInput("选择操作：覆盖( O ) / 重命名( R ) / 放弃保存( C )").Trim().ToLower();
                    if (dupChoice == "o" || dupChoice == "overwrite")
                    {
                        // 覆盖已有条目
                        characters[existingIndex] = tmpCharacter;
                        try
                        {
                            DataManager.SaveCharacters(characters, SavePath);
                            Console.WriteLine($"角色数据已覆盖保存至：{SavePath}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"保存失败：{ex.Message}");
                        }
                        break;
                    }
                    else if (dupChoice == "r" || dupChoice == "rename")
                    {
                        string newName = ReadInput("请输入新的角色名称：");
                        if (string.IsNullOrWhiteSpace(newName) || newName.Equals("null", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("新名称无效，请重新输入。");
                            continue;
                        }
                        if (characters.Any(c => string.Equals(c.Name?.Trim(), newName.Trim(), StringComparison.OrdinalIgnoreCase)))
                        {
                            Console.WriteLine("该名称已存在，请选择其他名称。\n");
                            continue;
                        }
                        tmpCharacter.Name = newName.Trim();
                        characters.Add(tmpCharacter);
                        try
                        {
                            DataManager.SaveCharacters(characters, SavePath);
                            Console.WriteLine($"角色数据已保存至：{SavePath}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"保存失败：{ex.Message}");
                        }
                        break;
                    }
                    else if (dupChoice == "c" || dupChoice == "cancel")
                    {
                        Console.WriteLine("已放弃保存，返回添加数据菜单。");
                        WaitForConfirmation();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("无效选择，请输入 O / R / C");
                    }
                }
            }
            else
            {
                characters.Add(tmpCharacter);
                try
                {
                    DataManager.SaveCharacters(characters, SavePath);
                    Console.WriteLine($"角色数据已保存至：{SavePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"保存失败：{ex.Message}");
                }
            }
        }

        private async Task Additem()
        {
            Console.WriteLine("=== 添加新道具 ===");
            // 用户输入道具名称
            string name = ReadInput("请输入道具名称");
            while (true)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("道具名称不能为空，请重新输入。");
                    name = ReadInput("请输入道具名称");
                }
                else if (name == "null")
                {
                    Console.WriteLine("道具名称不合法，具体原因详见帮助会话");
                }
                else { break; }
            }

            // 创建一个未储存道具
            Item tmpItem = new Item
            {
                Name = name,
                Attributes = new Dictionary<string, double>()
            };

            // 用户输入道具属性名称
            while (true)
            {
                string attributeName = ReadInput("请输入属性名称（输入 'done' 完成添加）");
                if (string.IsNullOrWhiteSpace(attributeName) || attributeName == "null")
                {
                    Console.WriteLine("属性名称错误：为空或为\"null\"");
                    continue;
                }
                if (attributeName.ToLower() == "done")
                {
                    Console.WriteLine("属性添加结束");
                    break;
                }
                string attributeValueInput = ReadInput($"请输入属性 '{attributeName}' 的数值");
                if (!double.TryParse(attributeValueInput, out double val))
                {
                    Console.WriteLine("属性值输入错误，请输入合法的数字。");
                    continue;
                }
                double attributeValue = val;
                tmpItem.AddAttribute(attributeName, attributeValue);
                Console.WriteLine($"已添加属性：{attributeName} = {attributeValue}");
            }

            Console.WriteLine($"已创建道具实例：Name:{tmpItem.Name}");
            foreach (var attr in tmpItem.Attributes)
            {
                Console.WriteLine($"属性：{attr.Key} = {attr.Value}");
            }

            // 是否保存道具数据(Y/N)
            string saveChoice = ReadInput("是否保存该道具数据？(Y/N)").ToLower();
            if (!(saveChoice == "y"))
            {
                Console.WriteLine("道具数据未保存，返回添加数据菜单。");
                WaitForConfirmation();
                return;
            }

            // 是否进行道具分类(Y/N)
            Console.WriteLine($"默认保存路径：{ItemDataPath}");
            string SavePath = ItemDataPath;
            string classifyChoice = ReadInput("是否对该道具进行分类？(Y/N)").ToLower();
            if (classifyChoice == "y")
            {
                string category = ReadInput("请输入道具分类名称");
                if (string.IsNullOrWhiteSpace(category) || category == "null")
                {
                    Console.WriteLine("分类名称错误：为空或为\"null\"，将保存至默认路径。");
                }
                else if (!System.IO.Directory.Exists("GameObj_data"))
                {
                    System.IO.Directory.CreateDirectory("GameObj_data");
                    SavePath = $"GameObj_data/{category}_Item.json";
                    Console.WriteLine($"已创建文件夹并设置保存路径为：{SavePath}");
                }
                else
                {
                    SavePath = $"GameObj_data/{category}_Item.json";
                    Console.WriteLine($"已设置保存路径为：{SavePath}");
                }
            }

            // 保存道具数据到JSON文件
            List<Item> items = DataManager.LoadItems(SavePath);
            // 检查重名（不区分大小写，忽略首尾空白）
            string newNameKey = tmpItem.Name?.Trim() ?? string.Empty;
            var existingIndex = items.FindIndex(i => string.Equals(i.Name?.Trim(), newNameKey, StringComparison.OrdinalIgnoreCase));
            if (existingIndex >= 0)
            {
                Console.WriteLine($"检测到已存在同名道具: {items[existingIndex].Name}");
                while (true)
                {
                    string dupChoice = ReadInput("选择操作：覆盖( O ) / 重命名( R ) / 放弃保存( C )").Trim().ToLower();
                    if (dupChoice == "o" || dupChoice == "overwrite")
                    {
                        // 覆盖已有条目
                        items[existingIndex] = tmpItem;
                        try
                        {
                            DataManager.SaveItems(items, SavePath);
                            Console.WriteLine($"道具数据已覆盖保存至：{SavePath}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"保存失败：{ex.Message}");
                        }
                        break;
                    }
                    else if (dupChoice == "r" || dupChoice == "rename")
                    {
                        string newName = ReadInput("请输入新的道具名称：");
                        if (string.IsNullOrWhiteSpace(newName) || newName.Equals("null", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("新名称无效，请重新输入。");
                            continue;
                        }
                        if (items.Any(i => string.Equals(i.Name?.Trim(), newName.Trim(), StringComparison.OrdinalIgnoreCase)))
                        {
                            Console.WriteLine("该名称已存在，请选择其他名称。\n");
                            continue;
                        }
                        tmpItem.Name = newName.Trim();
                        items.Add(tmpItem);
                        try
                        {
                            DataManager.SaveItems(items, SavePath);
                            Console.WriteLine($"道具数据已保存至：{SavePath}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"保存失败：{ex.Message}");
                        }
                        break;
                    }
                    else if (dupChoice == "c" || dupChoice == "cancel")
                    {
                        Console.WriteLine("已放弃保存，返回添加数据菜单。");
                        WaitForConfirmation();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("无效选择，请输入 O / R / C");
                    }
                }
            }
            else
            {
                items.Add(tmpItem);
                try
                {
                    DataManager.SaveItems(items, SavePath);
                    Console.WriteLine($"道具数据已保存至：{SavePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"保存失败：{ex.Message}");
                }
            }
        }

        private async Task AddFormula()
        {
            Console.WriteLine("=== 添加新公式 ===");

            // 用户输入公式名称
            string name = ReadInput("请输入公式名称");
            while (true)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("公式名称不能为空，请重新输入。");
                    name = ReadInput("请输入公式名称");
                }
                else if (name == "null")
                {
                    Console.WriteLine("公式名称不合法，具体原因详见帮助会话");
                    name = ReadInput("请输入公式名称");
                }
                else { break; }
            }

            // 用户输入目标属性
            string target = ReadInput("请输入目标属性名称");
            while (string.IsNullOrWhiteSpace(target))
            {
                Console.WriteLine("目标属性名称不能为空，请重新输入。");
                target = ReadInput("请输入目标属性名称");
            }

            Formula tmpFormula = new Formula
            {
                Name = name,
                Form = "null"
            };

            // 用户输入公式表达式
            while (true)
            {
                string formulaExpression = ReadInput("请输入公式表达式");
                if (string.IsNullOrWhiteSpace(formulaExpression))
                {
                    Console.WriteLine("公式表达式不能为空，请重新输入。");
                    continue;
                }
                else
                {
                    // 简单验证公式表达式（可扩展为更复杂的验证逻辑）
                    tmpFormula.Form = formulaExpression;
                    tmpFormula.inspection();

                    if (!tmpFormula.Distinction)
                    {
                        string _saveChoice = ReadInput("公式可算性检验失败，是否储存(Y/N)").ToLower();
                        if (_saveChoice == "y")
                            break;
                        else
                            continue;
                    }
                    else
                    {
                        // 通过验证，继续后续输入
                        break;
                    }
                }
            }

            // 显示用户输入的公式信息
            Console.WriteLine($"\n公式信息预览：");
            Console.WriteLine($"名称: {name}");
            Console.WriteLine($"表达式: {tmpFormula.Form}");
            Console.WriteLine($"目标属性: {target}");
            Console.WriteLine($"可算性检验: {(tmpFormula.Distinction ? "通过" : "未通过")}");

            // 确认是否保存
            string saveChoice = ReadInput("是否保存该公式数据？(Y/N)").ToLower();
            if (!(saveChoice == "y"))
            {
                Console.WriteLine("公式数据未保存，返回添加数据菜单。");
                WaitForConfirmation();
                return;
            }

            // 选择保存路径
            Console.WriteLine($"默认保存路径：{FormulaDataPath}");
            string savePath = FormulaDataPath;
            string classifyChoice = ReadInput("是否对该公式进行分类？(Y/N)").ToLower();
            if (classifyChoice == "y")
            {
                string category = ReadInput("请输入公式分类名称");
                if (string.IsNullOrWhiteSpace(category) || category == "null")
                {
                    Console.WriteLine("分类名称错误：为空或为\"null\"，将保存至默认路径。");
                }
                else
                {
                    // 检查文件夹是否存在，不存在则创建
                    if (!System.IO.Directory.Exists("Formula_data"))
                    {
                        System.IO.Directory.CreateDirectory("Formula_data");
                    }
                    savePath = $"Formula_data/{category}_Formula.json";
                    Console.WriteLine($"已设置保存路径为：{savePath}");
                }
                // 读取JSON文件的数据，验证重名
                List<Formula> formulas = DataManager.LoadFormulas(savePath);
                // 检查重名 （不区分大小写，忽略首尾空白）
                string newNameKey = tmpFormula.Name?.Trim() ?? string.Empty;
                var existingIndex = formulas.FindIndex(f => string.Equals(f.Name?.Trim(), newNameKey, StringComparison.OrdinalIgnoreCase));
                if (existingIndex >= 0)
                {
                    Console.WriteLine($"检测到已存在同名公式: {formulas[existingIndex].Name}");
                    while (true)
                    {
                        string dupChoice = ReadInput("选择操作：覆盖( O ) / 重命名( R ) / 放弃保存( C )").Trim().ToLower();
                        if (dupChoice == "o" || dupChoice == "overwrite")
                        {
                            // 覆盖已有条目
                            formulas[existingIndex] = tmpFormula;
                            try
                            {
                                DataManager.SaveFormula(formulas, savePath);
                                Console.WriteLine($"公式数据已覆盖保存至：{savePath}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"保存失败：{ex.Message}");
                            }
                            break;
                        }
                        else if (dupChoice == "r" || dupChoice == "rename")
                        {
                            string newName = ReadInput("请输入新的公式名称：");
                            if (string.IsNullOrWhiteSpace(newName) || newName.Equals("null", StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine("新名称无效，请重新输入。");
                                continue;
                            }
                            if (formulas.Any(f => string.Equals(f.Name?.Trim(), newName.Trim(), StringComparison.OrdinalIgnoreCase)))
                            {
                                Console.WriteLine("该名称已存在，请选择其他名称。\n");
                                continue;
                            }
                            tmpFormula.Name = newName.Trim();
                            formulas.Add(tmpFormula);
                            try
                            {
                                DataManager.SaveFormula(formulas, savePath);
                                Console.WriteLine($"公式数据已保存至：{savePath}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"保存失败：{ex.Message}");
                            }
                            WaitForConfirmation();
                        }
                        else if (dupChoice == "c" || dupChoice == "cancel")
                        {
                            Console.WriteLine("已放弃保存，返回添加数据菜单。");
                            WaitForConfirmation();
                            return;
                        }
                        else
                        {
                            Console.WriteLine("无效选择，请输入 O / R / C");
                        }
                    }
                }
                else
                {
                    formulas.Add(tmpFormula);
                    try
                    {
                        DataManager.SaveFormula(formulas, savePath);
                        Console.WriteLine($"公式数据已保存至：{savePath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"保存失败：{ex.Message}");
                    }
                    WaitForConfirmation();
                }
            }
            else
            {
                // 读取JSON文件的数据，验证重名
                List<Formula> formulas = DataManager.LoadFormulas(savePath);
                // 检查重名 （不区分大小写，忽略首尾空白）
                string newNameKey = tmpFormula.Name?.Trim() ?? string.Empty;
                var existingIndex = formulas.FindIndex(f => string.Equals(f.Name?.Trim(), newNameKey, StringComparison.OrdinalIgnoreCase));
                if (existingIndex >= 0)
                {
                    Console.WriteLine($"检测到已存在同名公式: {formulas[existingIndex].Name}");
                    while (true)
                    {
                        string dupChoice = ReadInput("选择操作：覆盖( O ) / 重命名( R ) / 放弃保存( C )").Trim().ToLower();
                        if (dupChoice == "o" || dupChoice == "overwrite")
                        {
                            // 覆盖已有条目
                            formulas[existingIndex] = tmpFormula;
                            try
                            {
                                DataManager.SaveFormula(formulas, savePath);
                                Console.WriteLine($"公式数据已覆盖保存至：{savePath}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"保存失败：{ex.Message}");
                            }
                            break;
                        }
                        else if (dupChoice == "r" || dupChoice == "rename")
                        {
                            string newName = ReadInput("请输入新的公式名称：");
                            if (string.IsNullOrWhiteSpace(newName) || newName.Equals("null", StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine("新名称无效，请重新输入。");
                                continue;
                            }
                            if (formulas.Any(f => string.Equals(f.Name?.Trim(), newName.Trim(), StringComparison.OrdinalIgnoreCase)))
                            {
                                Console.WriteLine("该名称已存在，请选择其他名称。\n");
                                continue;
                            }
                            tmpFormula.Name = newName.Trim();
                            formulas.Add(tmpFormula);
                            try
                            {
                                DataManager.SaveFormula(formulas, savePath);
                                Console.WriteLine($"公式数据已保存至：{savePath}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"保存失败：{ex.Message}");
                            }
                            WaitForConfirmation();
                        }
                        else if (dupChoice == "c" || dupChoice == "cancel")
                        {
                            Console.WriteLine("已放弃保存，返回添加数据菜单");
                        }
                    }
                }
                else
                {
                    formulas.Add(tmpFormula);
                    try
                    {
                        DataManager.SaveFormula(formulas, savePath);
                        Console.WriteLine($"公式数据已保存至：{savePath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"保存失败：{ex.Message}");
                    }
                    WaitForConfirmation();
                }
            }
        }
    }
}