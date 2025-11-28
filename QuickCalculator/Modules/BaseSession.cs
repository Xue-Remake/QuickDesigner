using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickCalculator.Modules
{
    /// <summary>
    /// 会话基类，所有具体会话类型的基类。
    /// 定义了会话的基本结构和行为
    /// </summary>
    public abstract class BaseSession
    {
        public abstract string SessionName { get; } // 会话名称
        public abstract Task<BaseSession> ExecuteAsync(); // 抽象方法，执行会话逻辑

        public void DisPlayHeader() // 显示会话头部信息
        {
            Console.WriteLine("=== " + SessionName + " ===");
        }

        protected string ReadInput(string prompt) // 读取用户输入
        {
            Console.Write(prompt + ": ");
            return Console.ReadLine() ?? string.Empty;
        }

        protected void WaitForUser() // 等待用户按下回车键
        {
            Console.WriteLine("按下回车键继续...");
            Console.ReadLine();
        }

        protected void WaitForConfirmation(string message = "按任意键继续...")
        {
            Console.WriteLine(message);
            Console.ReadKey();
        }
    }
}
