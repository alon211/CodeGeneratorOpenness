///
/// 日志记录类 - 用于记录程序运行信息和错误
/// 
/// by Mark König @ 02/2020
/// 修改：添加日志功能
///

using System;
using System.IO;
using System.Windows.Forms;

namespace CodeGeneratorOpenness
{
    public static class Logger
    {
        private static string logFilePath;
        private static readonly object lockObject = new object();

        static Logger()
        {
            // 在应用程序目录下创建Logs文件夹
            string logDirectory = Path.Combine(Application.StartupPath, "Logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // 使用当前日期作为日志文件名
            string fileName = $"CodeGenerator_{DateTime.Now:yyyy-MM-dd}.log";
            logFilePath = Path.Combine(logDirectory, fileName);
        }

        /// <summary>
        /// 记录信息日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="source">日志来源</param>
        public static void LogInfo(string message, string source = "")
        {
            WriteLog("INFO", message, source);
        }

        /// <summary>
        /// 记录警告日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="source">日志来源</param>
        public static void LogWarning(string message, string source = "")
        {
            WriteLog("WARNING", message, source);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="source">日志来源</param>
        public static void LogError(string message, string source = "")
        {
            WriteLog("ERROR", message, source);
        }

        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="source">日志来源</param>
        public static void LogException(Exception ex, string source = "")
        {
            string message = $"异常类型: {ex.GetType().Name}\n" +
                           $"异常消息: {ex.Message}\n" +
                           $"堆栈跟踪: {ex.StackTrace}";
            
            if (ex.InnerException != null)
            {
                message += $"\n内部异常: {ex.InnerException.Message}";
            }

            WriteLog("EXCEPTION", message, source);
        }

        /// <summary>
        /// 记录调试日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="source">日志来源</param>
        public static void LogDebug(string message, string source = "")
        {
            WriteLog("DEBUG", message, source);
        }

        /// <summary>
        /// 写入日志到文件
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志消息</param>
        /// <param name="source">日志来源</param>
        private static void WriteLog(string level, string message, string source)
        {
            try
            {
                lock (lockObject)
                {
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    string sourceInfo = string.IsNullOrEmpty(source) ? "" : $" [{source}]";
                    string logEntry = $"[{timestamp}] [{level}]{sourceInfo}: {message}";

                    File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                // 如果日志写入失败，显示消息框（仅在调试模式下）
                #if DEBUG
                MessageBox.Show($"日志写入失败: {ex.Message}", "日志错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                #endif
            }
        }

        /// <summary>
        /// 获取当前日志文件路径
        /// </summary>
        /// <returns>日志文件路径</returns>
        public static string GetLogFilePath()
        {
            return logFilePath;
        }

        /// <summary>
        /// 清理旧的日志文件（保留最近30天的日志）
        /// </summary>
        public static void CleanOldLogs()
        {
            try
            {
                string logDirectory = Path.GetDirectoryName(logFilePath);
                if (Directory.Exists(logDirectory))
                {
                    var files = Directory.GetFiles(logDirectory, "CodeGenerator_*.log");
                    var cutoffDate = DateTime.Now.AddDays(-30);

                    foreach (var file in files)
                    {
                        var fileInfo = new FileInfo(file);
                        if (fileInfo.CreationTime < cutoffDate)
                        {
                            File.Delete(file);
                            LogInfo($"已删除旧日志文件: {Path.GetFileName(file)}", "Logger");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"清理旧日志文件时发生错误: {ex.Message}", "Logger");
            }
        }
    }
}