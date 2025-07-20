using System;
using System.Drawing;
using System.Windows.Forms;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.Types;
using Siemens.Engineering.SW.Units;
using Siemens.Engineering.Library;
using Siemens.Engineering.Library.MasterCopies;
using Siemens.Engineering.Library.Types;
using Siemens.Engineering;

namespace CodeGeneratorOpenness
{
    /// <summary>
    /// 通用右键菜单组件
    /// 提供模块化的右键菜单功能，支持复制粘贴等操作
    /// </summary>
    public class cContextMenu
    {
        private ContextMenuStrip contextMenu;
        private TreeView targetTreeView;
        private object copiedObject;
        private TreeNode copiedNode;
        
        // 菜单项
        private ToolStripMenuItem copyMenuItem;
        private ToolStripMenuItem pasteMenuItem;
        private ToolStripSeparator separator;
        
        public cContextMenu()
        {
            InitializeContextMenu();
        }
        
        /// <summary>
        /// 初始化右键菜单
        /// </summary>
        private void InitializeContextMenu()
        {
            try
            {
                Logger.LogInfo("开始初始化通用右键菜单", "cContextMenu.InitializeContextMenu");
                
                contextMenu = new ContextMenuStrip();
                
                // 创建复制菜单项
                copyMenuItem = new ToolStripMenuItem("复制");
                copyMenuItem.Click += CopyMenuItem_Click;
                copyMenuItem.Image = CreateMenuIcon(Color.Blue); // 蓝色图标
                
                // 创建分隔符
                separator = new ToolStripSeparator();
                
                // 创建粘贴菜单项
                pasteMenuItem = new ToolStripMenuItem("粘贴");
                pasteMenuItem.Click += PasteMenuItem_Click;
                pasteMenuItem.Image = CreateMenuIcon(Color.Green); // 绿色图标
                pasteMenuItem.Enabled = false; // 初始状态禁用
                
                // 添加到菜单
                contextMenu.Items.Add(copyMenuItem);
                contextMenu.Items.Add(separator);
                contextMenu.Items.Add(pasteMenuItem);
                
                // 设置菜单样式
                contextMenu.BackColor = SystemColors.Menu;
                contextMenu.ForeColor = SystemColors.MenuText;
                
                Logger.LogInfo("通用右键菜单初始化完成", "cContextMenu.InitializeContextMenu");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "初始化通用右键菜单");
            }
        }
        
        /// <summary>
        /// 创建菜单图标
        /// </summary>
        /// <param name="color">图标颜色</param>
        /// <returns>图标图像</returns>
        private Image CreateMenuIcon(Color color)
        {
            try
            {
                Bitmap icon = new Bitmap(16, 16);
                using (Graphics g = Graphics.FromImage(icon))
                {
                    g.Clear(Color.Transparent);
                    using (SolidBrush brush = new SolidBrush(color))
                    {
                        g.FillRectangle(brush, 2, 2, 12, 12);
                    }
                    using (Pen pen = new Pen(Color.Black, 1))
                    {
                        g.DrawRectangle(pen, 2, 2, 12, 12);
                    }
                }
                return icon;
            }
            catch (Exception ex)
            {
                Logger.LogError($"创建菜单图标失败: {ex.Message}", "cContextMenu.CreateMenuIcon");
                return null;
            }
        }
        
        /// <summary>
        /// 初始化并绑定到TreeView
        /// </summary>
        /// <param name="treeView">目标TreeView控件</param>
        public void Initialize(TreeView treeView)
        {
            try
            {
                if (treeView == null)
                {
                    Logger.LogWarning("TreeView为空，无法初始化右键菜单", "cContextMenu.Initialize");
                    return;
                }
                
                targetTreeView = treeView;
                
                // 绑定鼠标右键事件
                treeView.MouseDown += TreeView_MouseDown;
                
                Logger.LogInfo("右键菜单已绑定到TreeView", "cContextMenu.Initialize");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "初始化右键菜单绑定");
            }
        }
        
        /// <summary>
        /// TreeView鼠标按下事件处理
        /// </summary>
        private void TreeView_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    TreeView treeView = sender as TreeView;
                    if (treeView != null)
                    {
                        TreeNode clickedNode = treeView.GetNodeAt(e.X, e.Y);
                        if (clickedNode != null)
                        {
                            treeView.SelectedNode = clickedNode;
                            ShowContextMenu(treeView, e.Location, clickedNode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "TreeView鼠标事件处理");
            }
        }
        
        /// <summary>
        /// 显示右键菜单
        /// </summary>
        /// <param name="treeView">目标TreeView</param>
        /// <param name="location">显示位置</param>
        /// <param name="selectedNode">选中的节点</param>
        public void ShowContextMenu(TreeView treeView, Point location, TreeNode selectedNode)
        {
            try
            {
                if (treeView == null || selectedNode == null)
                {
                    Logger.LogWarning("TreeView或选中节点为空，无法显示右键菜单", "cContextMenu.ShowContextMenu");
                    return;
                }
                
                targetTreeView = treeView;
                
                // 更新菜单项状态
                UpdateMenuItemStates(selectedNode);
                
                // 显示菜单
                contextMenu.Show(treeView, location);
                
                Logger.LogInfo($"显示右键菜单，节点: {selectedNode.Text}", "cContextMenu.ShowContextMenu");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "显示右键菜单");
            }
        }
        
        /// <summary>
        /// 更新菜单项状态
        /// </summary>
        /// <param name="selectedNode">选中的节点</param>
        private void UpdateMenuItemStates(TreeNode selectedNode)
        {
            try
            {
                // 复制菜单项始终可用
                copyMenuItem.Enabled = true;
                
                // 粘贴菜单项根据是否有复制的对象来决定
                pasteMenuItem.Enabled = (copiedObject != null);
                
                // 根据节点类型更新菜单项文本
                if (selectedNode.Tag is PlcBlock)
                {
                    copyMenuItem.Text = "复制块";
                    pasteMenuItem.Text = "粘贴块";
                }
                else if (selectedNode.Tag is PlcStruct)
                {
                    copyMenuItem.Text = "复制数据类型";
                    pasteMenuItem.Text = "粘贴数据类型";
                }
                else if (selectedNode.Tag is PlcBlockGroup)
                {
                    copyMenuItem.Text = "复制组";
                    pasteMenuItem.Text = "粘贴到组";
                }
                else
                {
                    copyMenuItem.Text = "复制";
                    pasteMenuItem.Text = "粘贴";
                }
                
                // 高亮选中的菜单项
                copyMenuItem.BackColor = SystemColors.Highlight;
                copyMenuItem.ForeColor = SystemColors.HighlightText;
                
                if (pasteMenuItem.Enabled)
                {
                    pasteMenuItem.BackColor = SystemColors.Highlight;
                    pasteMenuItem.ForeColor = SystemColors.HighlightText;
                }
                else
                {
                    pasteMenuItem.BackColor = SystemColors.Menu;
                    pasteMenuItem.ForeColor = SystemColors.GrayText;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"更新菜单项状态失败: {ex.Message}", "cContextMenu.UpdateMenuItemStates");
            }
        }
        
        /// <summary>
        /// 复制菜单项点击事件
        /// </summary>
        private void CopyMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (targetTreeView?.SelectedNode == null)
                {
                    Logger.LogWarning("没有选中的节点，无法执行复制操作", "cContextMenu.CopyMenuItem_Click");
                    return;
                }
                
                TreeNode selectedNode = targetTreeView.SelectedNode;
                copiedObject = selectedNode.Tag;
                copiedNode = selectedNode;
                
                // 启用粘贴菜单项
                pasteMenuItem.Enabled = true;
                
                Logger.LogInfo($"复制对象: {selectedNode.Text}, 类型: {copiedObject?.GetType().Name}", "cContextMenu.CopyMenuItem_Click");
                
                // 显示复制成功的提示（可选）
                // MessageBox.Show($"已复制: {selectedNode.Text}", "复制成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "复制操作");
                MessageBox.Show($"复制失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// 粘贴菜单项点击事件
        /// </summary>
        private void PasteMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (copiedObject == null)
                {
                    Logger.LogWarning("没有复制的对象，无法执行粘贴操作", "cContextMenu.PasteMenuItem_Click");
                    return;
                }
                
                if (targetTreeView?.SelectedNode == null)
                {
                    Logger.LogWarning("没有选中的节点，无法执行粘贴操作", "cContextMenu.PasteMenuItem_Click");
                    return;
                }
                
                TreeNode targetNode = targetTreeView.SelectedNode;
                
                Logger.LogInfo($"开始执行粘贴操作: {copiedNode?.Text} 到 {targetNode.Text}", "cContextMenu.PasteMenuItem_Click");
                
                // 检查是否为软件单元复制粘贴
                if (copiedObject is PlcUnit && IsSoftwareUnitsNode(targetNode))
                {
                    bool result = ExecuteSoftwareUnitPaste(copiedObject as PlcUnit, targetNode);
                    if (result)
                    {
                        Logger.LogInfo($"软件单元粘贴成功: {copiedNode?.Text}", "cContextMenu.PasteMenuItem_Click");
                        MessageBox.Show($"软件单元复制成功: {copiedNode?.Text}", "复制成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        Logger.LogWarning($"软件单元粘贴失败: {copiedNode?.Text}", "cContextMenu.PasteMenuItem_Click");
                        MessageBox.Show($"软件单元复制失败: {copiedNode?.Text}", "复制失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    // 其他类型的复制粘贴逻辑
                    Logger.LogInfo($"执行通用粘贴操作: {copiedNode?.Text} -> {targetNode.Text}", "cContextMenu.PasteMenuItem_Click");
                    MessageBox.Show($"粘贴操作: {copiedNode?.Text} -> {targetNode.Text}\n\n注意：当前仅支持软件单元复制到Software Units节点。", 
                        "粘贴操作", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "粘贴操作");
                MessageBox.Show($"粘贴失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// 检查目标节点是否为Software Units节点
        /// </summary>
        /// <param name="targetNode">目标节点</param>
        /// <returns>是否为Software Units节点</returns>
        private bool IsSoftwareUnitsNode(TreeNode targetNode)
        {
            try
            {
                // 检查节点文本是否包含"Software Units"
                if (targetNode.Text.Contains("Software Units"))
                {
                    return true;
                }
                
                // 检查节点的Tag是否为PlcUnitSystemGroup类型
                if (targetNode.Tag is PlcUnitSystemGroup)
                {
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogError($"检查Software Units节点失败: {ex.Message}", "cContextMenu.IsSoftwareUnitsNode");
                return false;
            }
        }
        
        /// <summary>
        /// 执行软件单元复制粘贴操作
        /// </summary>
        /// <param name="sourceUnit">源软件单元</param>
        /// <param name="targetNode">目标节点</param>
        /// <returns>复制是否成功</returns>
        private bool ExecuteSoftwareUnitPaste(PlcUnit sourceUnit, TreeNode targetNode)
        {
            try
            {
                Logger.LogInfo($"准备执行软件单元复制: 源对象类型={sourceUnit.GetType().Name}, 源节点={sourceUnit.Name}", "cContextMenu.ExecuteSoftwareUnitPaste");
                
                // 获取项目和软件对象
                if (frmMainForm.project == null || frmMainForm.software == null)
                {
                    Logger.LogError("项目或软件对象为空，无法执行复制操作", "cContextMenu.ExecuteSoftwareUnitPaste");
                    return false;
                }
                
                // 使用TiaPortalOpennessManager封装的方法获取PlcUnitProvider
                PlcUnitProvider unitProvider = TiaPortalOpennessManager.GetPlcUnitProvider(frmMainForm.software);
                if (unitProvider == null)
                {
                    Logger.LogError("无法获取PlcUnitProvider", "cContextMenu.ExecuteSoftwareUnitPaste");
                    return false;
                }
                
                // 使用TiaPortalOpennessManager封装的方法获取项目库
                ProjectLibrary projectLibrary = TiaPortalOpennessManager.GetProjectLibrary(frmMainForm.project);
                if (projectLibrary == null)
                {
                    Logger.LogError("无法获取项目库", "cContextMenu.ExecuteSoftwareUnitPaste");
                    return false;
                }
                
                // 使用TiaPortalOpennessManager封装的方法获取软件单元组合
                PlcUnitComposition targetComposition = TiaPortalOpennessManager.GetPlcUnitComposition(unitProvider);
                if (targetComposition == null)
                {
                    Logger.LogError("无法获取软件单元组合", "cContextMenu.ExecuteSoftwareUnitPaste");
                    return false;
                }
                
                // 使用TiaPortalOpennessManager封装的方法执行软件单元复制
                PlcUnit newUnit = TiaPortalOpennessManager.CreateSoftwareUnitFromMasterCopy(sourceUnit, targetComposition, projectLibrary);
                
                Logger.LogInfo($"软件单元复制成功: {newUnit.Name}", "cContextMenu.ExecuteSoftwareUnitPaste");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "执行软件单元复制");
                return false;
            }
        }
        
        /// <summary>
        /// 清除复制的对象
        /// </summary>
        public void ClearCopiedObject()
        {
            copiedObject = null;
            copiedNode = null;
            if (pasteMenuItem != null)
            {
                pasteMenuItem.Enabled = false;
            }
            Logger.LogInfo("清除复制的对象", "cContextMenu.ClearCopiedObject");
        }
        
        /// <summary>
        /// 获取当前复制的对象
        /// </summary>
        public object GetCopiedObject()
        {
            return copiedObject;
        }
        
        /// <summary>
        /// 获取当前复制的节点
        /// </summary>
        public TreeNode GetCopiedNode()
        {
            return copiedNode;
        }
        
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            try
            {
                contextMenu?.Dispose();
                copyMenuItem?.Dispose();
                pasteMenuItem?.Dispose();
                separator?.Dispose();
                
                Logger.LogInfo("通用右键菜单资源释放完成", "cContextMenu.Dispose");
            }
            catch (Exception ex)
            {
                Logger.LogError($"释放右键菜单资源失败: {ex.Message}", "cContextMenu.Dispose");
            }
        }
    }
}