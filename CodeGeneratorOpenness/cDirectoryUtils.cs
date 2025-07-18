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
    }
}