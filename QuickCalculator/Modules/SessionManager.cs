using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickCalculator.Modules
{
    /// <summary>
    /// 会话管理器，管理应用程序的会话栈和导航。
    /// 使用栈结构来跟踪当前会话，支持会话的推入和弹出操作。
    /// </summary>
    public class SessionManager
    {
        private bool _isRunning; // 标记会话管理器是否在运行
        private Stack<BaseSession> sessionStack;
        public SessionManager()
        {
            sessionStack = new Stack<BaseSession>();
        }

        /// <summary>
        /// 推入一个新的会话到栈顶
        /// </summary>
        /// <param name="session"></param>
        public void PushSession(BaseSession session)
        {
            sessionStack.Push(session);
        }

        /// <summary>
        /// 弹出当前会话并返回上一个会话
        /// </summary>
        /// <returns></returns>
        public BaseSession? PopSession()
        {
            if (sessionStack.Count > 0)
            {
                sessionStack.Pop();
            }
            return sessionStack.Count > 0 ? sessionStack.Peek() : null;
        }

        /// <summary>
        /// 获取当前会话
        /// </summary>
        /// <returns></returns>
        public BaseSession? CurrentSession()
        {
            return sessionStack.Count > 0 ? sessionStack.Peek() : null;
        }

        //主循环，处理会话的执行和导航
        public async Task RunAsync()
        {
            // 验证会话栈状态
            if (sessionStack == null)
                throw new InvalidOperationException("会话栈未正确初始化");
            // 将主会话推入栈顶
            PushSession(new MainSession());
            _isRunning = true;

            // 主循环
            while (_isRunning && sessionStack.Count > 0)
            {
                try
                {
                    // 获取当前会话，但是不弹出
                    var currentSession = sessionStack.Peek();
                    // 执行当前会话逻辑，并获取下一个要执行的会话
                    var nextSession = await ExecuteCurrentSessionAsync(currentSession);

                    //处理会话逻辑导航
                    await HandleSessionNavigationAsync(currentSession, nextSession);
                }
                catch (Exception ex)
                {
                    await HandleSessionExceptionAsync(ex);
                }
            }
        }

        /// <summary>
        /// 执行当前会话并提供错误处理
        /// </summary>
        /// <param name="currentSession"></param>
        /// <returns></returns>
        private async Task<BaseSession?> ExecuteCurrentSessionAsync(BaseSession currentSession)
        {
            try
            {
                return await currentSession.ExecuteAsync();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("会话已取消，返回上一个会话...");
                return null; // 返回null以返回上一个会话
            }
            catch (Exception ex)
            {
                Console.WriteLine($"会话执行时发生错误：{ex.Message}");
                Console.WriteLine("已退出会话，按任意键继续...");
                Console.ReadKey();
                return null; // 出错时返回null以返回上一个会话
            }
        }

        private async Task HandleSessionNavigationAsync(BaseSession currentSession, BaseSession? nextSession)
        {
            if (nextSession == null)
            {
                // 如果下一个会话为null，弹出当前会话
                PopSession();
                if (sessionStack.Count == 0)
                {
                    // 如果栈为空，停止运行
                    _isRunning = false;
                }
            }
            /**
             * else if (nextSession is ExitSession)
             * {
             *     _isRunning = false; // 如果下一个会话是退出会话，停止运行
             * }
             */
            else if (nextSession != currentSession)
            {
                // 如果下一个会话不同于当前会话，推入新会话
                PushSession(nextSession);
            }
            // 否则保持在当前会话
            await Task.CompletedTask;
        }

        private async Task HandleSessionExceptionAsync(Exception ex)
        {
            Console.WriteLine($"会话管理器发生错误：{ex.Message}");
            Console.WriteLine("按任意键继续...");
            Console.ReadKey();
            await Task.CompletedTask;
        }
    }
}
