using System;
using System.Threading.Tasks;
using QuickCalculator.Modules;

namespace QuickCalculator
{
    public static class CalculateCount
    {
        // 使用静态变量跟踪计算次数
        public static int calculateCount = 1;
    }

    /// <summary>
    /// 主程序类，应用程序的入口点。
    /// 负责初始化和运行不同的会话模块。
    /// </summary>
    class Program
    {

        static async Task Main(string[] args)
        {
            //显示欢迎和作者信息
            Console.WriteLine("欢迎使用 QuickCalculator v1.0");
            //作者信息移动到帮助会话中

            try
            {
                //创建会话管理器实例
                var sessionManager = new SessionManager();
                //启动会话管理器主循环
                await sessionManager.RunAsync();
                //程序正常退出
                Console.WriteLine("感谢使用 QuickCalculator，程序已退出。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"程序发生未预期的错误：{ex.Message}");
                Console.WriteLine("按任意键退出");
                Console.ReadKey();
            }
        }
    }
}