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

namespace CodeGeneratorOpenness
{
    class cFunctionGroups
    {
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

                try
                {
                    Logger.LogInfo("开始加载程序块", "LoadTreeView");
                    // Add Program Blocks
                    TreeNode programBlocks = new TreeNode("Program Blocks");
                    programBlocks.Tag = Software.BlockGroup;
                    root.Nodes.Add(programBlocks);

                    AddPlcBlocks(Software.BlockGroup, programBlocks);
                    programBlocks.Expand();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex, "加载程序块");
                    MessageBox.Show(String.Format("加载程序块时发生错误:\n{0}", ex.Message), 
                                  "程序块加载错误", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Warning);
                }

                try
                {
                    Logger.LogInfo("开始加载数据类型", "LoadTreeView");
                    // add data types
                    TreeNode dataTypes = new TreeNode("PLC Data types");
                    dataTypes.Tag = Software.TypeGroup;
                    root.Nodes.Add(dataTypes);

                    AddPlcTypes(Software.TypeGroup, dataTypes);
                    dataTypes.Expand();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex, "加载数据类型");
                    MessageBox.Show(String.Format("加载数据类型时发生错误:\n{0}", ex.Message), 
                                  "数据类型加载错误", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Warning);
                }

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
                    n.ImageIndex = 2;

                    OB ob = (OB)plcBlock;
                    if (ob.SecondaryType.Contains("Safe"))
                    {
                        n.BackColor = Color.Yellow;
                        n.ImageIndex = 7;
                    }
                }
                else if (plcBlock is FB)
                {
                    n = new TreeNode(plcBlock.Name + " [FB" + plcBlock.Number.ToString() + "]");
                    n.Tag = plcBlock;
                    n.ImageIndex = 3;

                    FB fb = (FB)plcBlock;
                    if ((fb.ProgrammingLanguage == ProgrammingLanguage.F_LAD) ||
                        (fb.ProgrammingLanguage == ProgrammingLanguage.F_FBD))
                    {
                        n.BackColor = Color.Yellow;
                        n.ImageIndex = 8;
                    }
                }
                else if (plcBlock is FC)
                {
                    n = new TreeNode(plcBlock.Name + " [FC" + plcBlock.Number.ToString() + "]");
                    n.Tag = plcBlock;
                    n.ImageIndex = 4;
                }
                else if (plcBlock is InstanceDB)
                {
                    n = new TreeNode(plcBlock.Name + " [DB" + plcBlock.Number.ToString() + "]");
                    n.Tag = plcBlock;
                    n.ImageIndex = 5;

                    InstanceDB db = (InstanceDB)plcBlock;
                    n.Name = db.Name + "[DB" + db.Number.ToString() + "]";
                    if (db.ProgrammingLanguage == ProgrammingLanguage.F_DB)
                    {
                        n.BackColor = Color.Yellow;
                        n.ImageIndex = 6;
                    }
                }
                else if (plcBlock is GlobalDB)
                { 
                    n = new TreeNode(plcBlock.Name + " [DB" + plcBlock.Number.ToString() + "]");
                    n.Tag = plcBlock;
                    n.ImageIndex = 5;

                    GlobalDB db = (GlobalDB)plcBlock;
                    n.Name = db.Name + "[FB" + db.Number.ToString() + "]";
                    if (db.ProgrammingLanguage == ProgrammingLanguage.F_DB)
                    {
                        n.BackColor = Color.Yellow;
                        n.ImageIndex = 6;
                    }
                }
                else if (plcBlock is ArrayDB)
                {
                    n = new TreeNode(plcBlock.Name + " [DB" + plcBlock.Number.ToString() + "]");
                    n.Tag = plcBlock;
                    n.ImageIndex = 5;

                    ArrayDB db = (ArrayDB)plcBlock;
                    n.Name = db.Name + "[FB" + db.Number.ToString() + "]";
                    if (db.ProgrammingLanguage == ProgrammingLanguage.F_DB)
                    {
                        n.BackColor = Color.Yellow;
                        n.ImageIndex = 6;
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
                n.ImageIndex = 1;
                n.SelectedImageIndex = 1;

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
                n.ImageIndex = 9;
                n.SelectedImageIndex = n.ImageIndex;

                node.Nodes.Add(n);
            }

            // then add groups and search recursive
            foreach (PlcTypeGroup tGroup in plcTypeGroup.Groups)
            {
                TreeNode n = new TreeNode(tGroup.Name);
                n.Tag = tGroup;
                n.ImageIndex = 1;
                n.SelectedImageIndex = 1;

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
    }
}
