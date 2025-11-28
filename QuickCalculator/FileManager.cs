using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace QuickCalculator
{
    /// <summary>
    /// 文件管理类，负责查询文件(夹)是否存在，以及增删文件(夹)。
    /// 1. 查询文件或文件夹是否存在
    /// 2. 创建文件或文件夹
    /// 3. 删除文件或文件夹
    /// 4. 查找特定类型的JSON文件路径
    /// </summary>
    public class FileManager
    {
        public static bool CheckFileExists(string path)
        {
            if (System.IO.File.Exists(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckDirectoryExists(string path)
        {
            if (Directory.Exists(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void CreateFile(string name, string Path)
        {
            if (!CheckFileExists(Path))
            {
                System.IO.File.Create(Path).Dispose();
            }
            else
            {
                throw new Exception("File already exists.");
            }
        }

        public static void CreateDirectory(string path)
        {
            if (!CheckDirectoryExists(path))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                throw new Exception("Directory already exists.");
            }
        }

        public static void DeleteFile(string path)
        {
            if (CheckFileExists(path))
            {
                System.IO.File.Delete(path);
            }
            else
            {
                throw new Exception("File does not exist.");
            }
        }

        public static void DeleteDirectory(string path)
        {
            if (CheckDirectoryExists(path))
            {
                Directory.Delete(path, true);
            }
            else
            {
                throw new Exception("Directory does not exist.");
            }
        }


        private readonly string[] SupportedTypes = { "Character", "Item", "Formula" };

        public static List<string> FindJsonFilesByType(string projectdirectory, string dataType)
        {
            // 验证数据类型是否受支持
            if (!new FileManager().SupportedTypes.Contains(dataType))
            {
                throw new ArgumentException("Unsupported data type.");
                // 错误提示：不支持的数据类型
            }

            var pattern = $"*{dataType}.json";
            var jsonFiles = new List<string>();
            try
            {
                jsonFiles = Directory.GetFiles(projectdirectory, pattern, SearchOption.AllDirectories).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while searching for files: {ex.Message}");
            }
            return jsonFiles;
        }

        /// <summary>
        /// 在指定目录中查找特定类型的JSON文件路径
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static List<string> FindJsonFilesInPath(string folderPath, string dataType)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException($"目录不存在: {folderPath}");
            }

            // 构建搜索模式
            string searchPattern = $"*_{dataType}.json";

            // 搜索文件
            var files = Directory.GetFiles(folderPath, searchPattern, SearchOption.AllDirectories)
                               .ToList();

            return files;
        }
    }
}
