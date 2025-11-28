using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickCalculator.Modules
{
    /// <summary>
    /// 帮助会话类，显示帮助信息。
    /// </summary>
    public class HelpSession : BaseSession
    {
        public override string SessionName => "帮助信息";

        public override async Task<BaseSession> ExecuteAsync()
        {
            Console.WriteLine("帮助信息：");
            Console.WriteLine("set        - 设置计算参数和基础配置");
            Console.WriteLine("add        - 添加角色、道具和公式数据");
            Console.WriteLine("check      - 查看已储存的数据信息");
            Console.WriteLine("calculate  - 进入计算模式");
            Console.WriteLine("?          - 显示帮助信息");
            Console.WriteLine("exit       - 退出应用程序");
            WaitForConfirmation();
            return new MainSession();
        }
    }
}