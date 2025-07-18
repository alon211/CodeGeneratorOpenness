///
/// Sample applicatin for automated code generation for Siemens TIA Portal with Openness Interface
/// 
/// by Mark König @ 02/2020
/// 
/// cFunctionGroup contains some functions for blocks (recursive)
///

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.Types;
using Siemens.Engineering.SW.Tags;
using Siemens.Engineering.SW.ExternalSources;
using Siemens.Engineering.SW.Alarm.TextLists;
using Siemens.Engineering.SW.WatchAndForceTables;
using Siemens.Engineering.SW.TechnologicalObjects;
using Siemens.Engineering.SW.Units;

namespace CodeGeneratorOpenness
{
    class cFunctionGroups
    {
        // 图标索引常量
         // 图标索引常量
        private const int DEFAULT_ICON_INDEX=0;
        private const int FOLDERCLOSED_ICON_INDEX = 1;
        private const int OB_ICON_INDEX = 2;
        private const int FB_ICON_INDEX = 3;
        private const int FC_ICON_INDEX = 4;
        private const int DB_ICON_INDEX = 5;
        private const int SAFEDB_ICON_INDEX = 6;
        private const int SAFEOB_ICON_INDEX = 7;
        private const int SAFEFB_ICON_INDEX = 8;
        private const int DATATYPE_ICON_INDEX = 9;
        private const int TAG_TABLE_ICON_INDEX = 10;  // tag_table.png
        private const int SOFTWARE_UNITS_ICON_INDEX = 11;  // softwareUnits.png
        public void ClearTreeView(TreeView Tree)
        {
            Tree.Nodes.Clear();
        }

        public void LoadTreeView(TreeView Tree, PlcSoftware Software)
        {
            if (Software == null)
            {
                Logger.LogWarning("软件对象为空，无法加载TreeView", "LoadTreeView");
                return;
            }

            Logger.LogInfo($"开始加载软件 {Software.Name} 到TreeView", "LoadTreeView");

            try
            {
                // start update treeview
                Tree.BeginUpdate();

                // add root node
                TreeNode root = new TreeNode(Software.Name);
                Tree.Nodes.Add(root);

                // 自动加载所有PlcSoftware的结构组件
                LoadPlcSoftwareStructures(Software, root);

                root.Expand();
                Logger.LogInfo("TreeView加载完成", "LoadTreeView");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "加载树视图");
                MessageBox.Show(String.Format("加载树视图时发生错误:\n{0}", ex.Message), 
                              "树视图加载错误", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Error);
            }
            finally
            {
                // end update
                Tree.EndUpdate();
            }
        }

        /// <summary>
        /// 自动加载PlcSoftware的所有结构组件
        /// </summary>
        /// <param name="software">PlcSoftware对象</param>
        /// <param name="root">根节点</param>
        private void LoadPlcSoftwareStructures(PlcSoftware software, TreeNode root)
        {
            Logger.LogInfo("开始自动加载PlcSoftware的所有结构组件", "LoadPlcSoftwareStructures");

            // 1. 加载程序块组 (BlockGroup)
            LoadBlockGroup(software, root);

            // 2. 加载数据类型组 (TypeGroup)
            LoadTypeGroup(software, root);

            // 3. 加载标签表组 (TagTableGroup)
            LoadTagTableGroup(software, root);

            // 4. 加载外部源文件组 (ExternalSourceGroup)
            LoadExternalSourceGroup(software, root);

            // 5. 加载PLC报警文本列表组 (PlcAlarmTextlistGroup)
            LoadPlcAlarmTextlistGroup(software, root);

            // 6. 加载技术对象组 (TechnologicalObjectGroup)
            LoadTechnologicalObjectGroup(software, root);

            // 7. 加载监视和强制表组 (WatchAndForceTableGroup)
            LoadWatchAndForceTableGroup(software, root);

            // 8. 加载软件单元组 (Software Units)
            LoadSoftwareUnitsGroup(software, root);

            Logger.LogInfo("PlcSoftware结构组件加载完成", "LoadPlcSoftwareStructures");
        }

        /// <summary>
        /// 获取程序块组中的块数量
        /// </summary>
        private int GetBlockCount(PlcBlockGroup blockGroup)
        {
            int count = 0;
            try
            {
                count += blockGroup.Blocks.Count;
                foreach (PlcBlockGroup subGroup in blockGroup.Groups)
                {
                    count += GetBlockCount(subGroup);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "获取程序块数量");
            }
            return count;
        }

        /// <summary>
        /// 获取数据类型组中的类型数量
        /// </summary>
        private int GetTypeCount(PlcTypeGroup typeGroup)
        {
            int count = 0;
            try
            {
                count += typeGroup.Types.Count;
                foreach (PlcTypeGroup subGroup in typeGroup.Groups)
                {
                    count += GetTypeCount(subGroup);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "获取数据类型数量");
            }
            return count;
        }

        /// <summary>
        /// 添加标签表到TreeView
        /// </summary>
        private void AddTagTables(PlcTagTableGroup tagTableGroup, TreeNode node)
        {
            try
            {
                foreach (var tagTable in tagTableGroup.TagTables)
                {
                    TreeNode tagTableNode = new TreeNode(tagTable.Name);
                    tagTableNode.Tag = tagTable;
                    tagTableNode.ImageIndex = TAG_TABLE_ICON_INDEX;
                    tagTableNode.SelectedImageIndex = TAG_TABLE_ICON_INDEX;
                    node.Nodes.Add(tagTableNode);
                }

                foreach (PlcTagTableGroup subGroup in tagTableGroup.Groups)
                {
                    TreeNode groupNode = new TreeNode(subGroup.Name);
                    groupNode.Tag = subGroup;
                    groupNode.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                    groupNode.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;
                    AddTagTables(subGroup, groupNode);
                    node.Nodes.Add(groupNode);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "添加标签表");
            }
        }

        /// <summary>
        /// 添加外部源文件到TreeView
        /// </summary>
        private void AddExternalSources(PlcExternalSourceGroup externalSourceGroup, TreeNode node)
        {
            try
            {
                foreach (var externalSource in externalSourceGroup.ExternalSources)
                {
                    TreeNode sourceNode = new TreeNode(externalSource.Name);
                    sourceNode.Tag = externalSource;
                    sourceNode.ImageIndex = SOFTWARE_UNITS_ICON_INDEX;
                    sourceNode.SelectedImageIndex = SOFTWARE_UNITS_ICON_INDEX;
                    node.Nodes.Add(sourceNode);
                }

                foreach (PlcExternalSourceGroup subGroup in externalSourceGroup.Groups)
                {
                    TreeNode groupNode = new TreeNode(subGroup.Name);
                    groupNode.Tag = subGroup;
                    groupNode.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                    groupNode.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;
                    AddExternalSources(subGroup, groupNode);
                    node.Nodes.Add(groupNode);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "添加外部源文件");
            }
        }

        /// <summary>
        /// 添加PLC报警文本列表到TreeView
        /// </summary>
        private void AddAlarmTextlists(PlcAlarmTextlistGroup alarmTextlistGroup, TreeNode node)
        {
            try
            {
                // 添加系统报警文本列表
                foreach (var alarmTextlist in alarmTextlistGroup.PlcAlarmSystemTextlists)
                {
                    TreeNode alarmNode = new TreeNode($"[System] {alarmTextlist.Name}");
                    alarmNode.Tag = alarmTextlist;
                    alarmNode.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                    alarmNode.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;
                    node.Nodes.Add(alarmNode);
                }

                // 添加用户报警文本列表
                foreach (var alarmTextlist in alarmTextlistGroup.PlcAlarmUserTextlists)
                {
                    TreeNode alarmNode = new TreeNode($"[User] {alarmTextlist.Name}");
                    alarmNode.Tag = alarmTextlist;
                    alarmNode.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                    alarmNode.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;
                    node.Nodes.Add(alarmNode);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "添加PLC报警文本列表");
            }
        }

        /// <summary>
        /// 添加技术对象到TreeView
        /// </summary>
        private void AddTechnologicalObjects(TechnologicalInstanceDBGroup techObjectGroup, TreeNode node)
        {
            try
            {
                foreach (var techObject in techObjectGroup.TechnologicalObjects)
                {
                    TreeNode techNode = new TreeNode(techObject.Name);
                    techNode.Tag = techObject;
                    techNode.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                    techNode.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;
                    node.Nodes.Add(techNode);
                }

                foreach (TechnologicalInstanceDBGroup subGroup in techObjectGroup.Groups)
                {
                    TreeNode groupNode = new TreeNode(subGroup.Name);
                    groupNode.Tag = subGroup;
                    groupNode.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                    groupNode.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;
                    AddTechnologicalObjects(subGroup, groupNode);
                    node.Nodes.Add(groupNode);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "添加技术对象");
            }
        }

        /// <summary>
        /// 添加监视和强制表到TreeView
        /// </summary>
        private void AddWatchForceTables(PlcWatchAndForceTableGroup watchForceTableGroup, TreeNode node)
        {
            try
            {
                // 添加监视表
                foreach (var watchTable in watchForceTableGroup.WatchTables)
                {
                    TreeNode tableNode = new TreeNode($"[Watch] {watchTable.Name}");
                    tableNode.Tag = watchTable;
                    tableNode.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                    tableNode.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;
                    node.Nodes.Add(tableNode);
                }

                // 添加强制表
                foreach (var forceTable in watchForceTableGroup.ForceTables)
                {
                    TreeNode tableNode = new TreeNode($"[Force] {forceTable.Name}");
                    tableNode.Tag = forceTable;
                    tableNode.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                    tableNode.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;
                    node.Nodes.Add(tableNode);
                }

                foreach (PlcWatchAndForceTableGroup subGroup in watchForceTableGroup.Groups)
                {
                    TreeNode groupNode = new TreeNode(subGroup.Name);
                    groupNode.Tag = subGroup;
                    groupNode.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                    groupNode.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;
                    AddWatchForceTables(subGroup, groupNode);
                    node.Nodes.Add(groupNode);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "添加监视和强制表");
            }
        }

        /// <summary>
        /// 加载程序块组
        /// </summary>
        private void LoadBlockGroup(PlcSoftware software, TreeNode root)
        {
            try
            {
                Logger.LogInfo("开始加载程序块组", "LoadBlockGroup");
                if (software.BlockGroup != null)
                {
                    TreeNode programBlocks = new TreeNode("Program Blocks");
                    programBlocks.Tag = software.BlockGroup;
                    programBlocks.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                    programBlocks.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;
                    root.Nodes.Add(programBlocks);

                    AddPlcBlocks(software.BlockGroup, programBlocks);
                    programBlocks.Expand();
                    Logger.LogInfo($"程序块组加载完成，共{GetBlockCount(software.BlockGroup)}个块", "LoadBlockGroup");
                }
                else
                {
                    Logger.LogWarning("程序块组为空", "LoadBlockGroup");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "加载程序块组");
                MessageBox.Show(String.Format("加载程序块时发生错误:\n{0}", ex.Message), 
                              "程序块加载错误", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 加载数据类型组
        /// </summary>
        private void LoadTypeGroup(PlcSoftware software, TreeNode root)
        {
            try
            {
                Logger.LogInfo("开始加载数据类型组", "LoadTypeGroup");
                if (software.TypeGroup != null)
                {
                    TreeNode dataTypes = new TreeNode("PLC Data Types");
                    dataTypes.Tag = software.TypeGroup;
                    dataTypes.ImageIndex = DATATYPE_ICON_INDEX;
                    dataTypes.SelectedImageIndex = DATATYPE_ICON_INDEX;
                    root.Nodes.Add(dataTypes);

                    AddPlcTypes(software.TypeGroup, dataTypes);
                    dataTypes.Expand();
                    Logger.LogInfo($"数据类型组加载完成，共{GetTypeCount(software.TypeGroup)}个类型", "LoadTypeGroup");
                }
                else
                {
                    Logger.LogWarning("数据类型组为空", "LoadTypeGroup");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "加载数据类型组");
                MessageBox.Show(String.Format("加载数据类型时发生错误:\n{0}", ex.Message), 
                              "数据类型加载错误", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 加载标签表组
        /// </summary>
        private void LoadTagTableGroup(PlcSoftware software, TreeNode root)
        {
            try
            {
                Logger.LogInfo("开始加载标签表组", "LoadTagTableGroup");
                if (software.TagTableGroup != null)
                {
                    TreeNode tagTables = new TreeNode("Tag Tables");
                    tagTables.Tag = software.TagTableGroup;
                    tagTables.ImageIndex = TAG_TABLE_ICON_INDEX;
                    tagTables.SelectedImageIndex = TAG_TABLE_ICON_INDEX;
                    root.Nodes.Add(tagTables);

                    AddTagTables(software.TagTableGroup, tagTables);
                    Logger.LogInfo($"标签表组加载完成，共{software.TagTableGroup.TagTables.Count}个标签表", "LoadTagTableGroup");
                }
                else
                {
                    Logger.LogWarning("标签表组为空", "LoadTagTableGroup");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "加载标签表组");
                Logger.LogWarning($"标签表组加载失败: {ex.Message}", "LoadTagTableGroup");
            }
        }

        /// <summary>
        /// 加载外部源文件组
        /// </summary>
        private void LoadExternalSourceGroup(PlcSoftware software, TreeNode root)
        {
            try
            {
                Logger.LogInfo("开始加载外部源文件组", "LoadExternalSourceGroup");
                if (software.ExternalSourceGroup != null)
                {
                    TreeNode externalSources = new TreeNode("External Sources");
                    externalSources.Tag = software.ExternalSourceGroup;
                    externalSources.ImageIndex = FOLDERCLOSED_ICON_INDEX; // 使用通用组图标
                    externalSources.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;
                    root.Nodes.Add(externalSources);

                    AddExternalSources(software.ExternalSourceGroup, externalSources);
                    Logger.LogInfo($"外部源文件组加载完成，共{software.ExternalSourceGroup.ExternalSources.Count}个源文件", "LoadExternalSourceGroup");
                }
                else
                {
                    Logger.LogWarning("外部源文件组为空", "LoadExternalSourceGroup");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "加载外部源文件组");
                Logger.LogWarning($"外部源文件组加载失败: {ex.Message}", "LoadExternalSourceGroup");
            }
        }

        /// <summary>
        /// 加载PLC报警文本列表组
        /// </summary>
        private void LoadPlcAlarmTextlistGroup(PlcSoftware software, TreeNode root)
        {
            try
            {
                Logger.LogInfo("开始加载PLC报警文本列表组", "LoadPlcAlarmTextlistGroup");
                if (software.PlcAlarmTextlistGroup != null)
                {
                    TreeNode alarmTextlists = new TreeNode("Alarm Textlists");
                    alarmTextlists.Tag = software.PlcAlarmTextlistGroup;
                    alarmTextlists.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                    alarmTextlists.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;
                    root.Nodes.Add(alarmTextlists);

                    AddAlarmTextlists(software.PlcAlarmTextlistGroup, alarmTextlists);
                    int systemCount = software.PlcAlarmTextlistGroup.PlcAlarmSystemTextlists.Count;
                int userCount = software.PlcAlarmTextlistGroup.PlcAlarmUserTextlists.Count;
                Logger.LogInfo($"PLC报警文本列表组加载完成，共{systemCount + userCount}个文本列表（系统:{systemCount}，用户:{userCount}）", "LoadPlcAlarmTextlistGroup");
                }
                else
                {
                    Logger.LogWarning("PLC报警文本列表组为空", "LoadPlcAlarmTextlistGroup");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "加载PLC报警文本列表组");
                Logger.LogWarning($"PLC报警文本列表组加载失败: {ex.Message}", "LoadPlcAlarmTextlistGroup");
            }
        }

        /// <summary>
        /// 加载技术对象组
        /// </summary>
        private void LoadTechnologicalObjectGroup(PlcSoftware software, TreeNode root)
        {
            try
            {
                Logger.LogInfo("开始加载技术对象组", "LoadTechnologicalObjectGroup");
                if (software.TechnologicalObjectGroup != null)
                {
                    TreeNode techObjects = new TreeNode("Technological Objects");
                    techObjects.Tag = software.TechnologicalObjectGroup;
                    techObjects.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                    techObjects.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;
                    root.Nodes.Add(techObjects);

                    AddTechnologicalObjects(software.TechnologicalObjectGroup, techObjects);
                    Logger.LogInfo($"技术对象组加载完成，共{software.TechnologicalObjectGroup.TechnologicalObjects.Count}个技术对象", "LoadTechnologicalObjectGroup");
                }
                else
                {
                    Logger.LogWarning("技术对象组为空", "LoadTechnologicalObjectGroup");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "加载技术对象组");
                Logger.LogWarning($"技术对象组加载失败: {ex.Message}", "LoadTechnologicalObjectGroup");
            }
        }

        /// <summary>
        /// 加载监视和强制表组
        /// </summary>
        private void LoadWatchAndForceTableGroup(PlcSoftware software, TreeNode root)
        {
            try
            {
                Logger.LogInfo("开始加载监视和强制表组", "LoadWatchAndForceTableGroup");
                if (software.WatchAndForceTableGroup != null)
                {
                    TreeNode watchForceTables = new TreeNode("Watch & Force Tables");
                    watchForceTables.Tag = software.WatchAndForceTableGroup;
                    watchForceTables.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                    watchForceTables.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;
                    root.Nodes.Add(watchForceTables);

                    AddWatchForceTables(software.WatchAndForceTableGroup, watchForceTables);
                    int watchCount = software.WatchAndForceTableGroup.WatchTables.Count;
                int forceCount = software.WatchAndForceTableGroup.ForceTables.Count;
                Logger.LogInfo($"监视和强制表组加载完成，共{watchCount + forceCount}个表（监视:{watchCount}，强制:{forceCount}）", "LoadWatchAndForceTableGroup");
                }
                else
                {
                    Logger.LogWarning("监视和强制表组为空", "LoadWatchAndForceTableGroup");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "加载监视和强制表组");
                Logger.LogWarning($"监视和强制表组加载失败: {ex.Message}", "LoadWatchAndForceTableGroup");
            }
        }
        public void AddPlcBlocks(PlcBlockGroup plcGroup, TreeNode node)
        {
            // first add all plc blocks
            foreach (PlcBlock plcBlock in plcGroup.Blocks)
            {
                TreeNode n = null;

                if (plcBlock is OB)
                {
                    n = new TreeNode(plcBlock.Name + " [OB" + plcBlock.Number.ToString() + "]");
                    n.Tag = plcBlock;
                    //n.ToolTipText = "Version " + plcBlock.HeaderVersion.ToString();
                    n.ImageIndex = OB_ICON_INDEX;

                    OB ob = (OB)plcBlock;
                    if (ob.SecondaryType.Contains("Safe"))
                    {
                        n.BackColor = Color.Yellow;
                        n.ImageIndex = SAFEOB_ICON_INDEX;
                    }
                }
                else if (plcBlock is FB)
                {
                    n = new TreeNode(plcBlock.Name + " [FB" + plcBlock.Number.ToString() + "]");
                    n.Tag = plcBlock;
                    n.ImageIndex = FB_ICON_INDEX;

                    FB fb = (FB)plcBlock;
                    if ((fb.ProgrammingLanguage == ProgrammingLanguage.F_LAD) ||
                        (fb.ProgrammingLanguage == ProgrammingLanguage.F_FBD))
                    {
                        n.BackColor = Color.Yellow;
                        n.ImageIndex = SAFEFB_ICON_INDEX;
                    }
                }
                else if (plcBlock is FC)
                {
                    n = new TreeNode(plcBlock.Name + " [FC" + plcBlock.Number.ToString() + "]");
                    n.Tag = plcBlock;
                    n.ImageIndex = FC_ICON_INDEX;
                }
                else if (plcBlock is InstanceDB)
                {
                    n = new TreeNode(plcBlock.Name + " [DB" + plcBlock.Number.ToString() + "]");
                    n.Tag = plcBlock;
                    n.ImageIndex = DB_ICON_INDEX;

                    InstanceDB db = (InstanceDB)plcBlock;
                    n.Name = db.Name + "[DB" + db.Number.ToString() + "]";
                    if (db.ProgrammingLanguage == ProgrammingLanguage.F_DB)
                    {
                        n.BackColor = Color.Yellow;
                        n.ImageIndex = SAFEDB_ICON_INDEX;
                    }
                }
                else if (plcBlock is GlobalDB)
                { 
                    n = new TreeNode(plcBlock.Name + " [DB" + plcBlock.Number.ToString() + "]");
                    n.Tag = plcBlock;
                    n.ImageIndex = DB_ICON_INDEX;

                    GlobalDB db = (GlobalDB)plcBlock;
                    n.Name = db.Name + "[FB" + db.Number.ToString() + "]";
                    if (db.ProgrammingLanguage == ProgrammingLanguage.F_DB)
                    {
                        n.BackColor = Color.Yellow;
                        n.ImageIndex = SAFEDB_ICON_INDEX;
                    }
                }
                else if (plcBlock is ArrayDB)
                {
                    n = new TreeNode(plcBlock.Name + " [DB" + plcBlock.Number.ToString() + "]");
                    n.Tag = plcBlock;
                    n.ImageIndex = DB_ICON_INDEX;

                    ArrayDB db = (ArrayDB)plcBlock;
                    n.Name = db.Name + "[FB" + db.Number.ToString() + "]";
                    if (db.ProgrammingLanguage == ProgrammingLanguage.F_DB)
                    {
                        n.BackColor = Color.Yellow;
                        n.ImageIndex = SAFEDB_ICON_INDEX;
                    }
                }

                if (n != null)
                {
                    n.SelectedImageIndex = n.ImageIndex;
                    node.Nodes.Add(n);
                }
                else
                {
                    MessageBox.Show("Not found: " + plcBlock.Name);
                }
            }

            // then add groups and search recursive
            foreach (PlcBlockGroup group in plcGroup.Groups)
            {
                TreeNode n = new TreeNode(group.Name);
                n.Tag = group;
                n.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                n.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;

                AddPlcBlocks(group, n);
                node.Nodes.Add(n);
            }
        }
        public void AddPlcTypes(PlcTypeGroup plcTypeGroup, TreeNode node)
        {
            foreach (PlcType ty in plcTypeGroup.Types)
            {
                TreeNode n = new TreeNode(ty.Name);
                n.Tag = ty;
                n.ImageIndex = DATATYPE_ICON_INDEX;
                n.SelectedImageIndex = n.ImageIndex;

                node.Nodes.Add(n);
            }

            // then add groups and search recursive
            foreach (PlcTypeGroup tGroup in plcTypeGroup.Groups)
            {
                TreeNode n = new TreeNode(tGroup.Name);
                n.Tag = tGroup;
                n.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                n.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;

                node.Nodes.Add(n);
                AddPlcTypes(tGroup, n);
            }
        }

        public List<PlcBlock> GetAllBlocks(PlcBlockGroup SearchGroup, List<PlcBlock> result)
        {
            foreach (PlcBlock block in SearchGroup.Blocks)
            {
                result.Add(block);
            }
            foreach (PlcBlockGroup group in SearchGroup.Groups)
            {
                result = GetAllBlocks(group, result);
            }
            return result;
        }
        public List<PlcType> GetAllDataTypes(PlcTypeGroup SearchGroup, List<PlcType> result)
        {
            foreach (PlcType ty in SearchGroup.Types)
            {
                result.Add(ty);
            }
            foreach (PlcTypeGroup group in SearchGroup.Groups)
            {
                result = GetAllDataTypes(group, result);
            }
            return result;
        }
        public List<string> GetAllBlocksNames(PlcBlockGroup SearchGroup, List<string> result)
        {
            foreach (PlcBlock block in SearchGroup.Blocks)
            {
                result.Add(block.Name);
            }
            foreach (PlcBlockGroup group in SearchGroup.Groups)
            {
                result = GetAllBlocksNames(group, result);
            }
            return result;
        }
        public List<string> GetAllDataTypesNames(PlcTypeGroup SearchGroup, List<string> result)
        {
            foreach (PlcType ty in SearchGroup.Types)
            {
                result.Add(ty.Name);
            }
            foreach (PlcTypeGroup group in SearchGroup.Groups)
            {
                result = GetAllDataTypesNames(group, result);
            }
            return result;
        }

        // we need to enumerate through the tree since there is no function in the API?
        public bool NameExists(string Name, PlcSoftware Software)
        {
            List<string> list = new List<string>();
            list = GetAllBlocksNames(Software.BlockGroup, list);
            if (list.Contains(Name)) return true;

            list = new List<string>();
            list = GetAllDataTypesNames(Software.TypeGroup, list);
            if (list.Contains(Name)) return true;

            return false;
        }

        public bool GroupExists(string Name, PlcBlockGroup Group)
        {
            foreach (PlcBlockGroup g in Group.Groups)
            {
                if (g.Name.ToLower() == Name.ToLower()) return true;
            }

            return false;
        }

        public bool GroupBlockExists(string Name, PlcBlockUserGroupComposition Group)
        {
            foreach (PlcBlockGroup g in Group)
            {
                if (g.Name.ToLower() == Name.ToLower()) return true;
            }

            return false;
        }

        public bool GroupTypeExists(string Name, PlcTypeUserGroupComposition Group)
        {
            foreach (PlcTypeUserGroup g in Group)
            {
                if (g.Name.ToLower() == Name.ToLower()) return true;
            }

            return false;
        }

        /// <summary>
        /// 加载软件单元组 (Software Units)
        /// </summary>
        /// <param name="software">PlcSoftware对象</param>
        /// <param name="root">根节点</param>
        private void LoadSoftwareUnitsGroup(PlcSoftware software, TreeNode root)
        {
            try
            {
                Logger.LogInfo("开始加载软件单元组", "LoadSoftwareUnits");

                // 通过GetService获取PlcUnitProvider
                PlcUnitProvider unitProvider = software.GetService<PlcUnitProvider>();
                if (unitProvider != null)
                {
                    PlcUnitSystemGroup unitSystemGroup = unitProvider.UnitGroup;
                    if (unitSystemGroup != null)
                    {
                        TreeNode unitsNode = new TreeNode($"Software Units ({GetUnitCount(unitSystemGroup)})");
                        unitsNode.Tag = unitSystemGroup;
                        unitsNode.ImageIndex = SOFTWARE_UNITS_ICON_INDEX;
                        unitsNode.SelectedImageIndex = SOFTWARE_UNITS_ICON_INDEX;

                        // 添加软件单元
                        AddSoftwareUnits(unitSystemGroup, unitsNode);

                        root.Nodes.Add(unitsNode);
                        Logger.LogInfo($"软件单元组加载完成，共 {GetUnitCount(unitSystemGroup)} 个单元", "LoadSoftwareUnits");
                    }
                    else
                    {
                        Logger.LogWarning("UnitGroup为空", "LoadSoftwareUnits");
                    }
                }
                else
                {
                    Logger.LogWarning("无法获取PlcUnitProvider服务", "LoadSoftwareUnits");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "加载软件单元组");
            }
        }

        /// <summary>
        /// 获取软件单元组中的单元数量
        /// </summary>
        private int GetUnitCount(PlcUnitSystemGroup unitSystemGroup)
        {
            int count = 0;
            try
            {
                count += unitSystemGroup.Units.Count;
                count += unitSystemGroup.SafetyUnits.Count;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "获取软件单元数量");
            }
            return count;
        }

        /// <summary>
        /// 添加软件单元到TreeView
        /// </summary>
        private void AddSoftwareUnits(PlcUnitSystemGroup unitSystemGroup, TreeNode node)
        {
            try
            {
                // 添加普通单元
                foreach (var unit in unitSystemGroup.Units)
                {
                    TreeNode unitNode = new TreeNode(unit.Name);
                    unitNode.Tag = unit;
                    unitNode.ImageIndex = SOFTWARE_UNITS_ICON_INDEX; // 使用软件单元图标索引
                    unitNode.SelectedImageIndex = SOFTWARE_UNITS_ICON_INDEX;
                    
                    // 递归解析软件单元的子组件
                    AddSoftwareUnitComponents(unit, unitNode);
                    
                    node.Nodes.Add(unitNode);
                }

                // 添加安全单元
                foreach (var safetyUnit in unitSystemGroup.SafetyUnits)
                {
                    TreeNode safetyUnitNode = new TreeNode($"{safetyUnit.Name} [Safety]");
                    safetyUnitNode.Tag = safetyUnit;
                    safetyUnitNode.ImageIndex = SOFTWARE_UNITS_ICON_INDEX; // 使用软件单元图标索引
                    safetyUnitNode.SelectedImageIndex = SOFTWARE_UNITS_ICON_INDEX;
                    
                    // 递归解析安全单元的子组件
                    AddSoftwareUnitComponents(safetyUnit, safetyUnitNode);
                    
                    node.Nodes.Add(safetyUnitNode);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "添加软件单元");
            }
        }

        /// <summary>
        /// 添加软件单元的子组件（递归解析）
        /// </summary>
        /// <param name="unit">软件单元（PlcUnit或PlcSafetyUnit）</param>
        /// <param name="unitNode">单元节点</param>
        private void AddSoftwareUnitComponents(object unit, TreeNode unitNode)
        {
            try
            {
                // 使用反射获取PlcUnitBase的属性
                var unitType = unit.GetType();
                
                // 添加块组 (Block Group)
                var blockGroupProperty = unitType.GetProperty("BlockGroup");
                if (blockGroupProperty != null)
                {
                    var blockGroup = blockGroupProperty.GetValue(unit);
                    if (blockGroup != null)
                    {
                        TreeNode blockGroupNode = new TreeNode("Block Group");
                        blockGroupNode.Tag = blockGroup;
                        blockGroupNode.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                        blockGroupNode.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;
                        
                        // 递归添加块组内容
                        AddPlcBlocks((dynamic)blockGroup, blockGroupNode);
                        
                        unitNode.Nodes.Add(blockGroupNode);
                    }
                }
                
                // 添加类型组 (Type Group)
                var typeGroupProperty = unitType.GetProperty("TypeGroup");
                if (typeGroupProperty != null)
                {
                    var typeGroup = typeGroupProperty.GetValue(unit);
                    if (typeGroup != null)
                    {
                        TreeNode typeGroupNode = new TreeNode("Type Group");
                        typeGroupNode.Tag = typeGroup;
                        typeGroupNode.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                        typeGroupNode.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;
                        
                        // 递归添加类型组内容
                        AddPlcTypes((dynamic)typeGroup, typeGroupNode);
                        
                        unitNode.Nodes.Add(typeGroupNode);
                    }
                }
                
                // 添加标签表组 (Tag Table Group)
                var tagTableGroupProperty = unitType.GetProperty("TagTableGroup");
                if (tagTableGroupProperty != null)
                {
                    var tagTableGroup = tagTableGroupProperty.GetValue(unit);
                    if (tagTableGroup != null)
                    {
                        TreeNode tagTableGroupNode = new TreeNode("Tag Table Group");
                        tagTableGroupNode.Tag = tagTableGroup;
                        tagTableGroupNode.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                        tagTableGroupNode.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;
                        
                        // 添加标签表组内容
                        AddTagTableGroup((dynamic)tagTableGroup, tagTableGroupNode);
                        
                        unitNode.Nodes.Add(tagTableGroupNode);
                    }
                }
                
                // 添加外部源组 (External Source Group)
                var externalSourceGroupProperty = unitType.GetProperty("ExternalSourceGroup");
                if (externalSourceGroupProperty != null)
                {
                    var externalSourceGroup = externalSourceGroupProperty.GetValue(unit);
                    if (externalSourceGroup != null)
                    {
                        TreeNode externalSourceGroupNode = new TreeNode("External Source Group");
                        externalSourceGroupNode.Tag = externalSourceGroup;
                        externalSourceGroupNode.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                        externalSourceGroupNode.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;
                        
                        // 添加外部源组内容
                        AddExternalSourceGroup((dynamic)externalSourceGroup, externalSourceGroupNode);
                        
                        unitNode.Nodes.Add(externalSourceGroupNode);
                    }
                }
                
                // 添加报警文本列表组 (Alarm Text List Group)
                var alarmTextListGroupProperty = unitType.GetProperty("PlcAlarmTextlistGroup");
                if (alarmTextListGroupProperty != null)
                {
                    var alarmTextListGroup = alarmTextListGroupProperty.GetValue(unit);
                    if (alarmTextListGroup != null)
                    {
                        TreeNode alarmTextListGroupNode = new TreeNode("Alarm Text List Group");
                        alarmTextListGroupNode.Tag = alarmTextListGroup;
                        alarmTextListGroupNode.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                        alarmTextListGroupNode.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;
                        
                        unitNode.Nodes.Add(alarmTextListGroupNode);
                    }
                }
                
                Logger.LogInfo($"软件单元 {GetUnitName(unit)} 的子组件加载完成", "AddSoftwareUnitComponents");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, $"添加软件单元子组件: {GetUnitName(unit)}");
            }
        }
        
        /// <summary>
        /// 获取软件单元名称
        /// </summary>
        private string GetUnitName(object unit)
        {
            try
            {
                var nameProperty = unit.GetType().GetProperty("Name");
                return nameProperty?.GetValue(unit)?.ToString() ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
        
        /// <summary>
        /// 添加标签表组内容
        /// </summary>
        private void AddTagTableGroup(dynamic tagTableGroup, TreeNode node)
        {
            try
            {
                // 添加标签表
                foreach (var tagTable in tagTableGroup.TagTables)
                {
                    TreeNode tagTableNode = new TreeNode(tagTable.Name);
                    tagTableNode.Tag = tagTable;
                    tagTableNode.ImageIndex = TAG_TABLE_ICON_INDEX;
                    tagTableNode.SelectedImageIndex = TAG_TABLE_ICON_INDEX;
                    node.Nodes.Add(tagTableNode);
                }
                
                // 递归添加子组
                foreach (var subGroup in tagTableGroup.Groups)
                {
                    TreeNode subGroupNode = new TreeNode(subGroup.Name);
                    subGroupNode.Tag = subGroup;
                    subGroupNode.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                    subGroupNode.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;
                    
                    AddTagTableGroup(subGroup, subGroupNode);
                    node.Nodes.Add(subGroupNode);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "添加标签表组");
            }
        }
        
        /// <summary>
        /// 添加外部源组内容
        /// </summary>
        private void AddExternalSourceGroup(dynamic externalSourceGroup, TreeNode node)
        {
            try
            {
                // 添加外部源
                foreach (var externalSource in externalSourceGroup.ExternalSources)
                {
                    TreeNode externalSourceNode = new TreeNode(externalSource.Name);
                    externalSourceNode.Tag = externalSource;
                    externalSourceNode.ImageIndex = SOFTWARE_UNITS_ICON_INDEX;
                    externalSourceNode.SelectedImageIndex = SOFTWARE_UNITS_ICON_INDEX;
                    node.Nodes.Add(externalSourceNode);
                }
                
                // 递归添加子组
                foreach (var subGroup in externalSourceGroup.Groups)
                {
                    TreeNode subGroupNode = new TreeNode(subGroup.Name);
                    subGroupNode.Tag = subGroup;
                    subGroupNode.ImageIndex = FOLDERCLOSED_ICON_INDEX;
                    subGroupNode.SelectedImageIndex = FOLDERCLOSED_ICON_INDEX;
                    
                    AddExternalSourceGroup(subGroup, subGroupNode);
                    node.Nodes.Add(subGroupNode);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "添加外部源组");
            }
        }
    }
}
