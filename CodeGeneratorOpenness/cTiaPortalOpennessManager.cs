///
/// TIA Portal Openness API 封装管理类
/// 
/// 功能：将常用的TIA Portal Openness API操作集中管理，提高代码的可维护性和复用性
/// 创建时间：2025-01-27
///

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using Siemens.Engineering;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.Types;
using Siemens.Engineering.SW.Tags;
using Siemens.Engineering.SW.ExternalSources;
using Siemens.Engineering.SW.Alarm.TextLists;
using Siemens.Engineering.SW.WatchAndForceTables;
using Siemens.Engineering.SW.TechnologicalObjects;
using Siemens.Engineering.SW.Units;
using Siemens.Engineering.Library;
using Siemens.Engineering.Library.MasterCopies;
using Siemens.Engineering.Compiler;

namespace CodeGeneratorOpenness
{
    /// <summary>
    /// TIA Portal Openness API 封装管理类
    /// 提供统一的API操作接口，简化TIA Portal对象的访问和操作
    /// </summary>
    public static class TiaPortalOpennessManager
    {
        #region TIA Portal 连接管理
        
        /// <summary>
        /// 创建TIA Portal连接
        /// </summary>
        /// <param name="mode">连接模式</param>
        /// <returns>TIA Portal实例</returns>
        public static TiaPortal CreateTiaPortalConnection(TiaPortalMode mode = TiaPortalMode.WithoutUserInterface)
        {
            try
            {
                Logger.LogInfo($"正在创建TIA Portal连接，模式: {mode}", "TiaPortalOpennessManager.CreateTiaPortalConnection");
                TiaPortal tiaPortal = new TiaPortal(mode);
                Logger.LogInfo("TIA Portal连接创建成功", "TiaPortalOpennessManager.CreateTiaPortalConnection");
                return tiaPortal;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "创建TIA Portal连接");
                throw;
            }
        }
        
        /// <summary>
        /// 安全释放TIA Portal连接
        /// </summary>
        /// <param name="tiaPortal">TIA Portal实例</param>
        public static void DisposeTiaPortalConnection(TiaPortal tiaPortal)
        {
            try
            {
                if (tiaPortal != null)
                {
                    Logger.LogInfo("正在释放TIA Portal连接", "TiaPortalOpennessManager.DisposeTiaPortalConnection");
                    tiaPortal.Dispose();
                    Logger.LogInfo("TIA Portal连接释放成功", "TiaPortalOpennessManager.DisposeTiaPortalConnection");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "释放TIA Portal连接");
            }
        }
        
        #endregion
        
        #region 项目管理
        
        /// <summary>
        /// 打开TIA Portal项目
        /// </summary>
        /// <param name="tiaPortal">TIA Portal实例</param>
        /// <param name="projectPath">项目路径</param>
        /// <returns>项目实例</returns>
        public static Project OpenProject(TiaPortal tiaPortal, string projectPath)
        {
            try
            {
                if (tiaPortal == null)
                    throw new ArgumentNullException(nameof(tiaPortal), "TIA Portal实例不能为空");
                    
                if (string.IsNullOrWhiteSpace(projectPath))
                    throw new ArgumentException("项目路径不能为空", nameof(projectPath));
                    
                if (!File.Exists(projectPath))
                    throw new FileNotFoundException($"项目文件不存在: {projectPath}");
                
                Logger.LogInfo($"正在打开项目: {projectPath}", "TiaPortalOpennessManager.OpenProject");
                Project project = tiaPortal.Projects.Open(new FileInfo(projectPath));
                Logger.LogInfo($"项目打开成功: {project.Name}", "TiaPortalOpennessManager.OpenProject");
                return project;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "打开项目");
                throw;
            }
        }
        
        /// <summary>
        /// 创建新的TIA Portal项目
        /// </summary>
        /// <param name="tiaPortal">TIA Portal实例</param>
        /// <param name="projectPath">项目路径</param>
        /// <param name="projectName">项目名称</param>
        /// <returns>项目实例</returns>
        public static Project CreateProject(TiaPortal tiaPortal, string projectPath, string projectName)
        {
            try
            {
                if (tiaPortal == null)
                    throw new ArgumentNullException(nameof(tiaPortal), "TIA Portal实例不能为空");
                    
                if (string.IsNullOrWhiteSpace(projectPath))
                    throw new ArgumentException("项目路径不能为空", nameof(projectPath));
                    
                if (string.IsNullOrWhiteSpace(projectName))
                    throw new ArgumentException("项目名称不能为空", nameof(projectName));
                
                Logger.LogInfo($"正在创建项目: {projectName} 在路径: {projectPath}", "TiaPortalOpennessManager.CreateProject");
                Project project = tiaPortal.Projects.Create(new DirectoryInfo(projectPath), projectName);
                Logger.LogInfo($"项目创建成功: {project.Name}", "TiaPortalOpennessManager.CreateProject");
                return project;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "创建项目");
                throw;
            }
        }
        
        /// <summary>
        /// 保存项目
        /// </summary>
        /// <param name="project">项目实例</param>
        /// <returns>保存是否成功</returns>
        public static bool SaveProject(Project project)
        {
            try
            {
                if (project == null)
                {
                    Logger.LogError("项目实例为空，无法保存", "TiaPortalOpennessManager.SaveProject");
                    return false;
                }
                
                if (project.IsModified)
                {
                    Logger.LogInfo($"正在保存项目: {project.Name}", "TiaPortalOpennessManager.SaveProject");
                    project.Save();
                    Logger.LogInfo($"项目保存成功: {project.Name}", "TiaPortalOpennessManager.SaveProject");
                    return true;
                }
                else
                {
                    Logger.LogInfo($"项目无修改，无需保存: {project.Name}", "TiaPortalOpennessManager.SaveProject");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "保存项目");
                return false;
            }
        }
        
        /// <summary>
        /// 另存为项目
        /// </summary>
        /// <param name="project">项目实例</param>
        /// <param name="targetPath">目标路径</param>
        /// <returns>保存是否成功</returns>
        public static bool SaveProjectAs(Project project, string targetPath)
        {
            try
            {
                if (project == null)
                {
                    Logger.LogError("项目实例为空，无法另存为", "TiaPortalOpennessManager.SaveProjectAs");
                    return false;
                }
                
                if (string.IsNullOrWhiteSpace(targetPath))
                {
                    Logger.LogError("目标路径不能为空", "TiaPortalOpennessManager.SaveProjectAs");
                    return false;
                }
                
                Logger.LogInfo($"正在另存为项目: {project.Name} 到 {targetPath}", "TiaPortalOpennessManager.SaveProjectAs");
                project.SaveAs(new DirectoryInfo(targetPath));
                Logger.LogInfo($"项目另存为成功: {project.Name}", "TiaPortalOpennessManager.SaveProjectAs");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "另存为项目");
                return false;
            }
        }
        
        #endregion
        
        #region 设备管理
        
        /// <summary>
        /// 获取项目中的所有S7-1500设备
        /// </summary>
        /// <param name="project">项目实例</param>
        /// <returns>S7-1500设备列表</returns>
        public static List<Device> GetS7_1500Devices(Project project)
        {
            List<Device> s7Devices = new List<Device>();
            
            try
            {
                if (project == null)
                {
                    Logger.LogError("项目实例为空", "TiaPortalOpennessManager.GetS7_1500Devices");
                    return s7Devices;
                }
                
                Logger.LogInfo($"正在搜索项目 {project.Name} 中的S7-1500设备", "TiaPortalOpennessManager.GetS7_1500Devices");
                
                foreach (Device device in project.Devices)
                {
                    if (device.TypeIdentifier == "System:Device.S71500")
                    {
                        s7Devices.Add(device);
                        Logger.LogInfo($"发现S7-1500设备: {device.Name}", "TiaPortalOpennessManager.GetS7_1500Devices");
                    }
                }
                
                Logger.LogInfo($"搜索完成，共找到 {s7Devices.Count} 个S7-1500设备", "TiaPortalOpennessManager.GetS7_1500Devices");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "获取S7-1500设备");
            }
            
            return s7Devices;
        }
        
        /// <summary>
        /// 获取设备中的CPU
        /// </summary>
        /// <param name="device">设备实例</param>
        /// <returns>CPU设备项列表</returns>
        public static List<DeviceItem> GetCPUs(Device device)
        {
            List<DeviceItem> cpus = new List<DeviceItem>();
            
            try
            {
                if (device == null)
                {
                    Logger.LogError("设备实例为空", "TiaPortalOpennessManager.GetCPUs");
                    return cpus;
                }
                
                Logger.LogInfo($"正在搜索设备 {device.Name} 中的CPU", "TiaPortalOpennessManager.GetCPUs");
                
                foreach (DeviceItem item in device.DeviceItems)
                {
                    if (item.Classification.ToString() == "CPU")
                    {
                        cpus.Add(item);
                        Logger.LogInfo($"发现CPU: {item.Name}", "TiaPortalOpennessManager.GetCPUs");
                    }
                }
                
                Logger.LogInfo($"搜索完成，共找到 {cpus.Count} 个CPU", "TiaPortalOpennessManager.GetCPUs");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "获取CPU");
            }
            
            return cpus;
        }
        
        /// <summary>
        /// 获取设备项的软件容器
        /// </summary>
        /// <param name="deviceItem">设备项</param>
        /// <returns>PLC软件实例</returns>
        public static PlcSoftware GetPlcSoftware(DeviceItem deviceItem)
        {
            try
            {
                if (deviceItem == null)
                {
                    Logger.LogError("设备项为空", "TiaPortalOpennessManager.GetPlcSoftware");
                    return null;
                }
                
                Logger.LogInfo($"正在获取设备项 {deviceItem.Name} 的软件容器", "TiaPortalOpennessManager.GetPlcSoftware");
                
                SoftwareContainer softwareContainer = ((IEngineeringServiceProvider)deviceItem).GetService<SoftwareContainer>();
                if (softwareContainer != null)
                {
                    PlcSoftware software = softwareContainer.Software as PlcSoftware;
                    if (software != null)
                    {
                        Logger.LogInfo($"成功获取软件容器: {software.Name}", "TiaPortalOpennessManager.GetPlcSoftware");
                        return software;
                    }
                }
                
                Logger.LogWarning($"设备项 {deviceItem.Name} 的软件容器为空", "TiaPortalOpennessManager.GetPlcSoftware");
                return null;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "获取PLC软件");
                return null;
            }
        }
        
        #endregion
        
        #region 服务提供者管理
        
        /// <summary>
        /// 获取PLC软件的PlcUnitProvider服务
        /// </summary>
        /// <param name="plcSoftware">PLC软件实例</param>
        /// <returns>PlcUnitProvider实例</returns>
        public static PlcUnitProvider GetPlcUnitProvider(PlcSoftware plcSoftware)
        {
            try
            {
                if (plcSoftware == null)
                {
                    Logger.LogError("PLC软件实例为空", "TiaPortalOpennessManager.GetPlcUnitProvider");
                    return null;
                }
                
                Logger.LogInfo($"正在获取PLC软件 {plcSoftware.Name} 的PlcUnitProvider服务", "TiaPortalOpennessManager.GetPlcUnitProvider");
                
                PlcUnitProvider unitProvider = plcSoftware.GetService<PlcUnitProvider>();
                if (unitProvider != null)
                {
                    Logger.LogInfo("成功获取PlcUnitProvider服务", "TiaPortalOpennessManager.GetPlcUnitProvider");
                }
                else
                {
                    Logger.LogWarning("PlcUnitProvider服务为空", "TiaPortalOpennessManager.GetPlcUnitProvider");
                }
                
                return unitProvider;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "获取PlcUnitProvider服务");
                return null;
            }
        }
        
        /// <summary>
        /// 获取项目库
        /// </summary>
        /// <param name="project">项目实例</param>
        /// <returns>项目库实例</returns>
        public static ProjectLibrary GetProjectLibrary(Project project)
        {
            try
            {
                if (project == null)
                {
                    Logger.LogError("项目实例为空", "TiaPortalOpennessManager.GetProjectLibrary");
                    return null;
                }
                
                Logger.LogInfo($"正在获取项目 {project.Name} 的项目库", "TiaPortalOpennessManager.GetProjectLibrary");
                
                ProjectLibrary projectLibrary = project.ProjectLibrary;
                
                if (projectLibrary != null)
                {
                    Logger.LogInfo("成功获取项目库", "TiaPortalOpennessManager.GetProjectLibrary");
                }
                else
                {
                    Logger.LogWarning("项目库为空", "TiaPortalOpennessManager.GetProjectLibrary");
                }
                
                return projectLibrary;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "获取项目库");
                return null;
            }
        }
        
        /// <summary>
        /// 获取软件单元组合
        /// </summary>
        /// <param name="unitProvider">单元提供者</param>
        /// <returns>软件单元组合</returns>
        public static PlcUnitComposition GetPlcUnitComposition(PlcUnitProvider unitProvider)
        {
            try
            {
                if (unitProvider == null)
                {
                    Logger.LogError("单元提供者为空", "TiaPortalOpennessManager.GetPlcUnitComposition");
                    return null;
                }
                
                Logger.LogInfo("正在获取软件单元组合", "TiaPortalOpennessManager.GetPlcUnitComposition");
                
                PlcUnitComposition unitComposition = unitProvider.UnitGroup?.Units;
                
                if (unitComposition != null)
                {
                    Logger.LogInfo("成功获取软件单元组合", "TiaPortalOpennessManager.GetPlcUnitComposition");
                }
                else
                {
                    Logger.LogWarning("软件单元组合为空", "TiaPortalOpennessManager.GetPlcUnitComposition");
                }
                
                return unitComposition;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "获取软件单元组合");
                return null;
            }
        }
        
        /// <summary>
        /// 获取软件单元系统组
        /// </summary>
        /// <param name="unitProvider">单元提供者</param>
        /// <returns>软件单元系统组</returns>
        public static PlcUnitSystemGroup GetPlcUnitSystemGroup(PlcUnitProvider unitProvider)
        {
            try
            {
                if (unitProvider == null)
                {
                    Logger.LogError("单元提供者为空", "TiaPortalOpennessManager.GetPlcUnitSystemGroup");
                    return null;
                }
                
                Logger.LogInfo("正在获取软件单元系统组", "TiaPortalOpennessManager.GetPlcUnitSystemGroup");
                
                PlcUnitSystemGroup unitSystemGroup = unitProvider.UnitGroup;
                
                if (unitSystemGroup != null)
                {
                    Logger.LogInfo("成功获取软件单元系统组", "TiaPortalOpennessManager.GetPlcUnitSystemGroup");
                }
                else
                {
                    Logger.LogWarning("软件单元系统组为空", "TiaPortalOpennessManager.GetPlcUnitSystemGroup");
                }
                
                return unitSystemGroup;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "获取软件单元系统组");
                return null;
            }
        }
        
        #endregion
        
        #region 语言设置管理
        
        /// <summary>
        /// 设置项目语言
        /// </summary>
        /// <param name="project">项目实例</param>
        /// <param name="editingLanguage">编辑语言</param>
        /// <param name="referenceLanguage">参考语言</param>
        /// <returns>设置是否成功</returns>
        public static bool SetProjectLanguages(Project project, CultureInfo editingLanguage, CultureInfo referenceLanguage)
        {
            try
            {
                if (project == null)
                {
                    Logger.LogError("项目实例为空", "TiaPortalOpennessManager.SetProjectLanguages");
                    return false;
                }
                
                Logger.LogInfo($"正在设置项目语言: 编辑语言={editingLanguage.Name}, 参考语言={referenceLanguage.Name}", "TiaPortalOpennessManager.SetProjectLanguages");
                
                LanguageSettings languageSettings = project.LanguageSettings;
                LanguageComposition supportedLanguages = languageSettings.Languages;
                LanguageAssociation activeLanguages = languageSettings.ActiveLanguages;
                
                // 查找支持的语言
                Language supportedEditingLanguage = supportedLanguages.Find(editingLanguage);
                Language supportedReferenceLanguage = supportedLanguages.Find(referenceLanguage);
                
                // 添加到活动语言（如果需要）
                Language activeEditingLanguage = activeLanguages.Find(editingLanguage);
                if (activeEditingLanguage == null && supportedEditingLanguage != null)
                {
                    activeLanguages.Add(supportedEditingLanguage);
                }
                
                Language activeReferenceLanguage = activeLanguages.Find(referenceLanguage);
                if (activeReferenceLanguage == null && supportedReferenceLanguage != null)
                {
                    activeLanguages.Add(supportedReferenceLanguage);
                }
                
                // 设置编辑和参考语言
                if (supportedEditingLanguage != null)
                    languageSettings.EditingLanguage = supportedEditingLanguage;
                    
                if (supportedReferenceLanguage != null)
                    languageSettings.ReferenceLanguage = supportedReferenceLanguage;
                
                Logger.LogInfo("项目语言设置成功", "TiaPortalOpennessManager.SetProjectLanguages");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "设置项目语言");
                return false;
            }
        }
        
        #endregion
        
        #region 对象创建
        
        /// <summary>
        /// 创建PLC程序块组
        /// </summary>
        /// <param name="parentGroup">父级程序块组</param>
        /// <param name="groupName">组名称</param>
        /// <returns>创建的程序块组</returns>
        public static PlcBlockUserGroup CreatePlcBlockGroup(PlcBlockGroup parentGroup, string groupName)
        {
            try
            {
                if (parentGroup == null)
                    throw new ArgumentNullException(nameof(parentGroup), "父级程序块组不能为空");
                    
                if (string.IsNullOrWhiteSpace(groupName))
                    throw new ArgumentException("组名称不能为空", nameof(groupName));
                
                Logger.LogInfo($"正在创建PLC程序块组: {groupName}", "TiaPortalOpennessManager.CreatePlcBlockGroup");
                
                PlcBlockUserGroup group = null;
                if (parentGroup is PlcBlockSystemGroup systemGroup)
                {
                    group = systemGroup.Groups.Create(groupName);
                }
                else if (parentGroup is PlcBlockUserGroup userGroup)
                {
                    group = userGroup.Groups.Create(groupName);
                }
                
                Logger.LogInfo($"PLC程序块组创建成功: {groupName}", "TiaPortalOpennessManager.CreatePlcBlockGroup");
                return group;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "创建PLC程序块组");
                throw;
            }
        }
        
        /// <summary>
        /// 创建PLC数据类型组
        /// </summary>
        /// <param name="parentGroup">父级数据类型组</param>
        /// <param name="groupName">组名称</param>
        /// <returns>创建的数据类型组</returns>
        public static PlcTypeUserGroup CreatePlcTypeGroup(PlcTypeGroup parentGroup, string groupName)
        {
            try
            {
                if (parentGroup == null)
                    throw new ArgumentNullException(nameof(parentGroup), "父级数据类型组不能为空");
                    
                if (string.IsNullOrWhiteSpace(groupName))
                    throw new ArgumentException("组名称不能为空", nameof(groupName));
                
                Logger.LogInfo($"正在创建PLC数据类型组: {groupName}", "TiaPortalOpennessManager.CreatePlcTypeGroup");
                
                PlcTypeUserGroup group = null;
                if (parentGroup is PlcTypeSystemGroup systemGroup)
                {
                    group = systemGroup.Groups.Create(groupName);
                }
                else if (parentGroup is PlcTypeUserGroup userGroup)
                {
                    group = userGroup.Groups.Create(groupName);
                }
                
                Logger.LogInfo($"PLC数据类型组创建成功: {groupName}", "TiaPortalOpennessManager.CreatePlcTypeGroup");
                return group;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "创建PLC数据类型组");
                throw;
            }
        }
        
        /// <summary>
        /// 创建主副本并复制软件单元
        /// </summary>
        /// <param name="sourceUnit">源软件单元</param>
        /// <param name="targetComposition">目标软件单元组合</param>
        /// <param name="projectLibrary">项目库</param>
        /// <returns>创建的新软件单元</returns>
        public static PlcUnit CreateSoftwareUnitFromMasterCopy(PlcUnit sourceUnit, PlcUnitComposition targetComposition, ProjectLibrary projectLibrary)
        {
            try
            {
                if (sourceUnit == null)
                    throw new ArgumentNullException(nameof(sourceUnit), "源软件单元不能为空");
                    
                if (targetComposition == null)
                    throw new ArgumentNullException(nameof(targetComposition), "目标软件单元组合不能为空");
                    
                if (projectLibrary == null)
                    throw new ArgumentNullException(nameof(projectLibrary), "项目库不能为空");
                
                Logger.LogInfo($"正在通过主副本复制软件单元: {sourceUnit.Name}", "TiaPortalOpennessManager.CreateSoftwareUnitFromMasterCopy");
                
                // 步骤1: 将源软件单元复制到项目库作为主副本
                MasterCopyComposition masterCopies = projectLibrary.MasterCopyFolder.MasterCopies;
                IMasterCopySource unitAsMasterCopy = (IMasterCopySource)sourceUnit;
                MasterCopy masterCopy = masterCopies.Create(unitAsMasterCopy);
                
                // 步骤2: 从主副本创建新的软件单元实例
                PlcUnit newUnit = targetComposition.CreateFrom(masterCopy);
                
                Logger.LogInfo($"软件单元复制成功: {newUnit.Name}", "TiaPortalOpennessManager.CreateSoftwareUnitFromMasterCopy");
                return newUnit;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "通过主副本复制软件单元");
                throw;
            }
        }
        
        #endregion
        
        #region 对象重命名
        
        /// <summary>
        /// 重命名PLC块
        /// </summary>
        /// <param name="block">PLC块</param>
        /// <param name="newName">新名称</param>
        /// <returns>重命名是否成功</returns>
        public static bool RenameBlock(PlcBlock block, string newName)
        {
            try
            {
                if (block == null)
                {
                    Logger.LogError("PLC块实例为空", "TiaPortalOpennessManager.RenameBlock");
                    return false;
                }
                
                if (string.IsNullOrWhiteSpace(newName))
                {
                    Logger.LogError("新名称不能为空", "TiaPortalOpennessManager.RenameBlock");
                    return false;
                }
                
                string oldName = block.Name;
                Logger.LogInfo($"正在重命名PLC块: {oldName} -> {newName}", "TiaPortalOpennessManager.RenameBlock");
                
                block.Name = newName;
                Logger.LogInfo($"PLC块重命名成功: {oldName} -> {newName}", "TiaPortalOpennessManager.RenameBlock");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "重命名PLC块");
                return false;
            }
        }
        
        /// <summary>
        /// 重命名数据类型
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <param name="newName">新名称</param>
        /// <returns>重命名是否成功</returns>
        public static bool RenameDataType(PlcStruct dataType, string newName)
        {
            try
            {
                if (dataType == null)
                {
                    Logger.LogError("数据类型实例为空", "TiaPortalOpennessManager.RenameDataType");
                    return false;
                }
                
                if (string.IsNullOrWhiteSpace(newName))
                {
                    Logger.LogError("新名称不能为空", "TiaPortalOpennessManager.RenameDataType");
                    return false;
                }
                
                string oldName = dataType.Name;
                Logger.LogInfo($"正在重命名数据类型: {oldName} -> {newName}", "TiaPortalOpennessManager.RenameDataType");
                
                dataType.Name = newName;
                Logger.LogInfo($"数据类型重命名成功: {oldName} -> {newName}", "TiaPortalOpennessManager.RenameDataType");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "重命名数据类型");
                return false;
            }
        }
        
        #endregion
        
        #region 对象复制
        
        /// <summary>
        /// 创建PLC标签表
        /// </summary>
        /// <param name="tagTableGroup">标签表组</param>
        /// <param name="tableName">标签表名称</param>
        /// <returns>创建的标签表</returns>
        public static PlcTagTable CreateTagTable(PlcTagTableGroup tagTableGroup, string tableName)
        {
            try
            {
                if (tagTableGroup == null)
                    throw new ArgumentNullException(nameof(tagTableGroup), "标签表组不能为空");
                    
                if (string.IsNullOrWhiteSpace(tableName))
                    throw new ArgumentException("标签表名称不能为空", nameof(tableName));
                
                Logger.LogInfo($"正在创建PLC标签表: {tableName}", "TiaPortalOpennessManager.CreateTagTable");
                
                PlcTagTable tagTable = tagTableGroup.TagTables.Create(tableName);
                
                Logger.LogInfo($"PLC标签表创建成功: {tableName}", "TiaPortalOpennessManager.CreateTagTable");
                return tagTable;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "创建PLC标签表");
                throw;
            }
        }
        

        
        /// <summary>
        /// 创建PLC数据类型(UDT)
        /// </summary>
        /// <param name="typeGroup">数据类型组</param>
        /// <param name="typeName">类型名称</param>
        /// <returns>创建的数据类型</returns>
        public static PlcStruct CreatePlcDataType(PlcTypeGroup typeGroup, string typeName)
        {
            try
            {
                if (typeGroup == null)
                    throw new ArgumentNullException(nameof(typeGroup), "数据类型组不能为空");
                    
                if (string.IsNullOrWhiteSpace(typeName))
                    throw new ArgumentException("类型名称不能为空", nameof(typeName));
                
                Logger.LogInfo($"正在创建PLC数据类型: {typeName}", "TiaPortalOpennessManager.CreatePlcDataType");
                
                // 注意：PlcTypeComposition没有CreateStruct方法，需要使用CreateFrom方法从主副本创建
                // 或者直接创建一个新的数据类型
                throw new NotImplementedException("CreateStruct方法不存在，需要使用CreateFrom方法从主副本创建数据类型");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "创建PLC数据类型");
                throw;
            }
        }
        
        #endregion
        
        #region 导入导出操作
        
        /// <summary>
        /// 导出程序块到文件
        /// </summary>
        /// <param name="block">程序块</param>
        /// <param name="exportPath">导出路径</param>
        /// <param name="exportOptions">导出选项</param>
        /// <returns>导出是否成功</returns>
        public static bool ExportBlock(PlcBlock block, string exportPath, ExportOptions exportOptions = ExportOptions.None)
        {
            try
            {
                if (block == null)
                {
                    Logger.LogError("程序块实例为空", "TiaPortalOpennessManager.ExportBlock");
                    return false;
                }
                
                if (string.IsNullOrWhiteSpace(exportPath))
                {
                    Logger.LogError("导出路径不能为空", "TiaPortalOpennessManager.ExportBlock");
                    return false;
                }
                
                Logger.LogInfo($"正在导出程序块: {block.Name} 到 {exportPath}", "TiaPortalOpennessManager.ExportBlock");
                
                if (exportOptions == null)
                    exportOptions = ExportOptions.None;
                
                block.Export(new FileInfo(exportPath), exportOptions);
                
                Logger.LogInfo($"程序块导出成功: {block.Name}", "TiaPortalOpennessManager.ExportBlock");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "导出程序块");
                return false;
            }
        }
        
        /// <summary>
        /// 导入程序块从文件
        /// </summary>
        /// <param name="blockGroup">程序块组</param>
        /// <param name="importPath">导入路径</param>
        /// <param name="importOptions">导入选项</param>
        /// <returns>导入是否成功</returns>
        public static bool ImportBlock(PlcBlockGroup blockGroup, string importPath, ImportOptions importOptions = ImportOptions.Override)
        {
            try
            {
                if (blockGroup == null)
                {
                    Logger.LogError("程序块组实例为空", "TiaPortalOpennessManager.ImportBlock");
                    return false;
                }
                
                if (string.IsNullOrWhiteSpace(importPath))
                {
                    Logger.LogError("导入路径不能为空", "TiaPortalOpennessManager.ImportBlock");
                    return false;
                }
                
                if (!File.Exists(importPath))
                {
                    Logger.LogError($"导入文件不存在: {importPath}", "TiaPortalOpennessManager.ImportBlock");
                    return false;
                }
                
                Logger.LogInfo($"正在导入程序块从: {importPath}", "TiaPortalOpennessManager.ImportBlock");
                
                if (importOptions == null)
                    importOptions = ImportOptions.Override;
                
                blockGroup.Blocks.Import(new FileInfo(importPath), importOptions);
                
                Logger.LogInfo($"程序块导入成功从: {importPath}", "TiaPortalOpennessManager.ImportBlock");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "导入程序块");
                return false;
            }
        }
        
        #endregion
        
        #region 设备和项目操作
        
        /// <summary>
        /// 获取TIA Portal进程列表
        /// </summary>
        /// <returns>TIA Portal进程列表</returns>
        public static IEnumerable<TiaPortalProcess> GetTiaPortalProcesses()
        {
            try
            {
                Logger.LogInfo("正在获取TIA Portal进程列表", "TiaPortalOpennessManager.GetTiaPortalProcesses");
                return TiaPortal.GetProcesses();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "获取TIA Portal进程列表");
                throw;
            }
        }
        
        /// <summary>
        /// 连接到指定的TIA Portal进程
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns>TIA Portal实例</returns>
        public static TiaPortal AttachToTiaPortalProcess(int processId)
        {
            try
            {
                Logger.LogInfo($"正在连接到TIA Portal进程: {processId}", "TiaPortalOpennessManager.AttachToTiaPortalProcess");
                
                TiaPortalProcess process = TiaPortal.GetProcess(processId);
                TiaPortal tiaPortal = process.Attach();
                
                Logger.LogInfo($"成功连接到TIA Portal进程: {processId}", "TiaPortalOpennessManager.AttachToTiaPortalProcess");
                return tiaPortal;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "连接到TIA Portal进程");
                throw;
            }
        }
        
        /// <summary>
        /// 创建新的TIA Portal实例
        /// </summary>
        /// <param name="mode">TIA Portal模式</param>
        /// <returns>TIA Portal实例</returns>
        public static TiaPortal CreateTiaPortalInstance(TiaPortalMode mode = TiaPortalMode.WithUserInterface)
        {
            try
            {
                Logger.LogInfo($"正在创建TIA Portal实例，模式: {mode}", "TiaPortalOpennessManager.CreateTiaPortalInstance");
                
                TiaPortal tiaPortal = new TiaPortal(mode);
                
                Logger.LogInfo("TIA Portal实例创建成功", "TiaPortalOpennessManager.CreateTiaPortalInstance");
                return tiaPortal;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "创建TIA Portal实例");
                throw;
            }
        }
        
        /// <summary>
        /// 遍历项目中的S7-1500设备
        /// </summary>
        /// <param name="project">项目实例</param>
        /// <returns>S7-1500设备列表</returns>
        public static List<Device> GetS71500Devices(Project project)
        {
            var devices = new List<Device>();
            
            try
            {
                if (project == null)
                {
                    Logger.LogWarning("项目实例为空", "TiaPortalOpennessManager.GetS71500Devices");
                    return devices;
                }
                
                Logger.LogInfo($"正在遍历项目 {project.Name} 的设备", "TiaPortalOpennessManager.GetS71500Devices");
                Logger.LogInfo($"项目包含 {project.Devices.Count} 个设备", "TiaPortalOpennessManager.GetS71500Devices");
                
                foreach (Device device in project.Devices)
                {
                    if (device.TypeIdentifier != null && device.TypeIdentifier == "System:Device.S71500")
                    {
                        Logger.LogInfo($"发现S7-1500设备: {device.Name}", "TiaPortalOpennessManager.GetS71500Devices");
                        devices.Add(device);
                    }
                }
                
                Logger.LogInfo($"找到 {devices.Count} 个S7-1500设备", "TiaPortalOpennessManager.GetS71500Devices");
                return devices;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "遍历项目设备");
                throw;
            }
        }
        
        /// <summary>
        /// 获取设备中的CPU
        /// </summary>
        /// <param name="device">设备实例</param>
        /// <returns>CPU设备项列表</returns>
        public static List<DeviceItem> GetCpuDeviceItems(Device device)
        {
            var cpus = new List<DeviceItem>();
            
            try
            {
                if (device == null)
                {
                    Logger.LogWarning("设备实例为空", "TiaPortalOpennessManager.GetCpuDeviceItems");
                    return cpus;
                }
                
                Logger.LogInfo($"正在获取设备 {device.Name} 中的CPU", "TiaPortalOpennessManager.GetCpuDeviceItems");
                
                foreach (DeviceItem item in device.DeviceItems)
                {
                    if (item.Classification.ToString() == "CPU")
                    {
                        Logger.LogInfo($"发现CPU: {item.Name}", "TiaPortalOpennessManager.GetCpuDeviceItems");
                        cpus.Add(item);
                    }
                }
                
                Logger.LogInfo($"在设备 {device.Name} 中找到 {cpus.Count} 个CPU", "TiaPortalOpennessManager.GetCpuDeviceItems");
                return cpus;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "获取设备CPU");
                throw;
            }
        }
        
        /// <summary>
        /// 从CPU设备项获取PLC软件
        /// </summary>
        /// <param name="cpuItem">CPU设备项</param>
        /// <returns>PLC软件实例</returns>
        public static PlcSoftware GetPlcSoftwareFromCpu(DeviceItem cpuItem)
        {
            try
            {
                if (cpuItem == null)
                {
                    Logger.LogWarning("CPU设备项为空", "TiaPortalOpennessManager.GetPlcSoftwareFromCpu");
                    return null;
                }
                
                Logger.LogInfo($"正在获取CPU {cpuItem.Name} 的软件容器", "TiaPortalOpennessManager.GetPlcSoftwareFromCpu");
                
                SoftwareContainer softwareContainer = ((IEngineeringServiceProvider)cpuItem).GetService<SoftwareContainer>();
                if (softwareContainer != null)
                {
                    PlcSoftware software = softwareContainer.Software as PlcSoftware;
                    if (software != null)
                    {
                        Logger.LogInfo($"成功获取CPU {cpuItem.Name} 的PLC软件: {software.Name}", "TiaPortalOpennessManager.GetPlcSoftwareFromCpu");
                        return software;
                    }
                    else
                    {
                        Logger.LogWarning($"CPU {cpuItem.Name} 的软件容器不包含PLC软件", "TiaPortalOpennessManager.GetPlcSoftwareFromCpu");
                    }
                }
                else
                {
                    Logger.LogWarning($"CPU {cpuItem.Name} 的软件容器为空", "TiaPortalOpennessManager.GetPlcSoftwareFromCpu");
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "获取PLC软件");
                throw;
            }
        }
        
        /// <summary>
        /// 导出项目文本
        /// </summary>
        /// <param name="project">项目实例</param>
        /// <param name="exportPath">导出路径</param>
        /// <param name="editingLanguage">编辑语言</param>
        /// <param name="referenceLanguage">参考语言</param>
        /// <returns>导出是否成功</returns>
        public static bool ExportProjectTexts(Project project, string exportPath, CultureInfo editingLanguage, CultureInfo referenceLanguage)
        {
            try
            {
                if (project == null)
                {
                    Logger.LogError("项目实例为空", "TiaPortalOpennessManager.ExportProjectTexts");
                    return false;
                }
                
                if (string.IsNullOrWhiteSpace(exportPath))
                {
                    Logger.LogError("导出路径不能为空", "TiaPortalOpennessManager.ExportProjectTexts");
                    return false;
                }
                
                Logger.LogInfo($"正在导出项目 {project.Name} 的文本到 {exportPath}", "TiaPortalOpennessManager.ExportProjectTexts");
                
                project.ExportProjectTexts(new FileInfo(exportPath), editingLanguage, referenceLanguage);
                
                Logger.LogInfo($"项目文本导出成功: {exportPath}", "TiaPortalOpennessManager.ExportProjectTexts");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "导出项目文本");
                return false;
            }
        }
        
        /// <summary>
        /// 导入项目文本
        /// </summary>
        /// <param name="project">项目实例</param>
        /// <param name="importPath">导入路径</param>
        /// <param name="editingLanguage">编辑语言</param>
        /// <param name="referenceLanguage">参考语言</param>
        /// <returns>导入是否成功</returns>
        public static bool ImportProjectTexts(Project project, string importPath, CultureInfo editingLanguage, CultureInfo referenceLanguage)
        {
            try
            {
                if (project == null)
                {
                    Logger.LogError("项目实例为空", "TiaPortalOpennessManager.ImportProjectTexts");
                    return false;
                }
                
                if (string.IsNullOrWhiteSpace(importPath))
                {
                    Logger.LogError("导入路径不能为空", "TiaPortalOpennessManager.ImportProjectTexts");
                    return false;
                }
                
                if (!File.Exists(importPath))
                {
                    Logger.LogError($"导入文件不存在: {importPath}", "TiaPortalOpennessManager.ImportProjectTexts");
                    return false;
                }
                
                Logger.LogInfo($"正在导入项目文本从 {importPath} 到项目 {project.Name}", "TiaPortalOpennessManager.ImportProjectTexts");
                
                project.ImportProjectTexts(new FileInfo(importPath), true);
                
                Logger.LogInfo($"项目文本导入成功: {importPath}", "TiaPortalOpennessManager.ImportProjectTexts");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "导入项目文本");
                return false;
            }
        }
        
        /// <summary>
        /// 项目另存为
        /// </summary>
        /// <param name="project">项目对象</param>
        /// <param name="targetDirectory">目标目录</param>
        /// <returns>另存为是否成功</returns>
        public static bool SaveProjectAs(Project project, DirectoryInfo targetDirectory)
        {
            try
            {
                if (project == null || targetDirectory == null)
                {
                    Logger.LogError("项目对象或目标目录无效", "SaveProjectAs");
                    return false;
                }

                project.SaveAs(targetDirectory);
                Logger.LogInfo($"项目另存为成功: {targetDirectory.FullName}", "SaveProjectAs");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "SaveProjectAs");
                return false;
            }
        }

        /// <summary>
        /// 关闭项目
        /// </summary>
        /// <param name="project">项目对象</param>
        /// <returns>关闭是否成功</returns>
        public static bool CloseProject(Project project)
        {
            try
            {
                if (project == null)
                {
                    Logger.LogError("项目对象无效", "CloseProject");
                    return false;
                }

                project.Close();
                Logger.LogInfo("项目关闭成功", "CloseProject");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "CloseProject");
                return false;
            }
        }

        /// <summary>
        /// 打开项目
        /// </summary>
        /// <param name="tiaPortal">TIA Portal实例</param>
        /// <param name="projectFile">项目文件</param>
        /// <returns>打开的项目对象</returns>
        public static Project OpenProject(TiaPortal tiaPortal, FileInfo projectFile)
        {
            try
            {
                if (tiaPortal == null || projectFile == null || !projectFile.Exists)
                {
                    Logger.LogError("TIA Portal实例或项目文件无效", "OpenProject");
                    return null;
                }

                var project = tiaPortal.Projects.Open(projectFile);
                Logger.LogInfo($"项目打开成功: {projectFile.FullName}", "OpenProject");
                return project;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "OpenProject");
                return null;
            }
        }
        
        #endregion
        
        #region PLC块操作
        
        /// <summary>
        /// 删除PLC块
        /// </summary>
        /// <param name="block">要删除的PLC块</param>
        /// <returns>删除是否成功</returns>
        public static bool DeletePlcBlock(PlcBlock block)
        {
            try
            {
                if (block == null)
                {
                    Logger.LogError("PLC块实例为空", "TiaPortalOpennessManager.DeletePlcBlock");
                    return false;
                }
                
                Logger.LogInfo($"正在删除PLC块: {block.Name}", "TiaPortalOpennessManager.DeletePlcBlock");
                
                block.Delete();
                
                Logger.LogInfo($"PLC块删除成功: {block.Name}", "TiaPortalOpennessManager.DeletePlcBlock");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "删除PLC块");
                return false;
            }
        }
        
        /// <summary>
        /// 删除PLC数据类型
        /// </summary>
        /// <param name="dataType">要删除的PLC数据类型</param>
        /// <returns>删除是否成功</returns>
        public static bool DeletePlcDataType(PlcType dataType)
        {
            try
            {
                if (dataType == null)
                {
                    Logger.LogError("PLC数据类型实例为空", "TiaPortalOpennessManager.DeletePlcDataType");
                    return false;
                }
                
                Logger.LogInfo($"正在删除PLC数据类型: {dataType.Name}", "TiaPortalOpennessManager.DeletePlcDataType");
                
                dataType.Delete();
                
                Logger.LogInfo($"PLC数据类型删除成功: {dataType.Name}", "TiaPortalOpennessManager.DeletePlcDataType");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "删除PLC数据类型");
                return false;
            }
        }
        
        /// <summary>
        /// 删除PLC块用户组
        /// </summary>
        /// <param name="blockGroup">要删除的PLC块用户组</param>
        /// <returns>删除是否成功</returns>
        public static bool DeletePlcBlockGroup(PlcBlockUserGroup blockGroup)
        {
            try
            {
                if (blockGroup == null)
                {
                    Logger.LogError("PLC块用户组实例为空", "TiaPortalOpennessManager.DeletePlcBlockGroup");
                    return false;
                }
                
                Logger.LogInfo($"正在删除PLC块用户组: {blockGroup.Name}", "TiaPortalOpennessManager.DeletePlcBlockGroup");
                
                blockGroup.Delete();
                
                Logger.LogInfo($"PLC块用户组删除成功: {blockGroup.Name}", "TiaPortalOpennessManager.DeletePlcBlockGroup");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "删除PLC块用户组");
                return false;
            }
        }
        
        /// <summary>
        /// 导入数据类型
        /// </summary>
        /// <param name="typeGroup">数据类型组</param>
        /// <param name="importFile">导入文件</param>
        /// <param name="importOptions">导入选项</param>
        /// <returns>导入是否成功</returns>
        public static bool ImportDataType(PlcTypeSystemGroup typeGroup, FileInfo importFile, ImportOptions importOptions)
        {
            try
            {
                if (typeGroup == null)
                {
                    Logger.LogError("数据类型组实例为空", "TiaPortalOpennessManager.ImportDataType");
                    return false;
                }
                
                if (importFile == null || !importFile.Exists)
                {
                    Logger.LogError($"导入文件无效或不存在: {importFile?.FullName}", "TiaPortalOpennessManager.ImportDataType");
                    return false;
                }
                
                Logger.LogInfo($"正在导入数据类型: {importFile.FullName}", "TiaPortalOpennessManager.ImportDataType");
                
                typeGroup.Types.Import(importFile, importOptions);
                
                Logger.LogInfo($"数据类型导入成功: {importFile.FullName}", "TiaPortalOpennessManager.ImportDataType");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "导入数据类型");
                return false;
            }
        }
        
        /// <summary>
        /// 导入PLC块
        /// </summary>
        /// <param name="software">PLC软件实例</param>
        /// <param name="importPath">导入路径</param>
        /// <param name="overwriteExisting">是否覆盖已存在的块</param>
        /// <returns>导入是否成功</returns>
        public static bool ImportPlcBlocks(PlcSoftware software, string importPath, bool overwriteExisting = false)
        {
            try
            {
                if (software == null)
                {
                    Logger.LogError("PLC软件实例为空", "TiaPortalOpennessManager.ImportPlcBlocks");
                    return false;
                }
                
                if (string.IsNullOrWhiteSpace(importPath))
                {
                    Logger.LogError("导入路径不能为空", "TiaPortalOpennessManager.ImportPlcBlocks");
                    return false;
                }
                
                if (!File.Exists(importPath))
                {
                    Logger.LogError($"导入文件不存在: {importPath}", "TiaPortalOpennessManager.ImportPlcBlocks");
                    return false;
                }
                
                Logger.LogInfo($"正在导入PLC块从 {importPath}", "TiaPortalOpennessManager.ImportPlcBlocks");
                
                // 设置导入选项
                ImportOptions importOptions = ImportOptions.None;
                if (overwriteExisting)
                {
                    importOptions = ImportOptions.Override;
                }
                
                // 执行导入
                software.BlockGroup.Blocks.Import(new FileInfo(importPath), importOptions);
                
                Logger.LogInfo($"PLC块导入成功: {importPath}", "TiaPortalOpennessManager.ImportPlcBlocks");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "导入PLC块");
                return false;
            }
        }
        
        /// <summary>
        /// 处理块导入冲突（重命名或覆盖）
        /// </summary>
        /// <param name="software">PLC软件实例</param>
        /// <param name="importPath">导入路径</param>
        /// <param name="blockName">块名称</param>
        /// <param name="newName">新名称（如果重命名）</param>
        /// <param name="overwrite">是否覆盖</param>
        /// <returns>处理是否成功</returns>
        public static bool HandleBlockImportConflict(PlcSoftware software, string importPath, string blockName, string newName = null, bool overwrite = false)
        {
            try
            {
                if (software == null)
                {
                    Logger.LogError("PLC软件实例为空", "TiaPortalOpennessManager.HandleBlockImportConflict");
                    return false;
                }
                
                Logger.LogInfo($"正在处理块导入冲突: {blockName}", "TiaPortalOpennessManager.HandleBlockImportConflict");
                
                if (overwrite)
                {
                    // 覆盖现有块
                    Logger.LogInfo($"覆盖现有块: {blockName}", "TiaPortalOpennessManager.HandleBlockImportConflict");
                    return ImportPlcBlocks(software, importPath, true);
                }
                else if (!string.IsNullOrWhiteSpace(newName))
                {
                    // 重命名导入
                    Logger.LogInfo($"重命名导入块: {blockName} -> {newName}", "TiaPortalOpennessManager.HandleBlockImportConflict");
                    
                    // 先导入到临时位置，然后重命名
                    if (ImportPlcBlocks(software, importPath, false))
                    {
                        // 查找导入的块并重命名
                        PlcBlock importedBlock = software.BlockGroup.Blocks.Find(blockName);
                        if (importedBlock != null)
                        {
                            importedBlock.Name = newName;
                            Logger.LogInfo($"块重命名成功: {blockName} -> {newName}", "TiaPortalOpennessManager.HandleBlockImportConflict");
                            return true;
                        }
                    }
                }
                
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "处理块导入冲突");
                return false;
            }
        }
        
        #endregion
        
        #region 编译操作
        
        /// <summary>
        /// 编译PLC软件
        /// </summary>
        /// <param name="software">PLC软件</param>
        /// <returns>编译结果</returns>
        public static CompilerResult CompilePlcSoftware(PlcSoftware software)
        {
            try
            {
                if (software == null)
                {
                    Logger.LogError("PLC软件实例为空", "TiaPortalOpennessManager.CompilePlcSoftware");
                    return null;
                }
                
                Logger.LogInfo($"正在编译PLC软件: {software.Name}", "TiaPortalOpennessManager.CompilePlcSoftware");
                
                ICompilable compilable = software.GetService<ICompilable>();
                if (compilable != null)
                {
                    CompilerResult result = compilable.Compile();
                    
                    if (result.State == CompilerResultState.Success)
                    {
                        Logger.LogInfo($"PLC软件编译成功: {software.Name}", "TiaPortalOpennessManager.CompilePlcSoftware");
                    }
                    else
                    {
                        Logger.LogWarning($"PLC软件编译失败: {software.Name}, 状态: {result.State}", "TiaPortalOpennessManager.CompilePlcSoftware");
                        
                        // 记录编译错误和警告
                        foreach (var message in result.Messages)
                        {
                            Logger.LogWarning($"编译消息: {message.Description}", "TiaPortalOpennessManager.CompilePlcSoftware");
                        }
                    }
                    
                    return result;
                }
                else
                {
                    Logger.LogError($"无法获取编译服务: {software.Name}", "TiaPortalOpennessManager.CompilePlcSoftware");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "编译PLC软件");
                return null;
            }
        }
        
        #endregion
        
        #region 工具方法
        
        /// <summary>
        /// 生成唯一名称
        /// </summary>
        /// <param name="baseName">基础名称</param>
        /// <param name="existingNames">已存在的名称列表</param>
        /// <returns>唯一名称</returns>
        public static string GenerateUniqueName(string baseName, IEnumerable<string> existingNames)
        {
            if (string.IsNullOrWhiteSpace(baseName))
                throw new ArgumentException("基础名称不能为空", nameof(baseName));
                
            if (existingNames == null)
                return baseName;
                
            var nameSet = new HashSet<string>(existingNames, StringComparer.OrdinalIgnoreCase);
            
            if (!nameSet.Contains(baseName))
                return baseName;
                
            int counter = 1;
            string uniqueName;
            
            do
            {
                uniqueName = $"{baseName}_{counter}";
                counter++;
            }
            while (nameSet.Contains(uniqueName));
            
            return uniqueName;
        }
        
        /// <summary>
        /// 验证对象名称是否有效
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>是否有效</returns>
        public static bool IsValidObjectName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;
                
            // TIA Portal对象名称规则：
            // - 不能以数字开头
            // - 只能包含字母、数字和下划线
            // - 长度限制（通常不超过24个字符）
            if (name.Length > 24)
                return false;
                
            if (char.IsDigit(name[0]))
                return false;
                
            foreach (char c in name)
            {
                if (!char.IsLetterOrDigit(c) && c != '_')
                    return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// 获取对象类型描述
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>类型描述</returns>
        public static string GetObjectTypeDescription(object obj)
        {
            if (obj == null)
                return "Unknown";
                
            switch (obj)
            {
                case PlcBlock block:
                    return $"PLC Block ({block.GetType().Name})";
                case PlcStruct dataType:
                    return "PLC Data Type";
                case PlcTagTable tagTable:
                    return "Tag Table";
                case PlcSoftware software:
                    return "PLC Software";
                case PlcUnit unit:
                    return "Software Unit";
                case Device device:
                    return $"Device ({device.TypeIdentifier})";
                case DeviceItem deviceItem:
                    return $"Device Item ({deviceItem.Classification})";
                default:
                    return obj.GetType().Name;
            }
        }
        
        #endregion
        
        #region 命名空间管理
        
        /// <summary>
        /// 批量修改软件单元及其子元素的命名空间
        /// </summary>
        /// <param name="softwareUnit">软件单元</param>
        /// <param name="newNamespace">新的命名空间</param>
        /// <param name="includeBlocks">是否包含程序块</param>
        /// <param name="includeDataTypes">是否包含数据类型(UDT)</param>
        /// <returns>修改结果信息</returns>
        public static NamespaceUpdateResult UpdateSoftwareUnitNamespace(PlcUnit softwareUnit, string newNamespace, bool includeBlocks = true, bool includeDataTypes = true)
        {
            var result = new NamespaceUpdateResult();
            
            try
            {
                if (softwareUnit == null)
                {
                    result.ErrorMessage = "软件单元实例为空";
                    Logger.LogError(result.ErrorMessage, "TiaPortalOpennessManager.UpdateSoftwareUnitNamespace");
                    return result;
                }
                
                if (string.IsNullOrWhiteSpace(newNamespace))
                {
                    result.ErrorMessage = "命名空间不能为空";
                    Logger.LogError(result.ErrorMessage, "TiaPortalOpennessManager.UpdateSoftwareUnitNamespace");
                    return result;
                }
                
                Logger.LogInfo($"开始批量修改软件单元 {softwareUnit.Name} 的命名空间为: {newNamespace}", "TiaPortalOpennessManager.UpdateSoftwareUnitNamespace");
                
                // 1. 修改软件单元本身的命名空间
                try
                {
                    softwareUnit.SetAttribute("NamespacePreset", newNamespace);
                    result.UpdatedSoftwareUnits++;
                    Logger.LogInfo($"软件单元 {softwareUnit.Name} 命名空间修改成功", "TiaPortalOpennessManager.UpdateSoftwareUnitNamespace");
                }
                catch (Exception ex)
                {
                    result.FailedItems.Add($"软件单元 {softwareUnit.Name}: {ex.Message}");
                    Logger.LogWarning($"软件单元 {softwareUnit.Name} 命名空间修改失败: {ex.Message}", "TiaPortalOpennessManager.UpdateSoftwareUnitNamespace");
                }
                
                // 2. 修改程序块的命名空间
                if (includeBlocks)
                {
                    UpdateBlocksNamespace(softwareUnit.BlockGroup, newNamespace, result);
                }
                
                // 3. 修改数据类型(UDT)的命名空间
                if (includeDataTypes)
                {
                    UpdateDataTypesNamespace(softwareUnit.TypeGroup, newNamespace, result);
                }
                
                result.IsSuccess = result.FailedItems.Count == 0;
                
                Logger.LogInfo($"批量命名空间修改完成。成功: 软件单元={result.UpdatedSoftwareUnits}, 程序块={result.UpdatedBlocks}, 数据类型={result.UpdatedDataTypes}, 失败={result.FailedItems.Count}", "TiaPortalOpennessManager.UpdateSoftwareUnitNamespace");
                
                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                Logger.LogException(ex, "批量修改软件单元命名空间");
                return result;
            }
        }
        
        /// <summary>
        /// 递归修改程序块组中所有程序块的命名空间
        /// </summary>
        /// <param name="blockGroup">程序块组</param>
        /// <param name="newNamespace">新的命名空间</param>
        /// <param name="result">结果对象</param>
        private static void UpdateBlocksNamespace(PlcBlockGroup blockGroup, string newNamespace, NamespaceUpdateResult result)
        {
            if (blockGroup == null) return;
            
            try
            {
                // 修改当前组中的所有程序块
                foreach (PlcBlock block in blockGroup.Blocks)
                {
                    try
                    {
                        block.SetAttribute("Namespace", newNamespace);
                        result.UpdatedBlocks++;
                        Logger.LogInfo($"程序块 {block.Name} 命名空间修改成功", "TiaPortalOpennessManager.UpdateBlocksNamespace");
                    }
                    catch (Exception ex)
                    {
                        result.FailedItems.Add($"程序块 {block.Name}: {ex.Message}");
                        Logger.LogWarning($"程序块 {block.Name} 命名空间修改失败: {ex.Message}", "TiaPortalOpennessManager.UpdateBlocksNamespace");
                    }
                }
                
                // 递归处理子组
                foreach (PlcBlockGroup subGroup in blockGroup.Groups)
                {
                    UpdateBlocksNamespace(subGroup, newNamespace, result);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "修改程序块组命名空间");
            }
        }
        
        /// <summary>
        /// 递归修改数据类型组中所有UDT的命名空间
        /// </summary>
        /// <param name="typeGroup">数据类型组</param>
        /// <param name="newNamespace">新的命名空间</param>
        /// <param name="result">结果对象</param>
        private static void UpdateDataTypesNamespace(PlcTypeGroup typeGroup, string newNamespace, NamespaceUpdateResult result)
        {
            if (typeGroup == null) return;
            
            try
            {
                // 修改当前组中的所有数据类型
                foreach (PlcType dataType in typeGroup.Types)
                {
                    try
                    {
                        // 只处理UDT类型
                        if (dataType is PlcStruct udtType)
                        {
                            udtType.SetAttribute("Namespace", newNamespace);
                            result.UpdatedDataTypes++;
                            Logger.LogInfo($"数据类型 {dataType.Name} 命名空间修改成功", "TiaPortalOpennessManager.UpdateDataTypesNamespace");
                        }
                    }
                    catch (Exception ex)
                    {
                        result.FailedItems.Add($"数据类型 {dataType.Name}: {ex.Message}");
                        Logger.LogWarning($"数据类型 {dataType.Name} 命名空间修改失败: {ex.Message}", "TiaPortalOpennessManager.UpdateDataTypesNamespace");
                    }
                }
                
                // 递归处理子组
                foreach (PlcTypeGroup subGroup in typeGroup.Groups)
                {
                    UpdateDataTypesNamespace(subGroup, newNamespace, result);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "修改数据类型组命名空间");
            }
        }
        
        /// <summary>
        /// 获取软件单元的当前命名空间
        /// </summary>
        /// <param name="softwareUnit">软件单元</param>
        /// <returns>当前命名空间</returns>
        public static string GetSoftwareUnitNamespace(PlcUnit softwareUnit)
        {
            try
            {
                if (softwareUnit == null)
                {
                    Logger.LogError("软件单元实例为空", "TiaPortalOpennessManager.GetSoftwareUnitNamespace");
                    return string.Empty;
                }
                
                string currentNamespace = softwareUnit.GetAttribute("NamespacePreset")?.ToString() ?? string.Empty;
                Logger.LogInfo($"软件单元 {softwareUnit.Name} 当前命名空间: {currentNamespace}", "TiaPortalOpennessManager.GetSoftwareUnitNamespace");
                return currentNamespace;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "获取软件单元命名空间");
                return string.Empty;
            }
        }
        
        /// <summary>
        /// 获取程序块的当前命名空间
        /// </summary>
        /// <param name="block">程序块</param>
        /// <returns>当前命名空间</returns>
        public static string GetBlockNamespace(PlcBlock block)
        {
            try
            {
                if (block == null)
                {
                    Logger.LogError("程序块实例为空", "TiaPortalOpennessManager.GetBlockNamespace");
                    return string.Empty;
                }
                
                string currentNamespace = block.GetAttribute("Namespace")?.ToString() ?? string.Empty;
                Logger.LogInfo($"程序块 {block.Name} 当前命名空间: {currentNamespace}", "TiaPortalOpennessManager.GetBlockNamespace");
                return currentNamespace;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "获取程序块命名空间");
                return string.Empty;
            }
        }
        
        /// <summary>
        /// 获取数据类型的当前命名空间
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <returns>当前命名空间</returns>
        public static string GetDataTypeNamespace(PlcStruct dataType)
        {
            try
            {
                if (dataType == null)
                {
                    Logger.LogError("数据类型实例为空", "TiaPortalOpennessManager.GetDataTypeNamespace");
                    return string.Empty;
                }
                
                string currentNamespace = dataType.GetAttribute("Namespace")?.ToString() ?? string.Empty;
                Logger.LogInfo($"数据类型 {dataType.Name} 当前命名空间: {currentNamespace}", "TiaPortalOpennessManager.GetDataTypeNamespace");
                return currentNamespace;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "获取数据类型命名空间");
                return string.Empty;
            }
        }
        
        #endregion
        
        #region 通用工具方法
        
        /// <summary>
        /// 获取TIA Portal对象的名称
        /// </summary>
        /// <param name="obj">TIA Portal对象</param>
        /// <returns>对象名称，如果获取失败返回空字符串</returns>
        public static string GetObjectName(object obj)
        {
            try
            {
                if (obj == null)
                {
                    Logger.LogWarning("对象为空，无法获取名称", "TiaPortalOpennessManager.GetObjectName");
                    return string.Empty;
                }
                
                // 使用反射获取Name属性
                var nameProperty = obj.GetType().GetProperty("Name");
                if (nameProperty != null && nameProperty.CanRead)
                {
                    var name = nameProperty.GetValue(obj)?.ToString() ?? string.Empty;
                    Logger.LogInfo($"获取对象名称成功: {name} (类型: {obj.GetType().Name})", "TiaPortalOpennessManager.GetObjectName");
                    return name;
                }
                
                Logger.LogWarning($"对象类型 {obj.GetType().Name} 不包含Name属性", "TiaPortalOpennessManager.GetObjectName");
                return string.Empty;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "获取对象名称");
                return string.Empty;
            }
        }
        
        /// <summary>
        /// 获取PlcSoftware对象的名称
        /// </summary>
        /// <param name="software">PlcSoftware对象</param>
        /// <returns>软件名称</returns>
        public static string GetPlcSoftwareName(PlcSoftware software)
        {
            try
            {
                if (software == null)
                {
                    Logger.LogError("PlcSoftware对象为空", "TiaPortalOpennessManager.GetPlcSoftwareName");
                    return string.Empty;
                }
                
                string name = software.Name;
                Logger.LogInfo($"获取PlcSoftware名称: {name}", "TiaPortalOpennessManager.GetPlcSoftwareName");
                return name;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "获取PlcSoftware名称");
                return string.Empty;
            }
        }
        
        /// <summary>
        /// 获取PlcBlock对象的名称
        /// </summary>
        /// <param name="block">PlcBlock对象</param>
        /// <returns>程序块名称</returns>
        public static string GetPlcBlockName(PlcBlock block)
        {
            try
            {
                if (block == null)
                {
                    Logger.LogError("PlcBlock对象为空", "TiaPortalOpennessManager.GetPlcBlockName");
                    return string.Empty;
                }
                
                string name = block.Name;
                Logger.LogInfo($"获取PlcBlock名称: {name}", "TiaPortalOpennessManager.GetPlcBlockName");
                return name;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "获取PlcBlock名称");
                return string.Empty;
            }
        }
        
        /// <summary>
        /// 获取PlcType对象的名称
        /// </summary>
        /// <param name="type">PlcType对象</param>
        /// <returns>数据类型名称</returns>
        public static string GetPlcTypeName(PlcType type)
        {
            try
            {
                if (type == null)
                {
                    Logger.LogError("PlcType对象为空", "TiaPortalOpennessManager.GetPlcTypeName");
                    return string.Empty;
                }
                
                string name = type.Name;
                Logger.LogInfo($"获取PlcType名称: {name}", "TiaPortalOpennessManager.GetPlcTypeName");
                return name;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "获取PlcType名称");
                return string.Empty;
            }
        }
        
        /// <summary>
        /// 获取PlcTagTable对象的名称
        /// </summary>
        /// <param name="tagTable">PlcTagTable对象</param>
        /// <returns>标签表名称</returns>
        public static string GetPlcTagTableName(PlcTagTable tagTable)
        {
            try
            {
                if (tagTable == null)
                {
                    Logger.LogError("PlcTagTable对象为空", "TiaPortalOpennessManager.GetPlcTagTableName");
                    return string.Empty;
                }
                
                string name = tagTable.Name;
                Logger.LogInfo($"获取PlcTagTable名称: {name}", "TiaPortalOpennessManager.GetPlcTagTableName");
                return name;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "获取PlcTagTable名称");
                return string.Empty;
            }
        }
        
        #endregion
        
        #region 嵌套类定义
        
        /// <summary>
        /// 命名空间更新结果类
        /// </summary>
        public class NamespaceUpdateResult
        {
            /// <summary>
            /// 操作是否成功
            /// </summary>
            public bool IsSuccess { get; set; }
            
            /// <summary>
            /// 错误消息
            /// </summary>
            public string ErrorMessage { get; set; }
            
            /// <summary>
            /// 更新的软件单元数量
            /// </summary>
            public int UpdatedSoftwareUnits { get; set; }
            
            /// <summary>
            /// 更新的程序块数量
            /// </summary>
            public int UpdatedBlocks { get; set; }
            
            /// <summary>
            /// 更新的数据类型数量
            /// </summary>
            public int UpdatedDataTypes { get; set; }
            
            /// <summary>
            /// 失败的项目列表
            /// </summary>
            public List<string> FailedItems { get; set; } = new List<string>();
            
            /// <summary>
            /// 获取总的更新数量
            /// </summary>
            public int TotalUpdated => UpdatedSoftwareUnits + UpdatedBlocks + UpdatedDataTypes;
            
            /// <summary>
            /// 获取结果摘要
            /// </summary>
            /// <returns>结果摘要字符串</returns>
            public string GetSummary()
            {
                if (!IsSuccess && !string.IsNullOrEmpty(ErrorMessage))
                {
                    return $"操作失败: {ErrorMessage}";
                }
                
                var summary = $"命名空间更新完成。成功更新: 软件单元={UpdatedSoftwareUnits}, 程序块={UpdatedBlocks}, 数据类型={UpdatedDataTypes}";
                
                if (FailedItems.Count > 0)
                {
                    summary += $", 失败={FailedItems.Count}项";
                }
                
                return summary;
            }
        }
        
        #endregion
    }
}