using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickCalculator.Models;
using System.Threading.Tasks;

namespace QuickCalculator.Modules
{
    public static class CountOfCalculate
    {
        public static int Count = 1; // 静态变量跟踪计算次数
    }

    /// <summary>
    /// 计算会话类，处理批量计算的核心流程。
    /// 管理多个计算实例的执行和结果汇总。
    /// </summary>
    public class CalculationSession : BaseSession
    {
        //默认路径
        private readonly string DataDirectory = "GameObj_data";
        private readonly string FormulaDirectory = "Formula_data";
        private readonly string CharacterDataPath = "GameObj_data/Default_Character.json"; // 默认角色数据文件路径
        private readonly string ItemDataPath = "GameObj_data/Default_Item.json"; // 默认道具数据文件路径
        private readonly string FormulaDataPath = "Formula_data/Default_Formula.json"; // 默认公式数据文件路径
        //private string FormulaGroupDataPath = "Formula_data/FormulaGroupData.json"; // 公式组功能待实现

        private List<Calculator> _calculators = new List<Calculator>(); // 计算器列表
        public override string SessionName => "计算器-计算模式";

        public override async Task<BaseSession> ExecuteAsync()
        {
            DisPlayHeader();
            if (_calculators.Count == 0)
            {
                await InitializeCalculatorsAsync(); // 初始化计算器列表
            }
            Console.WriteLine($"准备执行 {_calculators.Count} 次计算...");
            DisPlayCalculationMenu();

            string choice = ReadInput("请选择操作");
            switch (choice)
            {
                case "run":
                    await RunCalculationsAsync();
                    return this;
                case "back":
                    return null;
                case "choose":
                    await ConfigureCalculators(); // 配置计算器
                    return this;
                case "view":
                    await ViewTheChoices(); // 查看当前选择
                    return this;
                default:
                    Console.WriteLine("无效输入，请重试。");
                    WaitForUser();
                    return this;
            }
        }

        private void DisPlayCalculationMenu()
        {
            Console.WriteLine("1. run         -执行计算");
            Console.WriteLine("2. choose      -配置计算选项");
            Console.WriteLine("3. view        -查看当前选择");
            Console.WriteLine("4. back        -返回主菜单");
            Console.WriteLine(); // 空行分隔
            Console.WriteLine($"当前配置：{_calculators.Count}个计算实例");
            Console.WriteLine();
        }

        private async Task InitializeCalculatorsAsync()
        {
            // 初始化计算器列表的逻辑
            // 这里可以添加默认的计算器实例，或者从配置文件加载
            int Count = CalculateCount.calculateCount;
            for (int i = 0; i < Count; i++)
            {
                _calculators.Add(new Calculator());
            }
        }

        //配置计算器
        private async Task ConfigureCalculators()
        {
            Console.WriteLine("配置计算器:");
            // 选择要配置的计算器实例
            Console.WriteLine($"当前有 {_calculators.Count} 个计算实例。");
        setCalculateNode: //设置计算器实例标记
            string input = ReadInput($"请输入要配置的计算实例编号 (1-{_calculators.Count})，输入\"back\"回到上级");
            if (input.ToLower() == "back")
            {
                return;
            }
            if (!int.TryParse(input, out int r)) 
            {
                Console.WriteLine("无效输入，返回到选择操作");
                WaitForConfirmation();
                return;
            }
            int index = int.Parse(input) - 1;
            CountOfCalculate.Count = index + 1; // 更新静态计数器
            if (index < 0 || index >= _calculators.Count)
            {
                Console.WriteLine("无效的实例编号。");
                WaitForConfirmation();
                return;
            }
            // 配置选定的计算器实例
            // 一个计算器实例必须有：角色1，角色2，道具1，道具2，角色-道具公式1，角色-道具公式2，两个临时角色交互公式(公式3)
            Console.WriteLine($"配置实例 {index + 1}:");
            while (true)
            {
                Console.WriteLine("目前的实例配置：");
                Console.WriteLine("1. 角色1：" + _calculators[index].character1.Name);
                Console.WriteLine("2. 角色2：" + _calculators[index].character2.Name);
                Console.WriteLine("3. 道具1：" + _calculators[index].item1.Name);
                Console.WriteLine("4. 道具2：" + _calculators[index].item2.Name);
                Console.WriteLine("5. 角色1-道具1交互公式：" + _calculators[index].Formula1.Name);
                Console.WriteLine("6. 角色2-道具2交互公式：" + _calculators[index].Formula2.Name);
                Console.WriteLine("7. 角色交互公式：" + _calculators[index].Formula3.Name);
                Console.WriteLine("8. back: 返回选择配置实例");

                string choice = ReadInput("请选择要配置的选项编号");
                if (choice == "back" || choice == "8")
                {
                    goto setCalculateNode;
                }

                //switch语句处理不同的配置选项，分解为各个方法
                switch (choice)
                { 
                    case "1":
                        SetCharacter1();
                        break;
                    case "2":
                        SetCharacter2();
                        break;
                    case "3":
                        SetItem1();
                        break;
                    case "4":
                        SetItem2();
                        break;
                    case "5":
                        SetFormula1();
                        break;
                    case "6":
                        SetFormula2();
                        break;
                    case "7":
                        SetFormula3();
                        break;
                    default:
                        Console.WriteLine("无效输入，请重试。");
                        break;
                }
            }

            Console.WriteLine("配置完成。");
            WaitForConfirmation();
        }

        private async Task RunCalculationsAsync()
        {
            Console.WriteLine("开始执行计算...");
            for (int i = 0; i < _calculators.Count; i++) //使用for确保按顺序执行
            {
                try
                {
                    //执行单个计算器实例的计算
                    double result = _calculators[i].all_Calculate();
                    Console.WriteLine($"实例{i + 1}的结果：{result:F4}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"实例{i + 1}计算出错：{ex.Message}");
                }
                Console.WriteLine("所有计算已完成。");
                WaitForConfirmation();
            }
        }

        private async Task ViewTheChoices()
        {
            Console.WriteLine("当前计算器配置：");

            // 显示每个计算器实例的配置
            for (int j = 0; j < _calculators.Count; j++)
            {
                Console.WriteLine($"========== 实例 {j + 1} 配置 ==========");
                Console.WriteLine("  角色1: " + _calculators[j].character1.Name);
                Console.WriteLine("  角色2: " + _calculators[j].character2.Name);
                Console.WriteLine("  道具1: " + _calculators[j].item1.Name);
                Console.WriteLine("  道具2: " + _calculators[j].item2.Name);
                Console.WriteLine("  角色1-道具1交互公式: " + _calculators[j].Formula1.Name);
                Console.WriteLine("  角色2-道具2交互公式: " + _calculators[j].Formula2.Name);
                Console.WriteLine("  角色交互公式: " + _calculators[j].Formula3.Name);
            }
            WaitForConfirmation();
        }


        private void SetCharacter1()
        {
            int useCount  = CountOfCalculate.Count; // 获取当前计算实例编号
            // 先查找文件(类似CheckSession中的view方法逻辑)，然后用户选择读取哪一个文件
            // 列举该文件中可用的角色，用户指定角色名称后，读取该角色数据并赋值给计算器实例的character1属性
            List<string> allfiles = FileManager.FindJsonFilesInPath(DataDirectory, "Character");
            Console.WriteLine("找到以下角色数据文件：");
            for (int i = 0; i < allfiles.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {allfiles[i]}");
            }

            //选择读取的文件
            string input = ReadInput("请输入要查看的文件编号");
            if (int.TryParse(input, out int fileIndex) && fileIndex >= 1 && fileIndex <= allfiles.Count)
            {
                string selectedFile = allfiles[fileIndex - 1];
                List<Character> characters = DataManager.LoadCharacters(allfiles[fileIndex - 1]);
                Console.WriteLine("文件中包含以下角色：");
                for (int i = 0; i < characters.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {characters[i].Name}");
                }
                string charInput = ReadInput("请输入要选择的角色编号");
                if (int.TryParse(charInput, out int charIndex) && charIndex >= 1 && charIndex <= characters.Count)
                {
                    _calculators[useCount-1].character1 = characters[charIndex - 1];
                    Console.WriteLine($"已选择角色1: {_calculators[useCount-1].character1.Name}");
                }
                else
                {
                    Console.WriteLine("无效的角色编号。");
                }
            }
            else
            {
                Console.WriteLine("无效的文件编号。");
            }
        }

        private void SetCharacter2()
        {
            int useCount = CountOfCalculate.Count; // 获取当前计算实例编号
            // 类似SetCharacter1方法
            List<string> allfiles = FileManager.FindJsonFilesInPath(DataDirectory, "Character");
            Console.WriteLine("找到以下角色数据文件：");
            for (int i = 0; i < allfiles.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {allfiles[i]}");
            }

            //选择读取的文件
            string input = ReadInput("请输入要查看的文件编号");
            if (int.TryParse(input, out int fileIndex) && fileIndex >= 1 && fileIndex <= allfiles.Count)
            {
                string selectedFile = allfiles[fileIndex - 1];
                List<Character> characters = DataManager.LoadCharacters(allfiles[fileIndex - 1]);
                Console.WriteLine("文件中包含以下角色：");
                for (int i = 0; i < characters.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {characters[i].Name}");
                }
                string charInput = ReadInput("请输入要选择的角色编号");
                if (int.TryParse(charInput, out int charIndex) && charIndex >= 1 && charIndex <= characters.Count)
                {
                    _calculators[useCount-1].character2 = characters[charIndex - 1];
                    Console.WriteLine($"已选择角色2: {_calculators[useCount-1].character2.Name}");
                }
                else
                {
                    Console.WriteLine("无效的角色编号。");
                }
            }
            else
            {
                Console.WriteLine("无效的文件编号。");
            }
        }

        private void SetItem1()
        {
            int useCount = CountOfCalculate.Count; // 获取当前计算实例编号
            // 类似SetCharacter1方法
            List<string> allfiles = FileManager.FindJsonFilesInPath(DataDirectory, "Item");
            Console.WriteLine("找到以下道具数据文件：");
            for (int i = 0; i < allfiles.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {allfiles[i]}");
            }
            //选择读取的文件
            string input = ReadInput("请输入要查看的文件编号");
            if (int.TryParse(input, out int fileIndex) && fileIndex >= 1 && fileIndex <= allfiles.Count)
            {
                string selectedFile = allfiles[fileIndex - 1];
                List<Item> items = DataManager.LoadItems(allfiles[fileIndex - 1]);
                Console.WriteLine("文件中包含以下道具：");
                for (int i = 0; i < items.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {items[i].Name}");
                }
                string itemInput = ReadInput("请输入要选择的道具编号");
                if (int.TryParse(itemInput, out int itemIndex) && itemIndex >= 1 && itemIndex <= items.Count)
                {
                    _calculators[useCount - 1].item1 = items[itemIndex - 1];
                    Console.WriteLine($"已选择道具1: {_calculators[useCount - 1].item1.Name}");
                }
                else
                {
                    Console.WriteLine("无效的道具编号。");
                }
            }
            else
            {
                Console.WriteLine("无效的文件编号。");
            }
        }

        private void SetItem2()
        {
            int useCount = CountOfCalculate.Count; // 获取当前计算实例编号
            // 类似SetItem1方法
            List<string> allfiles = FileManager.FindJsonFilesInPath(DataDirectory, "Item");
            Console.WriteLine("找到以下道具数据文件：");
            for (int i = 0; i < allfiles.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {allfiles[i]}");
            }
            //选择读取的文件
            string input = ReadInput("请输入要查看的文件编号");
            if (int.TryParse(input, out int fileIndex) && fileIndex >= 1 && fileIndex <= allfiles.Count)
            {
                string selectedFile = allfiles[fileIndex - 1];
                List<Item> items = DataManager.LoadItems(allfiles[fileIndex - 1]);
                Console.WriteLine("文件中包含以下道具：");
                for (int i = 0; i < items.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {items[i].Name}");
                }
                string itemInput = ReadInput("请输入要选择的道具编号");
                if (int.TryParse(itemInput, out int itemIndex) && itemIndex >= 1 && itemIndex <= items.Count)
                {
                    _calculators[useCount - 1].item2 = items[itemIndex - 1];
                    Console.WriteLine($"已选择道具2: {_calculators[useCount - 1].item2.Name}");
                }
                else
                {
                    Console.WriteLine("无效的道具编号。");
                }
            }
            else
            {
                Console.WriteLine("无效的文件编号。");
            }
        }

        private void SetFormula1()
        {
            int useCount = CountOfCalculate.Count; // 获取当前计算实例编号
            // 类似SetCharacter1方法
            List<string> allfiles = FileManager.FindJsonFilesInPath(FormulaDirectory, "Formula");
            Console.WriteLine("找到以下公式数据文件：");
            for (int i = 0; i < allfiles.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {allfiles[i]}");
            }
            //选择读取的文件
            string input = ReadInput("请输入要查看的文件编号");
            if (int.TryParse(input, out int fileIndex) && fileIndex >= 1 && fileIndex <= allfiles.Count)
            {
                string selectedFile = allfiles[fileIndex - 1];
                List<Formula> formulas = DataManager.LoadFormulas(allfiles[fileIndex - 1]);
                Console.WriteLine("文件中包含以下公式：");
                for (int i = 0; i < formulas.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {formulas[i].Name}");
                }
                string formulaInput = ReadInput("请输入要选择的公式编号");
                if (int.TryParse(formulaInput, out int formulaIndex) && formulaIndex >= 1 && formulaIndex <= formulas.Count)
                {
                    _calculators[useCount - 1].Formula1 = formulas[formulaIndex - 1];
                    Console.WriteLine($"已选择公式1: {_calculators[useCount - 1].Formula1.Name}");
                }
                else
                {
                    Console.WriteLine("无效的公式编号。");
                }
            }
            else
            {
                Console.WriteLine("无效的文件编号。");
            }
        }

        private void SetFormula2()
        {
            int useCount = CountOfCalculate.Count; // 获取当前计算实例编号
            // 类似SetFormula1方法
            List<string> allfiles = FileManager.FindJsonFilesInPath(FormulaDirectory, "Formula");
            Console.WriteLine("找到以下公式数据文件：");
            for (int i = 0; i < allfiles.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {allfiles[i]}");
            }
            //选择读取的文件
            string input = ReadInput("请输入要查看的文件编号");
            if (int.TryParse(input, out int fileIndex) && fileIndex >= 1 && fileIndex <= allfiles.Count)
            {
                string selectedFile = allfiles[fileIndex - 1];
                List<Formula> formulas = DataManager.LoadFormulas(allfiles[fileIndex - 1]);
                Console.WriteLine("文件中包含以下公式：");
                for (int i = 0; i < formulas.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {formulas[i].Name}");
                }
                string formulaInput = ReadInput("请输入要选择的公式编号");
                if (int.TryParse(formulaInput, out int formulaIndex) && formulaIndex >= 1 && formulaIndex <= formulas.Count)
                {
                    _calculators[useCount - 1].Formula2 = formulas[formulaIndex - 1];
                    Console.WriteLine($"已选择公式2: {_calculators[useCount - 1].Formula2.Name}");
                }
                else
                {
                    Console.WriteLine("无效的公式编号。");
                }
            }
            else
            {
                Console.WriteLine("无效的文件编号。");
            }
        }

        private void SetFormula3()
        {
            int useCount = CountOfCalculate.Count; // 获取当前计算实例编号
            // 类似SetFormula1方法
            List<string> allfiles = FileManager.FindJsonFilesInPath(FormulaDirectory, "Formula");
            Console.WriteLine("找到以下公式数据文件：");
            for (int i = 0; i < allfiles.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {allfiles[i]}");
            }
            //选择读取的文件
            string input = ReadInput("请输入要查看的文件编号");
            if (int.TryParse(input, out int fileIndex) && fileIndex >= 1 && fileIndex <= allfiles.Count)
            {
                string selectedFile = allfiles[fileIndex - 1];
                List<Formula> formulas = DataManager.LoadFormulas(allfiles[fileIndex - 1]);
                Console.WriteLine("文件中包含以下公式：");
                for (int i = 0; i < formulas.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {formulas[i].Name}");
                }
                string formulaInput = ReadInput("请输入要选择的公式编号");
                if (int.TryParse(formulaInput, out int formulaIndex) && formulaIndex >= 1 && formulaIndex <= formulas.Count)
                {
                    _calculators[useCount - 1].Formula3 = formulas[formulaIndex - 1];
                    Console.WriteLine($"已选择公式3: {_calculators[useCount - 1].Formula3.Name}");
                }
                else
                {
                    Console.WriteLine("无效的公式编号。");
                }
            }
            else
            {
                Console.WriteLine("无效的文件编号。");
            }
        }
    }
}
