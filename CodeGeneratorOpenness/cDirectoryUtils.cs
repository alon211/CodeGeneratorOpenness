///
/// 目录工具类 - 用于目录创建和路径处理
/// 
/// 创建日期：2024年12月19日
/// 功能：提供模块化的目录创建和路径处理功能
///

using System;
using System.IO;
using System.Windows.Forms;

namespace CodeGeneratorOpenness
{
    /// <summary>
    /// 目录工具类，提供目录创建和路径处理的静态方法
    /// </summary>
    public static class DirectoryUtils
    {
        /// <summary>
        /// 确保指定的相对路径存在，如果不存在则创建
        /// 支持多级目录创建，如 "project\config" 会创建 project 和 config 两级目录
        /// </summary>
        /// <param name="relativePath">相对于当前应用程序目录的路径</param>
        /// <returns>创建成功返回true，失败返回false</returns>
        public static bool EnsureDirectoryExists(string relativePath)
        {
            try
            {
                // 验证输入参数
                if (string.IsNullOrWhiteSpace(relativePath))
                {
                    Logger.LogWarning("相对路径不能为空", "DirectoryUtils.EnsureDirectoryExists");
                    return false;
                }

                // 获取当前应用程序目录
                string currentDirectory = Application.StartupPath;
                
                // 组合完整路径
                string fullPath = Path.Combine(currentDirectory, relativePath);
                
                // 标准化路径（处理 \\ 和 / 混用的情况）
                fullPath = Path.GetFullPath(fullPath);
                
                Logger.LogInfo($"检查目录路径: {fullPath}", "DirectoryUtils.EnsureDirectoryExists");
                
                // 检查目录是否已存在
                if (Directory.Exists(fullPath))
                {
                    Logger.LogInfo($"目录已存在: {fullPath}", "DirectoryUtils.EnsureDirectoryExists");
                    return true;
                }
                
                // 创建目录（包括所有必要的父目录）
                Directory.CreateDirectory(fullPath);
                
                Logger.LogInfo($"目录创建成功: {fullPath}", "DirectoryUtils.EnsureDirectoryExists");
                return true;
            }
            catch (ArgumentException ex)
            {
                Logger.LogError($"路径参数无效: {ex.Message}", "DirectoryUtils.EnsureDirectoryExists");
                return false;
            }
            catch (NotSupportedException ex)
            {
                Logger.LogError($"路径格式不支持: {ex.Message}", "DirectoryUtils.EnsureDirectoryExists");
                return false;
            }
            catch (IOException ex)
            {
                Logger.LogError($"IO异常，无法创建目录: {ex.Message}", "DirectoryUtils.EnsureDirectoryExists");
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogError($"权限不足，无法创建目录: {ex.Message}", "DirectoryUtils.EnsureDirectoryExists");
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "DirectoryUtils.EnsureDirectoryExists");
                return false;
            }
        }
        
        /// <summary>
        /// 批量确保多个相对路径存在
        /// </summary>
        /// <param name="relativePaths">相对路径数组</param>
        /// <returns>所有路径都创建成功返回true，否则返回false</returns>
        public static bool EnsureDirectoriesExist(params string[] relativePaths)
        {
            if (relativePaths == null || relativePaths.Length == 0)
            {
                Logger.LogWarning("路径数组不能为空", "DirectoryUtils.EnsureDirectoriesExist");
                return false;
            }
            
            bool allSuccess = true;
            
            foreach (string path in relativePaths)
            {
                if (!EnsureDirectoryExists(path))
                {
                    allSuccess = false;
                }
            }
            
            return allSuccess;
        }
        
        /// <summary>
        /// 获取相对路径的完整绝对路径
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <returns>完整的绝对路径</returns>
        public static string GetFullPath(string relativePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(relativePath))
                {
                    return Application.StartupPath;
                }
                
                string currentDirectory = Application.StartupPath;
                string fullPath = Path.Combine(currentDirectory, relativePath);
                return Path.GetFullPath(fullPath);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "DirectoryUtils.GetFullPath");
                return Application.StartupPath;
            }
        }
        
        /// <summary>
        /// 检查相对路径是否存在
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <returns>存在返回true，不存在返回false</returns>
        public static bool DirectoryExists(string relativePath)
        {
            try
            {
                string fullPath = GetFullPath(relativePath);
                return Directory.Exists(fullPath);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "DirectoryUtils.DirectoryExists");
                return false;
            }
        }
        
        /// <summary>
        /// 获取当前应用程序目录
        /// </summary>
        /// <returns>当前应用程序目录路径</returns>
        public static string GetApplicationDirectory()
        {
            return Application.StartupPath;
        }
        
        /// <summary>
        /// 复制文件夹及其所有内容到目标位置
        /// 支持递归复制子文件夹和文件，可选择是否覆盖已存在的文件
        /// </summary>
        /// <param name="sourceDirectory">源文件夹路径（可以是相对路径或绝对路径）</param>
        /// <param name="targetDirectory">目标文件夹路径（可以是相对路径或绝对路径）</param>
        /// <param name="overwriteFiles">是否覆盖已存在的文件，默认为true</param>
        /// <param name="copySubDirectories">是否复制子文件夹，默认为true</param>
        /// <returns>复制成功返回true，失败返回false</returns>
        public static bool CopyDirectory(string sourceDirectory, string targetDirectory, bool overwriteFiles = true, bool copySubDirectories = true)
        {
            try
            {
                // 验证输入参数
                if (string.IsNullOrWhiteSpace(sourceDirectory))
                {
                    Logger.LogError("源文件夹路径不能为空", "DirectoryUtils.CopyDirectory");
                    return false;
                }
                
                if (string.IsNullOrWhiteSpace(targetDirectory))
                {
                    Logger.LogError("目标文件夹路径不能为空", "DirectoryUtils.CopyDirectory");
                    return false;
                }
                
                // 处理路径（如果是相对路径，转换为绝对路径）
                string sourcePath = Path.IsPathRooted(sourceDirectory) ? sourceDirectory : GetFullPath(sourceDirectory);
                string targetPath = Path.IsPathRooted(targetDirectory) ? targetDirectory : GetFullPath(targetDirectory);
                
                Logger.LogInfo($"开始复制文件夹: {sourcePath} -> {targetPath}", "DirectoryUtils.CopyDirectory");
                
                // 检查源文件夹是否存在
                if (!Directory.Exists(sourcePath))
                {
                    Logger.LogError($"源文件夹不存在: {sourcePath}", "DirectoryUtils.CopyDirectory");
                    return false;
                }
                
                // 检查是否尝试复制到自身或子目录
                if (IsSubDirectoryOf(sourcePath, targetPath))
                {
                    Logger.LogError($"不能将文件夹复制到自身或其子目录: {sourcePath} -> {targetPath}", "DirectoryUtils.CopyDirectory");
                    return false;
                }
                
                // 创建目标文件夹
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                    Logger.LogInfo($"创建目标文件夹: {targetPath}", "DirectoryUtils.CopyDirectory");
                }
                
                // 复制文件
                DirectoryInfo sourceDir = new DirectoryInfo(sourcePath);
                FileInfo[] files = sourceDir.GetFiles();
                int fileCount = 0;
                
                foreach (FileInfo file in files)
                {
                    string targetFilePath = Path.Combine(targetPath, file.Name);
                    
                    try
                    {
                        file.CopyTo(targetFilePath, overwriteFiles);
                        fileCount++;
                        Logger.LogInfo($"复制文件: {file.FullName} -> {targetFilePath}", "DirectoryUtils.CopyDirectory");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"复制文件失败: {file.FullName} -> {targetFilePath}, 错误: {ex.Message}", "DirectoryUtils.CopyDirectory");
                        // 继续复制其他文件，不中断整个过程
                    }
                }
                
                Logger.LogInfo($"复制了 {fileCount} 个文件", "DirectoryUtils.CopyDirectory");
                
                // 递归复制子文件夹
                if (copySubDirectories)
                {
                    DirectoryInfo[] subDirectories = sourceDir.GetDirectories();
                    int dirCount = 0;
                    
                    foreach (DirectoryInfo subDir in subDirectories)
                    {
                        string targetSubDirPath = Path.Combine(targetPath, subDir.Name);
                        
                        if (CopyDirectory(subDir.FullName, targetSubDirPath, overwriteFiles, copySubDirectories))
                        {
                            dirCount++;
                        }
                    }
                    
                    Logger.LogInfo($"复制了 {dirCount} 个子文件夹", "DirectoryUtils.CopyDirectory");
                }
                
                Logger.LogInfo($"文件夹复制完成: {sourcePath} -> {targetPath}", "DirectoryUtils.CopyDirectory");
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogError($"权限不足，无法复制文件夹: {ex.Message}", "DirectoryUtils.CopyDirectory");
                return false;
            }
            catch (DirectoryNotFoundException ex)
            {
                Logger.LogError($"文件夹未找到: {ex.Message}", "DirectoryUtils.CopyDirectory");
                return false;
            }
            catch (IOException ex)
            {
                Logger.LogError($"IO异常，复制文件夹失败: {ex.Message}", "DirectoryUtils.CopyDirectory");
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "DirectoryUtils.CopyDirectory");
                return false;
            }
        }
        
        /// <summary>
        /// 检查目标路径是否是源路径的子目录
        /// 防止将文件夹复制到自身或其子目录中
        /// </summary>
        /// <param name="sourcePath">源路径</param>
        /// <param name="targetPath">目标路径</param>
        /// <returns>如果目标路径是源路径的子目录返回true</returns>
        private static bool IsSubDirectoryOf(string sourcePath, string targetPath)
        {
            try
            {
                DirectoryInfo sourceDir = new DirectoryInfo(sourcePath);
                DirectoryInfo targetDir = new DirectoryInfo(targetPath);
                
                // 标准化路径
                string normalizedSource = sourceDir.FullName.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToLowerInvariant();
                string normalizedTarget = targetDir.FullName.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToLowerInvariant();
                
                // 检查是否相同
                if (normalizedSource == normalizedTarget)
                {
                    return true;
                }
                
                // 检查目标是否是源的子目录
                return normalizedTarget.StartsWith(normalizedSource + Path.DirectorySeparatorChar);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "DirectoryUtils.IsSubDirectoryOf");
                return false;
            }
        }
        
        /// <summary>
        /// 复制文件夹（简化版本，使用默认参数）
        /// </summary>
        /// <param name="sourceDirectory">源文件夹路径</param>
        /// <param name="targetDirectory">目标文件夹路径</param>
        /// <returns>复制成功返回true，失败返回false</returns>
        public static bool CopyDirectorySimple(string sourceDirectory, string targetDirectory)
        {
            return CopyDirectory(sourceDirectory, targetDirectory, true, true);
        }
        
        /// <summary>
        /// 复制文件夹（仅复制文件，不复制子文件夹）
        /// </summary>
        /// <param name="sourceDirectory">源文件夹路径</param>
        /// <param name="targetDirectory">目标文件夹路径</param>
        /// <param name="overwriteFiles">是否覆盖已存在的文件</param>
        /// <returns>复制成功返回true，失败返回false</returns>
        public static bool CopyDirectoryFilesOnly(string sourceDirectory, string targetDirectory, bool overwriteFiles = true)
        {
            return CopyDirectory(sourceDirectory, targetDirectory, overwriteFiles, false);
        }
    }
}