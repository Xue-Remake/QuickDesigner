using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickCalculator.Models;

namespace QuickCalculator.Modules
{
    /// <summary>
    /// 设置会话类，管理应用程序的初始设置和配置选项。
    /// 包含计算次数设置和各个数据的管理功能。
    /// </summary>
    public class SetupSession : BaseSession
    {
        //默认路径
        private readonly string DataDirectory = "GameObj_data";
        private readonly string FormulaDirectory = "Formula_data";
        private readonly string CharacterDataPath = "GameObj_data/Default_Character.json"; // 默认角色数据文件路径
        private readonly string ItemDataPath = "GameObj_data/Default_Item.json"; // 默认道具数据文件路径
        private readonly string FormulaDataPath = "Formula_data/Default_Formula.json"; // 默认公式数据文件路径
        //private string FormulaGroupDataPath = "Formula_data/FormulaGroupData.json"; // 公式组功能待实现

        public override string SessionName => "计算器-设置菜单";

        public override async Task<BaseSession> ExecuteAsync()
        {
            DisPlayHeader();
            DisplaySetupMenu();
            string choice = ReadInput("请输入命令");
            switch (choice)
            {
                case "count":
                    await SetCalculationcount();
                    return this;
                case "chara":
                    await CharacterManagementSession();
                    return this;
                case "item":
                    await ItemManagementSession();
                    return this;
                case "formu":
                    await FormulaManagementSession();
                    return this;
                case "back":
                    return null;
                default:
                    Console.WriteLine("无效输入，请重试。");
                    WaitForUser();
                    return this;
            }
        }

        private void DisplaySetupMenu()
        {
            Console.WriteLine($"当前计算次数: {CalculateCount.calculateCount}");
            Console.WriteLine("1. count      -设置计算次数");
            Console.WriteLine("2. chara      -管理角色数据（添加、删除、修改属性）");
            Console.WriteLine("3. item       -管理道具数据（添加、删除、修改属性）");
            Console.WriteLine("4. formu      -管理公式数据（添加、删除、修改公式）");
            //Console.WriteLine("5. group      -管理公式组数据（添加、删除、修改公式组）");
            //group功能计划在v1.2到v1.4版本实现
            Console.WriteLine("5. back       -返回主菜单");
            Console.WriteLine(); // 空行分隔
        }

        private async Task SetCalculationcount()
        {
            string input = ReadInput("请输入计算次数（1-10的正整数）：");
            if (int.TryParse(input, out int count) && count >= 1 && count <= 10)
            {
                CalculateCount.calculateCount = count;
                Console.WriteLine($"计算次数已设置为: {CalculateCount.calculateCount}");
            }
            else
            {
                Console.WriteLine("无效输入，请输入1到10之间的正整数。");
            }
            WaitForConfirmation();
        }

        private async Task CharacterManagementSession()
        {
            List<string> allfiles = FileManager.FindJsonFilesInPath(DataDirectory, "Character");
            Console.WriteLine("找到以下角色数据文件：");
            for (int i = 0; i < allfiles.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {allfiles[i]}");
            }

        //选择设置的文件
        CharaFileCooseNode: //文件选择标记
            string input = ReadInput("请输入要设置的文件编号");
            if (int.TryParse(input, out int fileIndex) && fileIndex > 0 && fileIndex <= allfiles.Count)
            {
                string selectedFile = allfiles[fileIndex - 1];
                //读取并显示数据 
                List<Character> characters = DataManager.LoadCharacters(selectedFile);
                Console.WriteLine("该文件已有数据如下：");
                foreach (var character in characters)
                {
                    Console.WriteLine($"角色名称: {character.Name}");
                    foreach (var prop in character.Attributes)
                    {
                        Console.WriteLine($"{prop.Key}: {prop.Value}");
                    }
                }

            //管理角色数据的逻辑（添加、删除、修改属性）
            //选择要修改的数据
            NodeCharacterChoose: //角色选择标记
                string charaInput = ReadInput("请输入要修改的角色名称，输入\"back\"返回文件选择");
                if (charaInput.ToLower() == "back")
                {
                    goto CharaFileCooseNode;
                    //返回文件选择
                }
                if (characters.Any(c => c.Name == charaInput))
                {
                NodeCharacterPropertyEdit: //角色属性编辑标记
                    Character selectedCharacter = characters.First(c => c.Name == charaInput);
                    //显示该角色当前属性
                    foreach (var prop in selectedCharacter.Attributes)
                    {
                        Console.WriteLine($"{prop.Key}: {prop.Value}");
                    }
                    //选择是添加属性还是修改属性
                    string actionInput = ReadInput("请选择操作：\"a\"(添加属性) | \"e\"(编辑属性) | \"back\"(返回角色选择)");
                    switch (actionInput)
                    {
                        case "a":
                            //添加属性逻辑
                            string newPropName = ReadInput("请输入新属性名称");
                            double Value;
                            while (true)
                            {
                                string newPropValue = ReadInput("请输入新属性值");
                                double.TryParse(newPropValue, out double parsedValue);
                                if (double.TryParse(newPropValue, out parsedValue))
                                {
                                    Value = parsedValue;
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("无效的属性值，请输入数字类型的值。");
                                }
                            }
                            if (!selectedCharacter.Attributes.ContainsKey(newPropName))
                            {
                                selectedCharacter.Attributes[newPropName] = Value;
                                Console.WriteLine($"属性 {newPropName} 已添加，值为 {Value}。");
                            }
                            else
                            {
                                Console.WriteLine("该属性已存在，无法添加。");
                            }
                            goto NodeCharacterPropertyEdit;
                        case "e":
                            //编辑属性逻辑
                            goto EditCharacterPropertyNode;
                        case "back":
                            goto NodeCharacterChoose;
                        default:
                            Console.WriteLine("无效输入，请重试。");
                            goto NodeCharacterPropertyEdit;
                    }
                //输入要修改的属性名称
                EditCharacterPropertyNode: //编辑属性标记
                    string propInput = ReadInput("请输入要修改的属性名称");
                    if (selectedCharacter.Attributes.ContainsKey(propInput))
                    {
                        string valueInput = ReadInput($"请选择操作：\"r\"(移除属性) | \"c\"(更改属性)\" | \"back\"(返回角色编辑)");
                        switch (valueInput)
                        {
                            case "r":
                                selectedCharacter.Attributes.Remove(propInput);
                                Console.WriteLine($"属性 {propInput} 已移除。");
                                goto NodeCharacterPropertyEdit;
                            case "c":
                                double newValue;
                                while (true)
                                {
                                    string newPropValue = ReadInput("请输入新属性值");
                                    double.TryParse(newPropValue, out double parsedValue);
                                    if (double.TryParse(newPropValue, out parsedValue))
                                    {
                                        newValue = parsedValue;
                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine("无效的属性值，请输入数字类型的值。");
                                    }
                                }
                                selectedCharacter.Attributes[propInput] = newValue;
                                Console.WriteLine($"属性 {propInput} 已更新为 {newValue}。");
                                goto NodeCharacterPropertyEdit;
                            case "back":
                                goto NodeCharacterPropertyEdit;
                            default:
                                Console.WriteLine("无效输入，请重试。");
                                goto EditCharacterPropertyNode;
                        }
                    }
                    else
                    {
                        Console.WriteLine("未找到该属性名称，请重试。");
                    }
                }
                else
                {
                    Console.WriteLine("未找到该角色名称，请重试。");
                }
            }
            else
            {
                Console.WriteLine("无效的文件编号。");
            }
        }

        private async Task ItemManagementSession() // AI生成，暂时未审查，测试中未见异常
        {
            List<string> allfiles = FileManager.FindJsonFilesInPath(DataDirectory, "Item");
            Console.WriteLine("找到以下道具数据文件：");
            for (int i = 0; i < allfiles.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {allfiles[i]}");
            }

        //选择设置的文件
        ItemFileChooseNode: //文件选择标记
            string input = ReadInput("请输入要设置的文件编号");
            if (int.TryParse(input, out int fileIndex) && fileIndex > 0 && fileIndex <= allfiles.Count)
            {
                string selectedFile = allfiles[fileIndex - 1];
                //读取并显示数据 
                List<Item> items = DataManager.LoadItems(selectedFile);
                Console.WriteLine("该文件已有数据如下：");
                foreach (var item in items)
                {
                    Console.WriteLine($"道具名称: {item.Name}");
                    foreach (var prop in item.Attributes)
                    {
                        Console.WriteLine($"{prop.Key}: {prop.Value}");
                    }
                }

            //管理道具数据的逻辑（添加、删除、修改属性）
            //选择要修改的数据
            NodeItemChoose: //道具选择标记
                string itemInput = ReadInput("请输入要修改的道具名称，输入\"back\"返回文件选择");
                if (itemInput.ToLower() == "back")
                {
                    goto ItemFileChooseNode;
                    //返回文件选择
                }
                if (items.Any(c => c.Name == itemInput))
                {
                NodeItemPropertyEdit: //道具属性编辑标记
                    Item selectedItem = items.First(c => c.Name == itemInput);
                    //显示该道具当前属性
                    foreach (var prop in selectedItem.Attributes)
                    {
                        Console.WriteLine($"{prop.Key}: {prop.Value}");
                    }
                    //选择是添加属性还是修改属性
                    string actionInput = ReadInput("请选择操作：\"a\"(添加属性) | \"e\"(编辑属性) | \"back\"(返回道具选择)");
                    switch (actionInput)
                    {
                        case "a":
                            //添加属性逻辑
                            string newPropName = ReadInput("请输入新属性名称");
                            double Value;
                            while (true)
                            {
                                string newPropValue = ReadInput("请输入新属性值");
                                double.TryParse(newPropValue, out double parsedValue);
                                if (double.TryParse(newPropValue, out parsedValue))
                                {
                                    Value = parsedValue;
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("无效的属性值，请输入数字类型的值。");
                                }
                            }
                            if (!selectedItem.Attributes.ContainsKey(newPropName))
                            {
                                selectedItem.Attributes[newPropName] = Value;
                                Console.WriteLine($"属性 {newPropName} 已添加，值为 {Value}。");
                            }
                            else
                            {
                                Console.WriteLine("该属性已存在，无法添加。");
                            }
                            goto NodeItemPropertyEdit;
                        case "e":
                            //编辑属性逻辑
                            goto EditItemPropertyNode;
                        case "back":
                            goto NodeItemChoose;
                        default:
                            Console.WriteLine("无效输入，请重试。");
                            goto NodeItemPropertyEdit;
                    }
                //输入要修改的属性名称
                EditItemPropertyNode: //编辑属性标记
                    string propInput = ReadInput("请输入要修改的属性名称");
                    if (selectedItem.Attributes.ContainsKey(propInput))
                    {
                        string valueInput = ReadInput($"请选择操作：\"r\"(移除属性) | \"c\"(更改属性)\" | \"back\"(返回道具编辑)");
                        switch (valueInput)
                        {
                            case "r":
                                selectedItem.Attributes.Remove(propInput);
                                Console.WriteLine($"属性 {propInput} 已移除。");
                                goto NodeItemPropertyEdit;
                            case "c":
                                double newValue;
                                while (true)
                                {
                                    string newPropValue = ReadInput("请输入新属性值");
                                    double.TryParse(newPropValue, out double parsedValue);
                                    if (double.TryParse(newPropValue, out parsedValue))
                                    {
                                        newValue = parsedValue;
                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine("无效的属性值，请输入数字类型的值。");
                                    }
                                }
                                selectedItem.Attributes[propInput] = newValue;
                                Console.WriteLine($"属性 {propInput} 已更新为 {newValue}。");
                                goto NodeItemPropertyEdit;
                            case "back":
                                goto NodeItemPropertyEdit;
                            default:
                                Console.WriteLine("无效输入，请重试。");
                                goto EditItemPropertyNode;
                        }
                    }
                    else
                    {
                        Console.WriteLine("未找到该属性名称，请重试。");
                    }
                }
                else
                {
                    Console.WriteLine("未找到该道具名称，请重试。");
                }
            }
            else
            {
                Console.WriteLine("无效的文件编号。");
            }
        }

        private async Task FormulaManagementSession()// AI生成，测试中未见异常，暂时未审查
        {
            List<string> allfiles = FileManager.FindJsonFilesInPath(FormulaDirectory, "Formula");
            Console.WriteLine("找到以下公式数据文件：");
            for (int i = 0; i < allfiles.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {allfiles[i]}");
            }

        //选择设置的文件
        FormulaFileChooseNode: //文件选择标记
            string input = ReadInput("请输入要设置的文件编号");
            if (int.TryParse(input, out int fileIndex) && fileIndex > 0 && fileIndex <= allfiles.Count)
            {
                string selectedFile = allfiles[fileIndex - 1];
                //读取并显示数据 
                List<Formula> formulas = DataManager.LoadFormulas(selectedFile); //TODO: 实现公式加载方法
                Console.WriteLine("该文件已有数据如下：");
                foreach (var formula in formulas)
                {
                    Console.WriteLine($"公式名称: {formula.Name}");
                    Console.WriteLine($"目标变量: {formula.Target}");
                    Console.WriteLine($"可解析性: {formula.Distinction}");
                    Console.WriteLine($"公式内容: {formula.Form}");
                    Console.WriteLine("------------------------");
                }

            //管理公式数据的逻辑
            //选择要修改的数据
            NodeFormulaChoose: //公式选择标记
                string formulaInput = ReadInput("请输入要修改的公式名称，输入\"back\"返回文件选择");
                if (formulaInput.ToLower() == "back")
                {
                    goto FormulaFileChooseNode;
                    //返回文件选择
                }
                if (formulas.Any(c => c.Name == formulaInput))
                {
                NodeFormulaEdit: //公式编辑标记
                    Formula selectedFormula = formulas.First(c => c.Name == formulaInput);
                    //显示该公式当前数据
                    Console.WriteLine($"公式名称: {selectedFormula.Name}");
                    Console.WriteLine($"目标变量: {selectedFormula.Target}");
                    Console.WriteLine($"可解析性: {selectedFormula.Distinction}");
                    Console.WriteLine($"公式内容: {selectedFormula.Form}");

                    //选择要修改的字段
                    string fieldInput = ReadInput("请选择要修改的字段：\"t\"(目标变量) | \"f\"(公式内容) | \"back\"(返回公式选择)");
                    switch (fieldInput)
                    {
                        case "t":
                            //修改目标变量
                            string newTarget = ReadInput("请输入新的目标变量名称");
                            selectedFormula.Target = newTarget;
                            Console.WriteLine($"目标变量已更新为: {newTarget}");
                            //保存修改后的数据
                            DataManager.SaveFormula(formulas, selectedFile); //TODO: 实现公式保存方法
                            goto NodeFormulaEdit;
                        case "f":
                            //修改公式内容
                            string newForm = ReadInput("请输入新的公式内容");
                            selectedFormula.Form = newForm;
                            Console.WriteLine($"公式内容已更新为: {newForm}");
                            //保存修改后的数据
                            DataManager.SaveFormula(formulas, selectedFile); //TODO: 实现公式保存方法
                            goto NodeFormulaEdit;
                        case "back":
                            goto NodeFormulaChoose;
                        default:
                            Console.WriteLine("无效输入，请重试。");
                            goto NodeFormulaEdit;
                    }
                }
                else
                {
                    Console.WriteLine("未找到该公式名称，请重试。");
                    goto NodeFormulaChoose;
                }
            }
            else
            {
                Console.WriteLine("无效的文件编号。");
            }
        }
    }
}
