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

        public override Task<BaseSession> ExecuteAsync()
        {
            DisPlayHeader();
            Console.WriteLine("帮助信息：");
            Console.WriteLine("输入下列命令查看相关信息：");
            Console.WriteLine("character  - 查看角色类概念");
            Console.WriteLine("item       - 查看道具类概念");
            Console.WriteLine("formula    - 查看公式类概念");
            Console.WriteLine("calculate  - 查看计算类概念");
            Console.WriteLine("use        - 使用说明");
            Console.WriteLine("author     - 作者信息");
            Console.WriteLine("version    - 显示版本信息");
            Console.WriteLine("输入\"back\"返回主菜单");

            // 选择查看的帮助主题
            string input = ReadInput("输入 'author' 查看作者信息，输入 'version' 查看版本信息，或按回车返回主菜单");
            string lowerInput = input.ToLower();
            switch (lowerInput)
            {
                case "author":
                    ShowAuthorInformation();
                    break;
                case "version":
                    ShowVersionInformation();
                    break;
                case "back":
                    // 返回主菜单
                    return null;
                case "character":
                    ShowCharacterConcept();
                    break;
                case "item":
                    ShowItemConcept();
                    break;
                case "calculate":
                    ShowCalculateConcept();
                    break;
                case "use":
                    ShowUsageInstructions();
                    break;
                default:
                    Console.WriteLine("无效输入，返回主菜单。");
                    break;
            }
            WaitForConfirmation();
            return Task.FromResult<BaseSession>(null); // 返回主菜单
        }


        public new void DisPlayHeader() // 显示会话头部信息
        {
            Console.WriteLine("=== " + SessionName + " ===");
        }

        private void ShowAuthorInformation()
        {
            Console.WriteLine("QuickCalculator v1.0");
            Console.WriteLine("作者: Xue-Remake / 雪名残Remake.ver");
            Console.WriteLine("联系方式: xuemincanremake@gmail.com");
            Console.WriteLine("重要声明：本程序主体架构均由作者本人构思，细节上存在ai生成的代码片段，虽然经过修改和调试，暂时没有发现问题");
            Console.WriteLine("但是不排除存在一些严重bug，谨慎使用和学习");
            Console.WriteLine();
            Console.WriteLine("最后，该项目为我第一个较为完整的C#项目，属于学习性质，水平有限，欢迎指点和交流");
        }

        private void ShowVersionInformation()
        {
            Console.WriteLine("QuickCalculator 版本 v1.2");
            //显示更新信息，从v1.0.0beta到v1.2的更新内容，主要重构了项目，分层管理会话，以及新增了程序内分类新建文件储存数据的功能
            //新增了追踪不同数据类型文件路径的功能
            Console.WriteLine("更新内容：");
            Console.WriteLine("1. 重构项目结构，分层管理会话模块");
            Console.WriteLine("2. 新增程序内分类新建文件储存数据的功能");
            Console.WriteLine("3. 追踪不同数据类型文件路径的功能");
        }

        private void ShowCharacterConcept()
        {
            Console.WriteLine("角色类概念：");
            Console.WriteLine("角色由一个字符串Name和多个属性构成");
            Console.WriteLine("属性在添加会话和设置会话中增添和设置");
            Console.WriteLine("属性的名称需要是英文且不能重复");
            Console.WriteLine("计算中使用属性名称匹配的形式，将属性值代入公式进行计算");
        }

        private void ShowItemConcept()
        {
            Console.WriteLine("道具类概念：");
            Console.WriteLine("道具由一个字符串Name和多个属性构成");
            Console.WriteLine("属性在添加会话和设置会话中增添和设置");
            Console.WriteLine("属性的名称需要是英文且不能重复");
            Console.WriteLine("计算中使用属性名称匹配的形式，将属性值代入公式进行计算");
        }

        private void ShowFormulaConcept()
        {
            Console.WriteLine("公式类概念：");
            Console.WriteLine("公式由一个字符串Name，一个字符串Target，一个判断值Distinction，和字符串公式表达式组成");
            Console.WriteLine("公式表达式目前只支持严格按照括号和二元运算的形式");
            Console.WriteLine("比如：( health - ( ( 1.8 * attack ) - ( 1.5 * defence ) ) )");
            Console.WriteLine("公式中的变量名称需要和引用的角色或道具的属性名称一致，计算实例会将属性值替换进去运算");
            Console.WriteLine("Target表示公式计算后要更改的属性名称，如果不需要更改属性可以留空");
            Console.WriteLine("Distinction表示公式是否有效");
            Console.WriteLine("在创建公式时系统会自动使用随机数替换表达式中的字符变量判断是否可算");
        }

        private void ShowCalculateConcept()
        {
            Console.WriteLine("计算类概念：");
            Console.WriteLine("计算实例由字符串Name，两个角色，两个道具和三个公式构成");
            Console.WriteLine("计算时将根据所选角色和道具的属性值，代入公式进行计算");
            Console.WriteLine("基本逻辑是，角色1和道具1的属性值代入公式1进行计算，计算后通过公式的目标字符更改角色1的属性创造一个临时角色1");
            Console.WriteLine("角色2，道具2和公式2逻辑同理，创造临时角色2");
            Console.WriteLine("最后将临时角色1和临时角色2的属性值代入公式3进行最终计算，输出结果");
            Console.WriteLine("无论公式3是否有目标字段，都不会调用，只会输出一个计算结果");
        }

        private void ShowUsageInstructions()
        {
            Console.WriteLine("使用说明：在主菜单输入相应命令进入各个功能模块。");
            Console.WriteLine("首先使用Add会话功能添加需要测试的数据");
            Console.WriteLine("也可以直接在JSON文件中添加数据，程序启动时会加载JSON文件中的数据");
            Console.WriteLine("使用Set会话功能移除和更改角色和道具的属性值");
            Console.WriteLine("使用Calculate会话功能进行计算，选择相应的角色,道具和公式");
        }
    }
}