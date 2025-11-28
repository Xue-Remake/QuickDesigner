using QuickCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickCalculator.Modules
{
    /// <summary>
    /// 查看数据会话类，处理数据查看的相关操作。
    /// DisplayCheckDataMenu()                        显示数据查看菜单，提供角色、道具、公式查看选项
    /// ViewCharacters()                              查看角色数据，从GameObj_data目录读取并显示角色信息
    /// ViewItems()                                   查看道具数据，从GameObj_data目录读取并显示道具信息  
    /// ViewFormulas()                                查看公式数据，从Formula_data目录读取并显示公式信息
    /// </summary>


    public class CheckDataSession : BaseSession
    {

        //默认路径
        private readonly string DataDirectory = "GameObj_data";
        private readonly string FormulaDirectory = "Formula_data";
        private readonly string CharacterDataPath = "GameObj_data/Default_Character.json"; // 默认角色数据文件路径
        private readonly string ItemDataPath = "GameObj_data/Default_Item.json"; // 默认道具数据文件路径
        private readonly string FormulaDataPath = "Formula_data/Default_Formula.json"; // 默认公式数据文件路径
        //private string FormulaGroupDataPath = "Formula_data/FormulaGroupData.json"; // 公式组功能待实现

        //string baseDirectory = AppDomain.CurrentDomain.BaseDirectory; 
        //获取程序集的根目录，目前用不上
        public override string SessionName => "计算器-查看数据模式";

        public override async Task<BaseSession> ExecuteAsync()
        {
            DisPlayHeader();
            DisplayCheckDataMenu();
            string choice = ReadInput("请选择操作");
            
            switch (choice)
            {
                case "chara":
                    await ViewCharacters(); // 查看角色数据
                    return this;
                case "item":
                    await ViewItems(); // 查看道具数据
                    return this;
                case "formu":
                    await ViewFormulas(); // 查看公式数据
                    return this;
                case "back":
                    return null;
                default:
                    Console.WriteLine("无效输入，请重试。");
                    WaitForConfirmation();
                    return this;
            }
        }

        public void DisplayCheckDataMenu()
        {
            Console.WriteLine("请选择要查看的数据类型：");
            Console.WriteLine("1. 查看角色数据（输入'chara'）");
            Console.WriteLine("2. 查看道具数据（输入'item'）");
            Console.WriteLine("3. 查看公式数据（输入'formu'）");
            Console.WriteLine("4. 返回主菜单（输入'back'）");
        }

        public async Task ViewCharacters()
        {
            /**查看文件夹层次
            List<string> folders = new List<string>();
            folders = FileManager.FindJsonFilesByType(baseDirectory,"Character");
            选择文件夹

            目前不支持多层文件夹选择，后续版本考虑添加
            */

            //目前支持：默认从GameObj_data中读取
            List<string> allfiles = FileManager.FindJsonFilesInPath(DataDirectory, "Character");
            Console.WriteLine("找到以下角色数据文件：");
            for (int i = 0; i < allfiles.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {allfiles[i]}");
            }

            //选择读取的文件
            string input = ReadInput("请输入要查看的文件编号");
            if (int.TryParse(input, out int fileIndex) && fileIndex > 0 && fileIndex <= allfiles.Count)
            {
                string selectedFile = allfiles[fileIndex - 1];
                //读取并显示数据
                List<Character> characters =  DataManager.LoadCharacters(selectedFile);
                foreach (var character in characters)
                {
                    Console.WriteLine($"角色名称: {character.Name}");
                    foreach (var attr in character.Attributes)
                    {
                        Console.WriteLine($"属性: {attr.Key} - 值: {attr.Value}");
                    }
                }
            }
            else
            {
                Console.WriteLine("无效的文件编号。");
            }
        }

        public async Task ViewItems()
        {
            /**
             * 目前不支持多层文件夹选择，后续版本考虑添加
             */

            //目前支持：默认从GameObj_data中读取
            List<string> allfiles = FileManager.FindJsonFilesInPath(DataDirectory, "Item");
            Console.WriteLine("找到以下道具数据文件：");
            for (int i = 0; i < allfiles.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {allfiles[i]}");
            }

            //选择读取的文件
            string input = ReadInput("请输入要查看的文件编号");
            if (int.TryParse(input, out int fileIndex) && fileIndex > 0 && fileIndex <= allfiles.Count)
            {
                string selectedFile = allfiles[fileIndex - 1];
                //读取并显示数据
                List<Item> items =  DataManager.LoadItems(selectedFile);
                foreach (var item in items)
                {
                    Console.WriteLine($"道具名称: {item.Name}");
                    foreach (var attr in item.Attributes)
                    {
                        Console.WriteLine($"属性: {attr.Key} - 值: {attr.Value}");
                    }
                }
            }
            else
            {
                Console.WriteLine("无效的文件编号。");
            }
        }

        public async Task ViewFormulas()
        {
            /**
             * 目前不支持多层文件夹选择，后续版本考虑添加
             */

            //目前支持：默认从Formula_data中读取
            List<string> allfiles = FileManager.FindJsonFilesInPath(FormulaDirectory, "Formula");
            Console.WriteLine("找到以下公式数据文件：");
            for (int i = 0; i < allfiles.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {allfiles[i]}");
            }

            //选择读取的文件
            string input = ReadInput("请输入要查看的文件编号");
            if (int.TryParse(input, out int fileIndex) && fileIndex > 0 && fileIndex <= allfiles.Count)
            {
                string selectedFile = allfiles[fileIndex - 1];
                //读取并显示数据
                List<Formula> formmulas =  DataManager.LoadFormulas(selectedFile);
                foreach (var formula in formmulas)
                {
                    Console.WriteLine($"公式名称: {formula.Name}");
                    Console.WriteLine($"公式内容: {formula.Form}");
                    Console.WriteLine($"可算性: {(formula.Distinction ? "可算" : "不可算")}");
                    Console.WriteLine("---------------------------");
                }
            }
            else
            {
                Console.WriteLine("无效的文件编号。");
            }
        }
    }
}
