using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QuickCalculator
{

    public class Program
    {
        static bool IsInt(string input)
        {
            if (int.TryParse(input, out _))
            {
                return true;
            }
            return false;
        }


        public static void Main(string[] args)
        {
            int Calculation_count = 0; //计算次数
            Console.WriteLine("本批量计算器工具由雪名残Remake.Ver开发.");
            Console.WriteLine("本工具的功能有：添加自定义的角色和道具，并以“角色”--公式--“道具”进行互动");
            Console.WriteLine("最终计算“与道具1进行公式1互动的角色1”--公式3--“与道具2进行公式2互动的角色2”");
            Console.WriteLine("使用“set”指令设置计算次数，使用“choose”选择各种可选项，使用“add”添加自定义实例.");
            Console.WriteLine("更详细的使用方法请输入“?”了解更多。");
        Start: //标记的跳转，完成代码块跳转至此
            Console.WriteLine("请输入指令");

            //选择代码块
            string input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("输入错误：输入为空");
                goto Start; //返回开头
            }
            //~~~~~~~~~~~~~~~~start ?块~~~~~~~~~~~~~~~~
            if (input == "?")
            {
            learnMore:
                Console.WriteLine("选择要详细了解的部分：“set”、“choose”、“check”或“add”.输入“back”回到操作指令");
                string learnMore = Console.ReadLine()?.ToLower();
                if (learnMore == "back")
                    goto Start;
                if (learnMore != "set" && learnMore != "choose" && learnMore != "check" && learnMore != "add")
                {
                    Console.WriteLine("输入错误：不存在这个指令" + learnMore);
                    goto learnMore;
                }
            }
            //----------------end ?块----------------

            //~~~~~~~~~~~~~~~~start Set块~~~~~~~~~~~~~~~~
            if (input?.ToLower() == "set")
            {
            SetNode:
                Console.WriteLine("选择设置项目: count(计算次数) / chara(角色数据) / item(道具数据) / form(公式数据) / back(返回上级)");
                string setChoice = Console.ReadLine()?.ToLower();

                if (setChoice == "back")
                    goto Start;

                // 设置计算次数
                if (setChoice == "count")
                {
                SetCount:
                    Console.WriteLine("请输入计算次数 (正整数，最大10次):");
                    string countInput = Console.ReadLine();

                    if (!IsInt(countInput))
                    {
                        Console.WriteLine("错误：请输入正整数");
                        goto SetCount;
                    }

                    int count = int.Parse(countInput);
                    if (count <= 0)
                    {
                        Console.WriteLine("错误：计算次数必须为正整数");
                        goto SetCount;
                    }
                    if (count > 10)
                    {
                        Console.WriteLine("错误：数据过大，请重新输入 (最大10次)");
                        goto SetCount;
                    }

                    Calculation_count = count;
                    Console.WriteLine($"计算次数已设置为: {Calculation_count}");
                    goto SetNode;
                }

                // 设置角色数据
                else if (setChoice == "chara")
                {
                SetCharaNode:
                    Console.WriteLine("选择角色操作: check(查看数据) / set(设置数据) / back(返回上级)");
                    string charaChoice = Console.ReadLine()?.ToLower();

                    if (charaChoice == "back")
                        goto SetNode;

                    if (charaChoice == "check")
                    {
                        string charFilePath = "GameObj_data/CharacterData.json";
                        List<Character> characters = DataManager.LoadCharacters(charFilePath);

                        if (characters.Count == 0)
                        {
                            Console.WriteLine("角色数据文件为空或不存在");
                        }
                        else
                        {
                            Console.WriteLine("=== 角色数据列表 ===");
                            foreach (var character in characters)
                            {
                                Console.WriteLine($"角色: {character.Name}");
                                Console.WriteLine($"  属性数量: {character.Attributes.Count}");
                                foreach (var attr in character.Attributes)
                                {
                                    Console.WriteLine($"    {attr.Key}: {attr.Value}");
                                }
                                Console.WriteLine("---");
                            }
                        }
                        goto SetCharaNode;
                    }

                    else if (charaChoice == "set")
                    {
                    SetCharaDataNode:
                        Console.WriteLine("选择角色数据操作: del(移除角色) / check(查看角色属性) / add(添加角色属性) / remove(移除角色属性) / alter(更新属性值) / back(返回上级)");
                        string charaDataChoice = Console.ReadLine()?.ToLower();

                        if (charaDataChoice == "back")
                            goto SetCharaNode;

                        string charFilePath = "GameObj_data/CharacterData.json";

                        if (charaDataChoice == "del")
                        {
                            Console.WriteLine("请输入要移除的角色名称:");
                            string charName = Console.ReadLine();

                            if (string.IsNullOrEmpty(charName))
                            {
                                Console.WriteLine("错误：角色名称不能为空");
                                goto SetCharaDataNode;
                            }

                            List<Character> characters = DataManager.LoadCharacters(charFilePath);
                            var characterToRemove = characters.Find(c => c.Name == charName);

                            if (characterToRemove == null)
                            {
                                Console.WriteLine($"错误：角色 '{charName}' 不存在");
                                goto SetCharaDataNode;
                            }

                            characters.Remove(characterToRemove);
                            DataManager.SaveCharacters(characters, charFilePath);
                            Console.WriteLine($"角色 '{charName}' 已移除");
                            goto SetCharaDataNode;
                        }

                        else if (charaDataChoice == "check")
                        {
                            Console.WriteLine("请输入要查看的角色名称:");
                            string charName = Console.ReadLine();

                            if (string.IsNullOrEmpty(charName))
                            {
                                Console.WriteLine("错误：角色名称不能为空");
                                goto SetCharaDataNode;
                            }

                            Character character = DataManager.FindCharacter(charName, charFilePath);

                            if (character.Name == "null")
                            {
                                Console.WriteLine($"错误：角色 '{charName}' 不存在");
                                goto SetCharaDataNode;
                            }

                            Console.WriteLine($"=== 角色 '{charName}' 属性 ===");
                            foreach (var attr in character.Attributes)
                            {
                                Console.WriteLine($"{attr.Key}: {attr.Value}");
                            }
                            goto SetCharaDataNode;
                        }

                        else if (charaDataChoice == "add")
                        {
                            Console.WriteLine("请输入角色名称:");
                            string charName = Console.ReadLine();

                            if (string.IsNullOrEmpty(charName))
                            {
                                Console.WriteLine("错误：角色名称不能为空");
                                goto SetCharaDataNode;
                            }

                            Console.WriteLine("请输入要添加的属性名称:");
                            string attrName = Console.ReadLine();

                            if (string.IsNullOrEmpty(attrName))
                            {
                                Console.WriteLine("错误：属性名称不能为空");
                                goto SetCharaDataNode;
                            }

                            Console.WriteLine("请输入属性值:");
                            string valueInput = Console.ReadLine();

                            if (!double.TryParse(valueInput, out double value))
                            {
                                Console.WriteLine("错误：属性值必须是数字");
                                goto SetCharaDataNode;
                            }

                            DataManager.ChangeCharacter_Add(charName, charFilePath, attrName, value);
                            goto SetCharaDataNode;
                        }

                        else if (charaDataChoice == "remove")
                        {
                            Console.WriteLine("请输入角色名称:");
                            string charName = Console.ReadLine();

                            if (string.IsNullOrEmpty(charName))
                            {
                                Console.WriteLine("错误：角色名称不能为空");
                                goto SetCharaDataNode;
                            }

                            Console.WriteLine("请输入要移除的属性名称:");
                            string attrName = Console.ReadLine();

                            if (string.IsNullOrEmpty(attrName))
                            {
                                Console.WriteLine("错误：属性名称不能为空");
                                goto SetCharaDataNode;
                            }

                            DataManager.ChangeCharacter_Remove(charName, charFilePath, attrName);
                            goto SetCharaDataNode;
                        }

                        else if (charaDataChoice == "alter")
                        {
                            Console.WriteLine("请输入角色名称:");
                            string charName = Console.ReadLine();

                            if (string.IsNullOrEmpty(charName))
                            {
                                Console.WriteLine("错误：角色名称不能为空");
                                goto SetCharaDataNode;
                            }

                            Console.WriteLine("请输入要更新的属性名称:");
                            string attrName = Console.ReadLine();

                            if (string.IsNullOrEmpty(attrName))
                            {
                                Console.WriteLine("错误：属性名称不能为空");
                                goto SetCharaDataNode;
                            }

                            Console.WriteLine("请输入新的属性值:");
                            string valueInput = Console.ReadLine();

                            if (!double.TryParse(valueInput, out double value))
                            {
                                Console.WriteLine("错误：属性值必须是数字");
                                goto SetCharaDataNode;
                            }

                            DataManager.ChangeCharacter_Set(charName, charFilePath, attrName, value);
                            goto SetCharaDataNode;
                        }

                        else
                        {
                            Console.WriteLine("错误：未知的操作");
                            goto SetCharaDataNode;
                        }
                    }

                    else
                    {
                        Console.WriteLine("错误：未知的操作");
                        goto SetCharaNode;
                    }
                }

                // 设置道具数据 (与角色数据逻辑类似)
                else if (setChoice == "item")
                {
                SetItemNode:
                    Console.WriteLine("选择道具操作: check(查看数据) / set(设置数据) / back(返回上级)");
                    string itemChoice = Console.ReadLine()?.ToLower();

                    if (itemChoice == "back")
                        goto SetNode;

                    if (itemChoice == "check")
                    {
                        string itemFilePath = "GameObj_data/ItemData.json";
                        List<Item> items = DataManager.LoadItems(itemFilePath);

                        if (items.Count == 0)
                        {
                            Console.WriteLine("道具数据文件为空或不存在");
                        }
                        else
                        {
                            Console.WriteLine("=== 道具数据列表 ===");
                            foreach (var item in items)
                            {
                                Console.WriteLine($"道具: {item.Name}");
                                Console.WriteLine($"  属性数量: {item.Attributes.Count}");
                                foreach (var attr in item.Attributes)
                                {
                                    Console.WriteLine($"    {attr.Key}: {attr.Value}");
                                }
                                Console.WriteLine("---");
                            }
                        }
                        goto SetItemNode;
                    }

                    else if (itemChoice == "set")
                    {
                    SetItemDataNode:
                        Console.WriteLine("选择道具数据操作: del(移除道具) / check(查看道具属性) / add(添加道具属性) / remove(移除道具属性) / alter(更新属性值) / back(返回上级)");
                        string itemDataChoice = Console.ReadLine()?.ToLower();

                        if (itemDataChoice == "back")
                            goto SetItemNode;

                        string itemFilePath = "GameObj_data/ItemData.json";

                        if (itemDataChoice == "del")
                        {
                            Console.WriteLine("请输入要移除的道具名称:");
                            string itemName = Console.ReadLine();

                            if (string.IsNullOrEmpty(itemName))
                            {
                                Console.WriteLine("错误：道具名称不能为空");
                                goto SetItemDataNode;
                            }

                            List<Item> items = DataManager.LoadItems(itemFilePath);
                            var itemToRemove = items.Find(i => i.Name == itemName);

                            if (itemToRemove == null)
                            {
                                Console.WriteLine($"错误：道具 '{itemName}' 不存在");
                                goto SetItemDataNode;
                            }

                            items.Remove(itemToRemove);
                            DataManager.SaveItems(items, itemFilePath);
                            Console.WriteLine($"道具 '{itemName}' 已移除");
                            goto SetItemDataNode;
                        }

                        else if (itemDataChoice == "check")
                        {
                            Console.WriteLine("请输入要查看的道具名称:");
                            string itemName = Console.ReadLine();

                            if (string.IsNullOrEmpty(itemName))
                            {
                                Console.WriteLine("错误：道具名称不能为空");
                                goto SetItemDataNode;
                            }

                            Item item = DataManager.FindItem(itemName, itemFilePath);

                            if (item.Name == "null")
                            {
                                Console.WriteLine($"错误：道具 '{itemName}' 不存在");
                                goto SetItemDataNode;
                            }

                            Console.WriteLine($"=== 道具 '{itemName}' 属性 ===");
                            foreach (var attr in item.Attributes)
                            {
                                Console.WriteLine($"{attr.Key}: {attr.Value}");
                            }
                            goto SetItemDataNode;
                        }

                        else if (itemDataChoice == "add")
                        {
                            Console.WriteLine("请输入道具名称:");
                            string itemName = Console.ReadLine();

                            if (string.IsNullOrEmpty(itemName))
                            {
                                Console.WriteLine("错误：道具名称不能为空");
                                goto SetItemDataNode;
                            }

                            Console.WriteLine("请输入要添加的属性名称:");
                            string attrName = Console.ReadLine();

                            if (string.IsNullOrEmpty(attrName))
                            {
                                Console.WriteLine("错误：属性名称不能为空");
                                goto SetItemDataNode;
                            }

                            Console.WriteLine("请输入属性值:");
                            string valueInput = Console.ReadLine();

                            if (!double.TryParse(valueInput, out double value))
                            {
                                Console.WriteLine("错误：属性值必须是数字");
                                goto SetItemDataNode;
                            }

                            DataManager.ChangeItem_Add(itemName, itemFilePath, attrName, value);
                            goto SetItemDataNode;
                        }

                        else if (itemDataChoice == "remove")
                        {
                            Console.WriteLine("请输入道具名称:");
                            string itemName = Console.ReadLine();

                            if (string.IsNullOrEmpty(itemName))
                            {
                                Console.WriteLine("错误：道具名称不能为空");
                                goto SetItemDataNode;
                            }

                            Console.WriteLine("请输入要移除的属性名称:");
                            string attrName = Console.ReadLine();

                            if (string.IsNullOrEmpty(attrName))
                            {
                                Console.WriteLine("错误：属性名称不能为空");
                                goto SetItemDataNode;
                            }

                            DataManager.ChangeItem_Remove(itemName, itemFilePath, attrName);
                            goto SetItemDataNode;
                        }

                        else if (itemDataChoice == "alter")
                        {
                            Console.WriteLine("请输入道具名称:");
                            string itemName = Console.ReadLine();

                            if (string.IsNullOrEmpty(itemName))
                            {
                                Console.WriteLine("错误：道具名称不能为空");
                                goto SetItemDataNode;
                            }

                            Console.WriteLine("请输入要更新的属性名称:");
                            string attrName = Console.ReadLine();

                            if (string.IsNullOrEmpty(attrName))
                            {
                                Console.WriteLine("错误：属性名称不能为空");
                                goto SetItemDataNode;
                            }

                            Console.WriteLine("请输入新的属性值:");
                            string valueInput = Console.ReadLine();

                            if (!double.TryParse(valueInput, out double value))
                            {
                                Console.WriteLine("错误：属性值必须是数字");
                                goto SetItemDataNode;
                            }

                            DataManager.ChangeItem_Set(itemName, itemFilePath, attrName, value);
                            goto SetItemDataNode;
                        }

                        else
                        {
                            Console.WriteLine("错误：未知的操作");
                            goto SetItemDataNode;
                        }
                    }

                    else
                    {
                        Console.WriteLine("错误：未知的操作");
                        goto SetItemNode;
                    }
                }

                // 设置公式数据
                else if (setChoice == "form")
                {
                SetFormNode:
                    Console.WriteLine("选择公式操作: check(查看数据) / set(设置数据) / back(返回上级)");
                    string formChoice = Console.ReadLine()?.ToLower();

                    if (formChoice == "back")
                        goto SetNode;

                    if (formChoice == "check")
                    {
                        string formulaFilePath = "GameObj_data/FormulaData.json";
                        List<Formula> formulas = DataManager.LoadFormulas(formulaFilePath);

                        if (formulas.Count == 0)
                        {
                            Console.WriteLine("公式数据文件为空或不存在");
                        }
                        else
                        {
                            Console.WriteLine("=== 公式数据列表 ===");
                            foreach (var formula in formulas)
                            {
                                Console.WriteLine($"公式: {formula.Name}");
                                Console.WriteLine($"  目标属性: {formula.Target}");
                                Console.WriteLine($"  公式内容: {formula.Form}");
                                Console.WriteLine($"  可算性: {formula.Distinction}");
                                Console.WriteLine("---");
                            }
                        }
                        goto SetFormNode;
                    }

                    else if (formChoice == "set")
                    {
                    SetFormDataNode:
                        Console.WriteLine("选择公式数据操作: del(移除公式) / check(查看公式内容) / content(设置公式内容) / back(返回上级)");
                        string formDataChoice = Console.ReadLine()?.ToLower();

                        if (formDataChoice == "back")
                            goto SetFormNode;

                        string formulaFilePath = "GameObj_data/FormulaData.json";

                        if (formDataChoice == "del")
                        {
                            Console.WriteLine("请输入要移除的公式名称:");
                            string formName = Console.ReadLine();

                            if (string.IsNullOrEmpty(formName))
                            {
                                Console.WriteLine("错误：公式名称不能为空");
                                goto SetFormDataNode;
                            }

                            List<Formula> formulas = DataManager.LoadFormulas(formulaFilePath);
                            var formulaToRemove = formulas.Find(f => f.Name == formName);

                            if (formulaToRemove == null)
                            {
                                Console.WriteLine($"错误：公式 '{formName}' 不存在");
                                goto SetFormDataNode;
                            }

                            formulas.Remove(formulaToRemove);
                            DataManager.SaveFormula(formulas, formulaFilePath);
                            Console.WriteLine($"公式 '{formName}' 已移除");
                            goto SetFormDataNode;
                        }

                        else if (formDataChoice == "check")
                        {
                            Console.WriteLine("请输入要查看的公式名称:");
                            string formName = Console.ReadLine();

                            if (string.IsNullOrEmpty(formName))
                            {
                                Console.WriteLine("错误：公式名称不能为空");
                                goto SetFormDataNode;
                            }

                            Formula formula = DataManager.FindFormula(formName, formulaFilePath);

                            if (formula.Name == null)
                            {
                                Console.WriteLine($"错误：公式 '{formName}' 不存在");
                                goto SetFormDataNode;
                            }

                            Console.WriteLine($"=== 公式 '{formName}' 详细信息 ===");
                            Console.WriteLine($"名称: {formula.Name}");
                            Console.WriteLine($"目标属性: {formula.Target}");
                            Console.WriteLine($"公式内容: {formula.Form}");
                            Console.WriteLine($"可算性: {formula.Distinction}");
                            goto SetFormDataNode;
                        }

                        else if (formDataChoice == "content")
                        {
                        SetFormContentNode:
                            Console.WriteLine("选择公式内容设置: form(设置公式字符串) / target(设置改变值关键字) / back(返回上级)");
                            string formContentChoice = Console.ReadLine()?.ToLower();

                            if (formContentChoice == "back")
                                goto SetFormDataNode;

                            if (formContentChoice == "form")
                            {
                                Console.WriteLine("请输入公式名称:");
                                string formName = Console.ReadLine();

                                if (string.IsNullOrEmpty(formName))
                                {
                                    Console.WriteLine("错误：公式名称不能为空");
                                    goto SetFormContentNode;
                                }

                                Console.WriteLine("请输入新的公式内容:");
                                string newForm = Console.ReadLine();

                                if (string.IsNullOrEmpty(newForm))
                                {
                                    Console.WriteLine("错误：公式内容不能为空");
                                    goto SetFormContentNode;
                                }

                                // 重新创建公式并进行可算性检验
                                Formula updatedFormula = new Formula(formName, newForm);
                                updatedFormula.Target = DataManager.FindFormula(formName, formulaFilePath).Target; // 保持原有目标
                                updatedFormula.inspection();

                                if (!updatedFormula.Distinction)
                                {
                                    Console.WriteLine("公式检验失败，请修改公式内容");
                                    goto SetFormContentNode;
                                }

                                // 更新公式列表
                                List<Formula> formulas = DataManager.LoadFormulas(formulaFilePath);
                                var existingFormula = formulas.Find(f => f.Name == formName);

                                if (existingFormula != null)
                                {
                                    existingFormula.Form = newForm;
                                    existingFormula.Distinction = true;
                                    DataManager.SaveFormula(formulas, formulaFilePath);
                                    Console.WriteLine($"公式 '{formName}' 内容已更新");
                                }
                                else
                                {
                                    Console.WriteLine($"错误：公式 '{formName}' 不存在");
                                }
                                goto SetFormContentNode;
                            }

                            else if (formContentChoice == "target")
                            {
                                Console.WriteLine("请输入公式名称:");
                                string formName = Console.ReadLine();

                                if (string.IsNullOrEmpty(formName))
                                {
                                    Console.WriteLine("错误：公式名称不能为空");
                                    goto SetFormContentNode;
                                }

                                Console.WriteLine("请输入新的目标属性:");
                                string newTarget = Console.ReadLine();

                                if (string.IsNullOrEmpty(newTarget))
                                {
                                    Console.WriteLine("错误：目标属性不能为空");
                                    goto SetFormContentNode;
                                }

                                // 更新目标属性
                                List<Formula> formulas = DataManager.LoadFormulas(formulaFilePath);
                                var existingFormula = formulas.Find(f => f.Name == formName);

                                if (existingFormula != null)
                                {
                                    existingFormula.Target = newTarget;
                                    DataManager.SaveFormula(formulas, formulaFilePath);
                                    Console.WriteLine($"公式 '{formName}' 目标属性已更新为: {newTarget}");
                                }
                                else
                                {
                                    Console.WriteLine($"错误：公式 '{formName}' 不存在");
                                }
                                goto SetFormContentNode;
                            }

                            else
                            {
                                Console.WriteLine("错误：未知的操作");
                                goto SetFormContentNode;
                            }
                        }

                        else
                        {
                            Console.WriteLine("错误：未知的操作");
                            goto SetFormDataNode;
                        }
                    }

                    else
                    {
                        Console.WriteLine("错误：未知的操作");
                        goto SetFormNode;
                    }
                }

                else
                {
                    Console.WriteLine("错误：未知的设置项目");
                    goto SetNode;
                }
            }
            //----------------end Set块----------------
            //~~~~~~~~~~~~~~~~start Add块~~~~~~~~~~~~~~~~
            if (input?.ToLower() == "add")
            {
            NodeOfAdd: //跳转标记
                Console.WriteLine("输入要加入的实例类别 (chara/item/form)");
                string chooseClass = Console.ReadLine()?.ToLower();
                if (chooseClass != "chara" && chooseClass != "item" && chooseClass != "form")
                {
                    Console.WriteLine("输入错误：不存在可添加的实例类别: " + chooseClass);
                    goto NodeOfAdd;
                }

                // 定义文件路径
                string charFilePath = "GameObj_data/CharacterData.json";
                string itemFilePath = "GameObj_data/ItemData.json";
                string formulaFilePath = "GameObj_data/FormulaData.json";

                if (chooseClass == "chara")
                {
                    string SetName;
                begin_setCharacter:
                    Console.WriteLine("输入Character实例的名称");
                    string? InputName = Console.ReadLine();
                    if (string.IsNullOrEmpty(InputName))
                    {
                        Console.WriteLine("错误，输入为空");
                        goto begin_setCharacter;
                    }
                    if (InputName == "null")
                    {
                        Console.WriteLine("命名非法：null命名是统一的空对象/属性名称");
                        goto begin_setCharacter;
                    }
                    SetName = InputName;
                    Character newChar = new Character(SetName);

                setAttribute:
                    Console.WriteLine("输入属性名称（输入\"done\"结束添加属性）");
                    string? attrName = Console.ReadLine();
                    if (attrName?.ToLower() == "done")
                    {
                        // 确保目录存在
                        string directory = Path.GetDirectoryName(charFilePath);
                        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        // 保存角色到文件
                        List<Character> existingChars = DataManager.LoadCharacters(charFilePath);

                        // 检查是否已存在同名角色
                        if (existingChars.Any(c => c.Name == SetName))
                        {
                            Console.WriteLine($"错误：已存在名为 {SetName} 的角色");
                            goto begin_setCharacter;
                        }

                        existingChars.Add(newChar);
                        DataManager.SaveCharacters(existingChars, charFilePath);

                        // 添加调试信息
                        Console.WriteLine($"角色 {SetName} 已保存到 {charFilePath}");
                        Console.WriteLine($"文件是否存在: {File.Exists(charFilePath)}");

                        // 验证保存的数据
                        List<Character> verifyChars = DataManager.LoadCharacters(charFilePath);
                        Console.WriteLine($"文件中现有角色数量: {verifyChars.Count}");

                        goto Start;
                    }

                    if (string.IsNullOrEmpty(attrName))
                    {
                        Console.WriteLine("属性名称不能为空");
                        goto setAttribute;
                    }

                    Console.WriteLine($"输入属性 {attrName} 的值");
                    string? attrValueStr = Console.ReadLine();
                    if (!double.TryParse(attrValueStr, out double attrValue))
                    {
                        Console.WriteLine("属性值必须是数字");
                        goto setAttribute;
                    }

                    newChar.AddAttribute(attrName, attrValue);
                    Console.WriteLine($"属性 {attrName} = {attrValue} 已添加");
                    goto setAttribute;
                }
                else if (chooseClass == "item")
                {
                    string SetName;
                begin_setItem:
                    Console.WriteLine("输入Item实例的名称");
                    string? InputName = Console.ReadLine();
                    if (string.IsNullOrEmpty(InputName))
                    {
                        Console.WriteLine("错误，输入为空");
                        goto begin_setItem;
                    }
                    if (InputName == "null")
                    {
                        Console.WriteLine("命名非法：null命名是统一的空对象/属性名称");
                        goto begin_setItem;
                    }
                    SetName = InputName;
                    Item newItem = new Item(SetName);

                setItemAttribute:
                    Console.WriteLine("输入属性名称（输入\"done\"结束添加属性）");
                    string? attrName = Console.ReadLine();
                    if (attrName?.ToLower() == "done")
                    {
                        // 确保目录存在
                        string directory = Path.GetDirectoryName(itemFilePath);
                        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        // 保存道具到文件
                        List<Item> existingItems = DataManager.LoadItems(itemFilePath);

                        // 检查是否已存在同名道具
                        if (existingItems.Any(i => i.Name == SetName))
                        {
                            Console.WriteLine($"错误：已存在名为 {SetName} 的道具");
                            goto begin_setItem;
                        }

                        existingItems.Add(newItem);
                        DataManager.SaveItems(existingItems, itemFilePath);

                        // 添加调试信息
                        Console.WriteLine($"道具 {SetName} 已保存到 {itemFilePath}");
                        Console.WriteLine($"文件是否存在: {File.Exists(itemFilePath)}");

                        goto Start;
                    }

                    if (string.IsNullOrEmpty(attrName))
                    {
                        Console.WriteLine("属性名称不能为空");
                        goto setItemAttribute;
                    }

                    Console.WriteLine($"输入属性 {attrName} 的值");
                    string? attrValueStr = Console.ReadLine();
                    if (!double.TryParse(attrValueStr, out double attrValue))
                    {
                        Console.WriteLine("属性值必须是数字");
                        goto setItemAttribute;
                    }

                    newItem.AddAttribute(attrName, attrValue);
                    Console.WriteLine($"属性 {attrName} = {attrValue} 已添加");
                    goto setItemAttribute;
                }
                else if (chooseClass == "form")
                {
                begin_setFormula:
                    Console.WriteLine("输入Formula实例的名称");
                    string? formulaName = Console.ReadLine();
                    if (string.IsNullOrEmpty(formulaName))
                    {
                        Console.WriteLine("错误，输入为空");
                        goto begin_setFormula;
                    }
                    if (formulaName == "null")
                    {
                        Console.WriteLine("命名非法：null命名是统一的空对象/属性名称");
                        goto begin_setFormula;
                    }

                    Console.WriteLine("输入公式目标属性名称");
                    string? target = Console.ReadLine();
                    if (string.IsNullOrEmpty(target))
                    {
                        Console.WriteLine("目标属性不能为空");
                        goto begin_setFormula;
                    }

                    Console.WriteLine("输入公式表达式（使用空格分隔，例如：( first_attack + second_defense ) * 0.5 ）");
                    string? formulaExpression = Console.ReadLine();
                    if (string.IsNullOrEmpty(formulaExpression))
                    {
                        Console.WriteLine("公式表达式不能为空");
                        goto begin_setFormula;
                    }

                    // 创建公式并进行可算性检验
                    Formula newFormula = new Formula(formulaName, formulaExpression);
                    newFormula.Target = target;
                    newFormula.inspection();

                    if (!newFormula.Distinction)
                    {
                        Console.WriteLine("公式检验失败，请重新输入");
                        goto begin_setFormula;
                    }

                    // 确保目录存在
                    string directory = Path.GetDirectoryName(formulaFilePath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // 保存公式到文件
                    List<Formula> existingFormulas = DataManager.LoadFormulas(formulaFilePath);

                    // 检查是否已存在同名公式
                    if (existingFormulas.Any(f => f.Name == formulaName))
                    {
                        Console.WriteLine($"错误：已存在名为 {formulaName} 的公式");
                        goto begin_setFormula;
                    }

                    existingFormulas.Add(newFormula);
                    DataManager.SaveFormula(existingFormulas, formulaFilePath);

                    // 添加调试信息
                    Console.WriteLine($"公式 {formulaName} 已保存到 {formulaFilePath}");
                    Console.WriteLine($"文件是否存在: {File.Exists(formulaFilePath)}");

                    goto Start;
                }
            }
            //----------------end Add块----------------
            //~~~~~~~~~~~~~~~~start check块~~~~~~~~~~~~~~~~
            if (input?.ToLower() == "check")
            {
            CheckNode:
                Console.WriteLine("选择查看项目: chara(角色数据) / item(道具数据) / form(公式数据) / back(返回上级)");
                string checkChoice = Console.ReadLine()?.ToLower();

                if (checkChoice == "back")
                    goto Start;

                // 查看角色数据
                if (checkChoice == "chara")
                {
                CheckCharaNode:
                    Console.WriteLine("选择角色查看操作: list(查看角色列表) / data(查看角色属性) / back(返回上级)");
                    string charaCheckChoice = Console.ReadLine()?.ToLower();

                    if (charaCheckChoice == "back")
                        goto CheckNode;

                    string charFilePath = "GameObj_data/CharacterData.json";

                    if (charaCheckChoice == "list")
                    {
                        List<Character> characters = DataManager.LoadCharacters(charFilePath);

                        if (characters.Count == 0)
                        {
                            Console.WriteLine("角色数据文件为空或不存在");
                        }
                        else
                        {
                            Console.WriteLine("=== 角色列表 ===");
                            foreach (var character in characters)
                            {
                                Console.WriteLine($"- {character.Name}");
                            }
                            Console.WriteLine($"总计: {characters.Count} 个角色");
                        }
                        goto CheckCharaNode;
                    }

                    else if (charaCheckChoice == "data")
                    {
                    CheckCharaDataNode:
                        Console.WriteLine("请输入要查看的角色名称 (输入 back 返回上级):");
                        string charName = Console.ReadLine();

                        if (charName?.ToLower() == "back")
                            goto CheckCharaNode;

                        if (string.IsNullOrEmpty(charName))
                        {
                            Console.WriteLine("错误：角色名称不能为空");
                            goto CheckCharaDataNode;
                        }

                        Character character = DataManager.FindCharacter(charName, charFilePath);

                        if (character.Name == "null")
                        {
                            Console.WriteLine($"错误：角色 '{charName}' 不存在");
                            goto CheckCharaDataNode;
                        }

                        Console.WriteLine($"=== 角色 '{charName}' 属性详情 ===");
                        if (character.Attributes.Count == 0)
                        {
                            Console.WriteLine("该角色没有任何属性");
                        }
                        else
                        {
                            foreach (var attr in character.Attributes)
                            {
                                Console.WriteLine($"{attr.Key}: {attr.Value}");
                            }
                            Console.WriteLine($"总计: {character.Attributes.Count} 个属性");
                        }
                        goto CheckCharaDataNode;
                    }

                    else
                    {
                        Console.WriteLine("错误：未知的操作");
                        goto CheckCharaNode;
                    }
                }

                // 查看道具数据
                else if (checkChoice == "item")
                {
                CheckItemNode:
                    Console.WriteLine("选择道具查看操作: list(查看道具列表) / data(查看道具属性) / back(返回上级)");
                    string itemCheckChoice = Console.ReadLine()?.ToLower();

                    if (itemCheckChoice == "back")
                        goto CheckNode;

                    string itemFilePath = "GameObj_data/ItemData.json";

                    if (itemCheckChoice == "list")
                    {
                        List<Item> items = DataManager.LoadItems(itemFilePath);

                        if (items.Count == 0)
                        {
                            Console.WriteLine("道具数据文件为空或不存在");
                        }
                        else
                        {
                            Console.WriteLine("=== 道具列表 ===");
                            foreach (var item in items)
                            {
                                Console.WriteLine($"- {item.Name}");
                            }
                            Console.WriteLine($"总计: {items.Count} 个道具");
                        }
                        goto CheckItemNode;
                    }

                    else if (itemCheckChoice == "data")
                    {
                    CheckItemDataNode:
                        Console.WriteLine("请输入要查看的道具名称 (输入 back 返回上级):");
                        string itemName = Console.ReadLine();

                        if (itemName?.ToLower() == "back")
                            goto CheckItemNode;

                        if (string.IsNullOrEmpty(itemName))
                        {
                            Console.WriteLine("错误：道具名称不能为空");
                            goto CheckItemDataNode;
                        }

                        Item item = DataManager.FindItem(itemName, itemFilePath);

                        if (item.Name == "null")
                        {
                            Console.WriteLine($"错误：道具 '{itemName}' 不存在");
                            goto CheckItemDataNode;
                        }

                        Console.WriteLine($"=== 道具 '{itemName}' 属性详情 ===");
                        if (item.Attributes.Count == 0)
                        {
                            Console.WriteLine("该道具没有任何属性");
                        }
                        else
                        {
                            foreach (var attr in item.Attributes)
                            {
                                Console.WriteLine($"{attr.Key}: {attr.Value}");
                            }
                            Console.WriteLine($"总计: {item.Attributes.Count} 个属性");
                        }
                        goto CheckItemDataNode;
                    }

                    else
                    {
                        Console.WriteLine("错误：未知的操作");
                        goto CheckItemNode;
                    }
                }

                // 查看公式数据
                else if (checkChoice == "form")
                {
                CheckFormNode:
                    Console.WriteLine("选择公式查看操作: list(查看公式列表) / data(查看公式详情) / back(返回上级)");
                    string formCheckChoice = Console.ReadLine()?.ToLower();

                    if (formCheckChoice == "back")
                        goto CheckNode;

                    string formulaFilePath = "GameObj_data/FormulaData.json";

                    if (formCheckChoice == "list")
                    {
                        List<Formula> formulas = DataManager.LoadFormulas(formulaFilePath);

                        if (formulas.Count == 0)
                        {
                            Console.WriteLine("公式数据文件为空或不存在");
                        }
                        else
                        {
                            Console.WriteLine("=== 公式列表 ===");
                            foreach (var formula in formulas)
                            {
                                Console.WriteLine($"- {formula.Name}");
                            }
                            Console.WriteLine($"总计: {formulas.Count} 个公式");
                        }
                        goto CheckFormNode;
                    }

                    else if (formCheckChoice == "data")
                    {
                    CheckFormDataNode:
                        Console.WriteLine("请输入要查看的公式名称 (输入 back 返回上级):");
                        string formName = Console.ReadLine();

                        if (formName?.ToLower() == "back")
                            goto CheckFormNode;

                        if (string.IsNullOrEmpty(formName))
                        {
                            Console.WriteLine("错误：公式名称不能为空");
                            goto CheckFormDataNode;
                        }

                        Formula formula = DataManager.FindFormula(formName, formulaFilePath);

                        if (formula.Name == null)
                        {
                            Console.WriteLine($"错误：公式 '{formName}' 不存在");
                            goto CheckFormDataNode;
                        }

                        Console.WriteLine($"=== 公式 '{formName}' 详情 ===");
                        Console.WriteLine($"公式名称: {formula.Name}");
                        Console.WriteLine($"公式表达式: {formula.Form}");
                        Console.WriteLine($"改变值关键字: {formula.Target}");
                        Console.WriteLine($"是否可算: {(formula.Distinction ? "是" : "否")}");
                        goto CheckFormDataNode;
                    }

                    else
                    {
                        Console.WriteLine("错误：未知的操作");
                        goto CheckFormNode;
                    }
                }

                else
                {
                    Console.WriteLine("错误：未知的查看项目");
                    goto CheckNode;
                }
            }
            //----------------end check块----------------

            //~~~~~~~~~~~~~~~~start calculate块~~~~~~~~~~~~~~~~
            if (input?.ToLower() == "calculate")
            {
                Console.WriteLine("警告：进入计算模式后将无法中途退出，必须完成所有计算设置和计算流程");
                Console.WriteLine("是否继续？ (Y/N)");
                string confirm = Console.ReadLine()?.ToLower();

                if (confirm != "y")
                {
                    Console.WriteLine("已取消进入计算模式");
                    goto Start;
                }

                // 初始化计算次数和计算器实例列表
                if (Calculation_count <= 0)
                {
                    Calculation_count = 1;
                    Console.WriteLine("计算次数未设置，使用默认值: 1");
                }
                else
                {
                    Console.WriteLine($"设置的计算次数为: {Calculation_count}");
                }

                // 创建计算器实例列表
                List<Calculator> calculators = new List<Calculator>();
                for (int i = 0; i < Calculation_count; i++)
                {
                    calculators.Add(new Calculator());
                }

                // 定义文件路径
                string charFilePath = "GameObj_data/CharacterData.json";
                string itemFilePath = "GameObj_data/ItemData.json";
                string formulaFilePath = "GameObj_data/FormulaData.json";

            CalculateMain:
                Console.WriteLine("选择操作: choose(选择设置计算实例) / go(进行计算)");
                string calculateChoice = Console.ReadLine()?.ToLower();

                if (calculateChoice == "choose")
                {
                ChooseCalculator:
                    Console.WriteLine($"请输入要设置的计算实例编号 (1-{Calculation_count}) 或输入 back 返回:");
                    string calcInput = Console.ReadLine();

                    if (calcInput?.ToLower() == "back")
                        goto CalculateMain;

                    if (!int.TryParse(calcInput, out int calcIndex) || calcIndex < 1 || calcIndex > Calculation_count)
                    {
                        Console.WriteLine($"错误：请输入 1-{Calculation_count} 之间的数字");
                        goto ChooseCalculator;
                    }

                    Calculator currentCalc = calculators[calcIndex - 1];

                ChooseCalculatorDetail:
                    Console.WriteLine($"设置计算实例 #{calcIndex} - 选择操作: check(查看当前设置) / chara(设置角色) / item(设置道具) / form(设置公式) / back(返回)");
                    string detailChoice = Console.ReadLine()?.ToLower();

                    if (detailChoice == "back")
                        goto ChooseCalculator;

                    if (detailChoice == "check")
                    {
                        Console.WriteLine($"=== 计算实例 #{calcIndex} 当前设置 ===");
                        Console.WriteLine($"角色1: {(currentCalc.character1.Name != "null" ? currentCalc.character1.Name : "未设置")}");
                        Console.WriteLine($"角色2: {(currentCalc.character2.Name != "null" ? currentCalc.character2.Name : "未设置")}");
                        Console.WriteLine($"道具1: {(currentCalc.item1.Name != "null" ? currentCalc.item1.Name : "未设置")}");
                        Console.WriteLine($"道具2: {(currentCalc.item2.Name != "null" ? currentCalc.item2.Name : "未设置")}");
                        Console.WriteLine($"公式1: {(currentCalc.Formula1.Name != null ? currentCalc.Formula1.Name : "未设置")}");
                        Console.WriteLine($"公式2: {(currentCalc.Formula2.Name != null ? currentCalc.Formula2.Name : "未设置")}");
                        Console.WriteLine($"公式3: {(currentCalc.Formula3.Name != null ? currentCalc.Formula3.Name : "未设置")}");
                        goto ChooseCalculatorDetail;
                    }

                    else if (detailChoice == "chara")
                    {
                    ChooseChara:
                        Console.WriteLine("设置角色: fir(角色1) / sec(角色2) / back(返回)");
                        string charaChoice = Console.ReadLine()?.ToLower();

                        if (charaChoice == "back")
                            goto ChooseCalculatorDetail;

                        // 加载可用的角色列表
                        List<Character> availableChars = DataManager.LoadCharacters(charFilePath);
                        if (availableChars.Count == 0)
                        {
                            Console.WriteLine("错误：没有可用的角色数据");
                            goto ChooseChara;
                        }

                        Console.WriteLine("=== 可用角色列表 ===");
                        foreach (var character in availableChars)
                        {
                            Console.WriteLine($"- {character.Name}");
                        }

                        if (charaChoice == "fir" || charaChoice == "sec")
                        {
                            Console.WriteLine($"请输入要设置为角色{(charaChoice == "fir" ? "1" : "2")}的角色名称:");
                            string charName = Console.ReadLine();

                            if (string.IsNullOrEmpty(charName))
                            {
                                Console.WriteLine("错误：角色名称不能为空");
                                goto ChooseChara;
                            }

                            Character selectedChar = DataManager.FindCharacter(charName, charFilePath);
                            if (selectedChar.Name == "null")
                            {
                                Console.WriteLine($"错误：角色 '{charName}' 不存在");
                                goto ChooseChara;
                            }

                            if (charaChoice == "fir")
                            {
                                currentCalc.SetCharacter1(charName, charFilePath);
                                Console.WriteLine($"角色1 已设置为: {charName}");
                            }
                            else
                            {
                                currentCalc.Setcharacter2(charName, charFilePath);
                                Console.WriteLine($"角色2 已设置为: {charName}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("错误：未知的操作");
                        }
                        goto ChooseChara;
                    }

                    else if (detailChoice == "item")
                    {
                    ChooseItem:
                        Console.WriteLine("设置道具: fir(道具1) / sec(道具2) / back(返回)");
                        string itemChoice = Console.ReadLine()?.ToLower();

                        if (itemChoice == "back")
                            goto ChooseCalculatorDetail;

                        // 加载可用的道具列表
                        List<Item> availableItems = DataManager.LoadItems(itemFilePath);
                        if (availableItems.Count == 0)
                        {
                            Console.WriteLine("错误：没有可用的道具数据");
                            goto ChooseItem;
                        }

                        Console.WriteLine("=== 可用道具列表 ===");
                        foreach (var item in availableItems)
                        {
                            Console.WriteLine($"- {item.Name}");
                        }

                        if (itemChoice == "fir" || itemChoice == "sec")
                        {
                            Console.WriteLine($"请输入要设置为道具{(itemChoice == "fir" ? "1" : "2")}的道具名称:");
                            string itemName = Console.ReadLine();

                            if (string.IsNullOrEmpty(itemName))
                            {
                                Console.WriteLine("错误：道具名称不能为空");
                                goto ChooseItem;
                            }

                            Item selectedItem = DataManager.FindItem(itemName, itemFilePath);
                            if (selectedItem.Name == "null")
                            {
                                Console.WriteLine($"错误：道具 '{itemName}' 不存在");
                                goto ChooseItem;
                            }

                            if (itemChoice == "fir")
                            {
                                currentCalc.Setitem1(itemName, itemFilePath);
                                Console.WriteLine($"道具1 已设置为: {itemName}");
                            }
                            else
                            {
                                currentCalc.Setitem2(itemName, itemFilePath);
                                Console.WriteLine($"道具2 已设置为: {itemName}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("错误：未知的操作");
                        }
                        goto ChooseItem;
                    }

                    else if (detailChoice == "form")
                    {
                    ChooseForm:
                        Console.WriteLine("设置公式: fir(公式1) / sec(公式2) / thi(公式3) / back(返回)");
                        string formChoice = Console.ReadLine()?.ToLower();

                        if (formChoice == "back")
                            goto ChooseCalculatorDetail;

                        // 加载可用的公式列表
                        List<Formula> availableFormulas = DataManager.LoadFormulas(formulaFilePath);
                        if (availableFormulas.Count == 0)
                        {
                            Console.WriteLine("错误：没有可用的公式数据");
                            goto ChooseForm;
                        }

                        Console.WriteLine("=== 可用公式列表 ===");
                        foreach (var formula in availableFormulas)
                        {
                            Console.WriteLine($"- {formula.Name} (目标: {formula.Target}, 可算: {(formula.Distinction ? "是" : "否")})");
                        }

                        if (formChoice == "fir" || formChoice == "sec" || formChoice == "thi")
                        {
                            Console.WriteLine($"请输入要设置为公式{(formChoice == "fir" ? "1" : formChoice == "sec" ? "2" : "3")}的公式名称:");
                            string formName = Console.ReadLine();

                            if (string.IsNullOrEmpty(formName))
                            {
                                Console.WriteLine("错误：公式名称不能为空");
                                goto ChooseForm;
                            }

                            Formula selectedForm = DataManager.FindFormula(formName, formulaFilePath);
                            if (selectedForm.Name == null)
                            {
                                Console.WriteLine($"错误：公式 '{formName}' 不存在");
                                goto ChooseForm;
                            }

                            if (!selectedForm.Distinction)
                            {
                                Console.WriteLine($"错误：公式 '{formName}' 不可计算");
                                goto ChooseForm;
                            }

                            if (formChoice == "fir")
                            {
                                currentCalc.Setformula1(formName, formulaFilePath);
                                Console.WriteLine($"公式1 已设置为: {formName}");
                            }
                            else if (formChoice == "sec")
                            {
                                currentCalc.Setformula2(formName, formulaFilePath);
                                Console.WriteLine($"公式2 已设置为: {formName}");
                            }
                            else
                            {
                                currentCalc.Setformula3(formName, formulaFilePath);
                                Console.WriteLine($"公式3 已设置为: {formName}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("错误：未知的操作");
                        }
                        goto ChooseForm;
                    }

                    else
                    {
                        Console.WriteLine("错误：未知的操作");
                        goto ChooseCalculatorDetail;
                    }
                }

                else if (calculateChoice == "go")
                {
                    // 检查所有计算实例是否设置完整
                    bool allComplete = true;
                    List<string> incompleteInstances = new List<string>();

                    for (int i = 0; i < calculators.Count; i++)
                    {
                        var calc = calculators[i];
                        List<string> missing = new List<string>();

                        if (calc.character1.Name == "null") missing.Add("角色1");
                        if (calc.character2.Name == "null") missing.Add("角色2");
                        if (calc.item1.Name == "null") missing.Add("道具1");
                        if (calc.item2.Name == "null") missing.Add("道具2");
                        if (calc.Formula1.Name == null) missing.Add("公式1");
                        if (calc.Formula2.Name == null) missing.Add("公式2");
                        if (calc.Formula3.Name == null) missing.Add("公式3");

                        if (missing.Count > 0)
                        {
                            allComplete = false;
                            incompleteInstances.Add($"实例 #{i + 1}: 缺少 {string.Join(", ", missing)}");
                        }
                    }

                    if (!allComplete)
                    {
                        Console.WriteLine("错误：以下计算实例设置不完整:");
                        foreach (var instance in incompleteInstances)
                        {
                            Console.WriteLine($"  {instance}");
                        }
                        Console.WriteLine("请先完成所有设置");
                        goto CalculateMain;
                    }

                    // 显示所有计算实例的配置
                    Console.WriteLine("=== 所有计算实例配置 ===");
                    for (int i = 0; i < calculators.Count; i++)
                    {
                        var calc = calculators[i];
                        Console.WriteLine($"实例 #{i + 1}:");
                        Console.WriteLine($"  角色1: {calc.character1.Name}");
                        Console.WriteLine($"  角色2: {calc.character2.Name}");
                        Console.WriteLine($"  道具1: {calc.item1.Name}");
                        Console.WriteLine($"  道具2: {calc.item2.Name}");
                        Console.WriteLine($"  公式1: {calc.Formula1.Name}");
                        Console.WriteLine($"  公式2: {calc.Formula2.Name}");
                        Console.WriteLine($"  公式3: {calc.Formula3.Name}");
                    }

                    Console.WriteLine("是否开始计算？ (Y/N)");
                    string startConfirm = Console.ReadLine()?.ToLower();

                    if (startConfirm != "y")
                    {
                        Console.WriteLine("已取消计算");
                        goto CalculateMain;
                    }

                    // 执行所有计算
                    Console.WriteLine("=== 开始计算 ===");
                    for (int i = 0; i < calculators.Count; i++)
                    {
                        try
                        {
                            double result = calculators[i].all_Calculate();
                            Console.WriteLine($"实例 #{i + 1} 计算结果: {result}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"实例 #{i + 1} 计算失败: {ex.Message}");
                        }
                    }
                    Console.WriteLine("=== 所有计算完成 ===");

                    // 计算完成后返回Start
                    goto Start;
                }

                else
                {
                    Console.WriteLine("错误：未知的操作");
                    goto CalculateMain;
                }
            }
            //----------------end calculate块----------------
        }
    }
}