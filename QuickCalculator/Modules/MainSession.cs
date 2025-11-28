using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickCalculator.Modules
{
    /// <summary>
    /// 主会话类，负责管理和协调应用程序的主要功能和用户交互。
    /// 提供所有功能会话的入口点。
    /// </summary>
    public class MainSession : BaseSession
    {
        public override string SessionName => "计算器-主菜单";

        public override async Task<BaseSession> ExecuteAsync()
        {
            DisPlayHeader();
            DisplayMenu();

            string input = ReadInput("请输入命令");

            return input.ToLower() switch
            {
                "set" => new SetupSession(),
                "add" => new AddDataSession(),
                "check" => new CheckDataSession(),
                "calculate" => new CalculationSession(),
                "?" => new HelpSession(),
                "exit" => null, // 直接返回null以退出应用程序
                _ => this // 无效输入，保持在当前会话
            };
        }
        private void DisplayMenu()
        {
            Console.WriteLine("1. set            -设置计算参数和基础配置");
            Console.WriteLine("2. add            -添加角色、道具和公式数据");
            //在v1.2到v1.4实现创建公式组功能
            Console.WriteLine("3. check          -查看已储存的数据信息");
            //通过JSON文件导入和导出数据
            Console.WriteLine("4. calculate      -进入计算模式");
            Console.WriteLine("5. ?              -显示帮助信息");
            Console.WriteLine("6. exit           -退出应用程序");
            Console.WriteLine(); // 空行分隔
        }

        private BaseSession HandleInvalidInput()
        {
            Console.WriteLine("无效输入，请重试。");
            WaitForConfirmation();
            return this; // 保持在当前会话
        }
    }
}
