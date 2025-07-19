///
/// JSON导出器 - 用于将项目树结构导出为JSON格式
/// 
/// 功能：将TreeView中的项目结构转换为JSON对象并保存到指定路径
/// 创建时间：2025-07-19
///

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodeGeneratorOpenness
{
    /// <summary>
    /// JSON导出器静态工具类
    /// 提供TreeView到JSON的转换和导出功能
    /// </summary>
    public static class JsonExporter
    {
        /// <summary>
        /// 将TreeView导出为JSON文件
        /// </summary>
        /// <param name="treeView">要导出的TreeView控件</param>
        /// <param name="outputPath">输出文件路径（相对于应用程序目录）</param>
        /// <param name="fileName">JSON文件名（不含扩展名）</param>
        /// <returns>导出成功返回true，失败返回false</returns>
        public static bool ExportTreeViewToJson(TreeView treeView, string outputPath, string fileName)
        {
            if (treeView == null)
            {
                Logger.LogError("TreeView对象为空", "JsonExporter.ExportTreeViewToJson");
                return false;
            }

            if (string.IsNullOrWhiteSpace(outputPath) || string.IsNullOrWhiteSpace(fileName))
            {
                Logger.LogError("输出路径或文件名不能为空", "JsonExporter.ExportTreeViewToJson");
                return false;
            }

            try
            {
                Logger.LogInfo($"开始导出TreeView到JSON: {fileName}", "JsonExporter.ExportTreeViewToJson");

                // 确保输出目录存在
                if (!DirectoryUtils.EnsureDirectoryExists(outputPath))
                {
                    Logger.LogError($"无法创建输出目录: {outputPath}", "JsonExporter.ExportTreeViewToJson");
                    return false;
                }

                // 转换TreeView为JSON对象
                JObject jsonObject = ConvertTreeViewToJson(treeView);

                // 构建完整文件路径
                string fullPath = DirectoryUtils.GetFullPath(outputPath);
                string jsonFileName = fileName.EndsWith(".json") ? fileName : $"{fileName}.json";
                string filePath = Path.Combine(fullPath, jsonFileName);

                // 保存JSON到文件
                return SaveJsonToFile(jsonObject, filePath);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "JsonExporter.ExportTreeViewToJson");
                return false;
            }
        }

        /// <summary>
        /// 将自定义JSON对象保存到指定路径
        /// </summary>
        /// <param name="jsonObject">要保存的JSON对象</param>
        /// <param name="outputPath">输出文件路径（相对于应用程序目录）</param>
        /// <param name="fileName">JSON文件名（不含扩展名）</param>
        /// <returns>保存成功返回true，失败返回false</returns>
        public static bool SaveJsonObject(object jsonObject, string outputPath, string fileName)
        {
            if (jsonObject == null)
            {
                Logger.LogError("JSON对象为空", "JsonExporter.SaveJsonObject");
                return false;
            }

            if (string.IsNullOrWhiteSpace(outputPath) || string.IsNullOrWhiteSpace(fileName))
            {
                Logger.LogError("输出路径或文件名不能为空", "JsonExporter.SaveJsonObject");
                return false;
            }

            try
            {
                Logger.LogInfo($"开始保存JSON对象到文件: {fileName}", "JsonExporter.SaveJsonObject");

                // 确保输出目录存在
                if (!DirectoryUtils.EnsureDirectoryExists(outputPath))
                {
                    Logger.LogError($"无法创建输出目录: {outputPath}", "JsonExporter.SaveJsonObject");
                    return false;
                }

                // 构建完整文件路径
                string fullPath = DirectoryUtils.GetFullPath(outputPath);
                string jsonFileName = fileName.EndsWith(".json") ? fileName : $"{fileName}.json";
                string filePath = Path.Combine(fullPath, jsonFileName);

                // 将对象转换为JSON字符串
                string jsonString = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);

                // 保存到文件
                File.WriteAllText(filePath, jsonString, Encoding.UTF8);
                
                Logger.LogInfo($"JSON对象保存成功: {filePath}", "JsonExporter.SaveJsonObject");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "JsonExporter.SaveJsonObject");
                return false;
            }
        }

        /// <summary>
        /// 将TreeView转换为JSON对象
        /// </summary>
        /// <param name="treeView">TreeView控件</param>
        /// <returns>JSON对象</returns>
        private static JObject ConvertTreeViewToJson(TreeView treeView)
        {
            try
            {
                JObject result = new JObject();
                result["exportInfo"] = CreateExportInfo();
                result["treeStructure"] = new JArray();

                JArray nodesArray = (JArray)result["treeStructure"];

                // 遍历根节点
                foreach (TreeNode rootNode in treeView.Nodes)
                {
                    JObject nodeObject = ConvertTreeNodeToJson(rootNode);
                    nodesArray.Add(nodeObject);
                }

                Logger.LogInfo($"TreeView转换完成，共{treeView.GetNodeCount(true)}个节点", "JsonExporter.ConvertTreeViewToJson");
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "JsonExporter.ConvertTreeViewToJson");
                throw;
            }
        }

        /// <summary>
        /// 递归将TreeNode转换为JSON对象
        /// </summary>
        /// <param name="node">TreeNode节点</param>
        /// <returns>JSON对象</returns>
        private static JObject ConvertTreeNodeToJson(TreeNode node)
        {
            JObject nodeObject = new JObject();
            
            // 基本属性
            nodeObject["name"] = node.Text;
            nodeObject["imageIndex"] = node.ImageIndex;
            nodeObject["selectedImageIndex"] = node.SelectedImageIndex;
            nodeObject["isExpanded"] = node.IsExpanded;
            nodeObject["nodeCount"] = node.Nodes.Count;
            
            // Tag对象信息
            if (node.Tag != null)
            {
                nodeObject["tagInfo"] = ExtractTagInfo(node.Tag);
            }

            // 子节点
            if (node.Nodes.Count > 0)
            {
                JArray childrenArray = new JArray();
                foreach (TreeNode childNode in node.Nodes)
                {
                    JObject childObject = ConvertTreeNodeToJson(childNode);
                    childrenArray.Add(childObject);
                }
                nodeObject["children"] = childrenArray;
            }

            return nodeObject;
        }

        /// <summary>
        /// 提取Tag对象的信息
        /// </summary>
        /// <param name="tag">Tag对象</param>
        /// <returns>Tag信息的JSON对象</returns>
        private static JObject ExtractTagInfo(object tag)
        {
            JObject tagInfo = new JObject();
            
            try
            {
                tagInfo["type"] = tag.GetType().Name;
                tagInfo["fullTypeName"] = tag.GetType().FullName;
                
                // 根据不同类型提取特定信息
                switch (tag.GetType().Name)
                {
                    case "PlcSoftware":
                        var software = tag as Siemens.Engineering.SW.PlcSoftware;
                        if (software != null)
                        {
                            tagInfo["name"] = software.Name;
                            tagInfo["description"] = "PLC Software Object";
                        }
                        break;
                        
                    case "PlcBlock":
                        // 处理PLC块信息
                        tagInfo["blockInfo"] = "PLC Block Object";
                        break;
                        
                    case "PlcType":
                        // 处理PLC类型信息
                        tagInfo["typeInfo"] = "PLC Type Object";
                        break;
                        
                    case "PlcTagTable":
                        // 处理标签表信息
                        tagInfo["tagTableInfo"] = "PLC Tag Table Object";
                        break;
                        
                    default:
                        tagInfo["description"] = "Unknown Object Type";
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"提取Tag信息时发生错误: {ex.Message}", "JsonExporter.ExtractTagInfo");
                tagInfo["error"] = ex.Message;
            }
            
            return tagInfo;
        }

        /// <summary>
        /// 创建导出信息
        /// </summary>
        /// <returns>导出信息的JSON对象</returns>
        private static JObject CreateExportInfo()
        {
            JObject exportInfo = new JObject();
            exportInfo["exportTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            exportInfo["exportVersion"] = "1.0";
            exportInfo["applicationName"] = "CodeGeneratorOpenness";
            exportInfo["tiaPortalVersion"] = Program.Version ?? "Unknown";
            exportInfo["tiaPortalApi"] = Program.Api ?? "Unknown";
            
            return exportInfo;
        }

        /// <summary>
        /// 保存JSON对象到文件
        /// </summary>
        /// <param name="jsonObject">JSON对象</param>
        /// <param name="filePath">完整文件路径</param>
        /// <returns>保存成功返回true</returns>
        private static bool SaveJsonToFile(JObject jsonObject, string filePath)
        {
            try
            {
                // 格式化JSON字符串
                string jsonString = jsonObject.ToString(Formatting.Indented);
                
                // 写入文件
                File.WriteAllText(filePath, jsonString, Encoding.UTF8);
                
                Logger.LogInfo($"JSON文件保存成功: {filePath}", "JsonExporter.SaveJsonToFile");
                Logger.LogInfo($"文件大小: {new FileInfo(filePath).Length} 字节", "JsonExporter.SaveJsonToFile");
                
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "JsonExporter.SaveJsonToFile");
                return false;
            }
        }

        /// <summary>
        /// 获取JSON文件的统计信息
        /// </summary>
        /// <param name="filePath">JSON文件路径</param>
        /// <returns>统计信息字符串</returns>
        public static string GetJsonFileStats(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return "文件不存在";
                }

                FileInfo fileInfo = new FileInfo(filePath);
                string content = File.ReadAllText(filePath);
                JObject jsonObject = JObject.Parse(content);
                
                StringBuilder stats = new StringBuilder();
                stats.AppendLine($"文件路径: {filePath}");
                stats.AppendLine($"文件大小: {fileInfo.Length} 字节");
                stats.AppendLine($"创建时间: {fileInfo.CreationTime}");
                stats.AppendLine($"修改时间: {fileInfo.LastWriteTime}");
                
                if (jsonObject["exportInfo"] != null)
                {
                    stats.AppendLine($"导出时间: {jsonObject["exportInfo"]["exportTime"]}");
                    stats.AppendLine($"TIA Portal版本: {jsonObject["exportInfo"]["tiaPortalVersion"]}");
                }
                
                return stats.ToString();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "JsonExporter.GetJsonFileStats");
                return $"获取文件统计信息失败: {ex.Message}";
            }
        }
    }
}