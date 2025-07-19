using System;
using System.Drawing;
using System.Windows.Forms;

namespace CodeGeneratorOpenness
{
    /// <summary>
    /// 启动界面窗体
    /// </summary>
    public partial class frmStartup : Form
    {
        public frmStartup()
        {
            InitializeComponent();
            this.Load += frmStartup_Load;
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        private void frmStartup_Load(object sender, EventArgs e)
        {
            try
            {
                Logger.LogInfo("启动画面加载，开始检查模板文件路径", "frmStartup.Load");
                
                // 检查模板文件路径 - 使用相对于bin\Debug目录的路径
                string templatePath = "template\\Automation_Framework_PROJ_V1_2\\Automation_Framework_PROJ_V1_2.ap19";
                CheckFilePathExists(templatePath);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "frmStartup.Load");
            }
        }

        /// <summary>
        /// 检查文件路径是否存在
        /// </summary>
        /// <param name="filePath">要检查的文件路径</param>
        /// <returns>文件是否存在</returns>
        private bool CheckFilePathExists(string filePath)
        {
            try
            {
                Logger.LogInfo($"开始检查文件路径: {filePath}", "CheckFilePathExists");
                
                // 将相对路径转换为绝对路径
                string absolutePath;
                if (System.IO.Path.IsPathRooted(filePath))
                {
                    absolutePath = filePath;
                }
                else
                {
                    absolutePath = System.IO.Path.Combine(Application.StartupPath, filePath);
                }
                
                Logger.LogInfo($"转换后的绝对路径: {absolutePath}", "CheckFilePathExists");
                
                bool exists = System.IO.File.Exists(absolutePath);
                
                if (exists)
                {
                    Logger.LogInfo($"文件路径存在: {absolutePath}", "CheckFilePathExists");
                }
                else
                {
                    Logger.LogWarning($"文件路径不存在: {absolutePath}", "CheckFilePathExists");
                    
                    // 弹窗告知用户文件不存在，确定后退出程序
                    MessageBox.Show($"模板文件不存在：\n{absolutePath}\n\n程序将退出。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                
                return exists;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "CheckFilePathExists");
                return false;
            }
        }

        private void InitializeComponent()
        {
            this.btnNewProject = new System.Windows.Forms.Button();
            this.btnOpenProject = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblTitle.Location = new System.Drawing.Point(80, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(240, 26);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Code Generator Openness";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Location = new System.Drawing.Point(150, 80);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(100, 100);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxLogo.TabIndex = 1;
            this.pictureBoxLogo.TabStop = false;
            
            // 尝试加载图标
            try
            {
                string iconPath = System.IO.Path.Combine(Application.StartupPath, "codeGenerator.ico");
                if (System.IO.File.Exists(iconPath))
                {
                    this.pictureBoxLogo.Image = new Bitmap(iconPath);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "frmStartup.InitializeComponent - Load Icon");
            }
            
            // 
            // btnNewProject
            // 
            this.btnNewProject.BackColor = System.Drawing.Color.LightBlue;
            this.btnNewProject.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewProject.Location = new System.Drawing.Point(80, 220);
            this.btnNewProject.Name = "btnNewProject";
            this.btnNewProject.Size = new System.Drawing.Size(120, 50);
            this.btnNewProject.TabIndex = 2;
            this.btnNewProject.Text = "新建项目";
            this.btnNewProject.UseVisualStyleBackColor = false;
            this.btnNewProject.Click += new System.EventHandler(this.btnNewProject_Click);
            
            // 
            // btnOpenProject
            // 
            this.btnOpenProject.BackColor = System.Drawing.Color.LightGreen;
            this.btnOpenProject.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenProject.Location = new System.Drawing.Point(220, 220);
            this.btnOpenProject.Name = "btnOpenProject";
            this.btnOpenProject.Size = new System.Drawing.Size(120, 50);
            this.btnOpenProject.TabIndex = 3;
            this.btnOpenProject.Text = "打开项目";
            this.btnOpenProject.UseVisualStyleBackColor = false;
            this.btnOpenProject.Click += new System.EventHandler(this.btnOpenProject_Click);
            
            // 
            // frmStartup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(400, 320);
            this.Controls.Add(this.btnOpenProject);
            this.Controls.Add(this.btnNewProject);
            this.Controls.Add(this.pictureBoxLogo);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmStartup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Code Generator Openness - 启动";
            
            // 设置窗体图标
            try
            {
                string iconPath = System.IO.Path.Combine(Application.StartupPath, "codeGenerator.ico");
                if (System.IO.File.Exists(iconPath))
                {
                    this.Icon = new Icon(iconPath);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "frmStartup.InitializeComponent - Set Icon");
            }
            
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button btnNewProject;
        private System.Windows.Forms.Button btnOpenProject;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.PictureBox pictureBoxLogo;

        /// <summary>
        /// 新建项目按钮点击事件
        /// </summary>
        private void btnNewProject_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.LogInfo("用户点击新建项目按钮", "frmStartup.btnNewProject_Click");
                
                CreateNewProject();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "frmStartup.btnNewProject_Click");
                MessageBox.Show($"新建项目时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 创建新项目的核心逻辑
        /// </summary>
        private void CreateNewProject()
        {
            try
            {
                // 弹出对话框输入项目名称
                string projectName = "";
                DialogResult result = Input.InputBox("新建项目", "请输入项目名称:", ref projectName);
                
                if (result != DialogResult.OK || string.IsNullOrWhiteSpace(projectName))
                {
                    Logger.LogInfo("用户取消了新建项目或未输入项目名称", "CreateNewProject");
                    return;
                }
                
                // 验证项目名称（移除非法字符）
                projectName = projectName.Trim();
                char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
                foreach (char c in invalidChars)
                {
                    projectName = projectName.Replace(c, '_');
                }
                
                Logger.LogInfo($"用户输入的项目名称: {projectName}", "CreateNewProject");
                
                // 检查模板文件是否存在
                string templatePath = System.IO.Path.Combine(Application.StartupPath, "template\\Automation_Framework_PROJ_V1_2\\Automation_Framework_PROJ_V1_2.ap19");
                if (!System.IO.File.Exists(templatePath))
                {
                    Logger.LogError($"模板文件不存在: {templatePath}", "CreateNewProject");
                    MessageBox.Show($"模板文件不存在：\n{templatePath}\n\n请确保模板文件存在后重试。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                // 确保project目录存在
                string projectDir = System.IO.Path.Combine(Application.StartupPath, "project");
                if (!System.IO.Directory.Exists(projectDir))
                {
                    System.IO.Directory.CreateDirectory(projectDir);
                    Logger.LogInfo($"创建project目录: {projectDir}", "CreateNewProject");
                }
                
                // 检查目标项目是否已存在
                string targetProjectDir = System.IO.Path.Combine(projectDir, projectName);
                if (System.IO.Directory.Exists(targetProjectDir))
                {
                    DialogResult overwriteResult = MessageBox.Show(
                        $"项目 '{projectName}' 已存在。是否覆盖？", 
                        "项目已存在", 
                        MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Question);
                    
                    if (overwriteResult != DialogResult.Yes)
                    {
                        Logger.LogInfo($"用户取消覆盖已存在的项目: {projectName}", "CreateNewProject");
                        return;
                    }
                    
                    // 删除已存在的项目目录
                    System.IO.Directory.Delete(targetProjectDir, true);
                    Logger.LogInfo($"删除已存在的项目目录: {targetProjectDir}", "CreateNewProject");
                }
                
                Logger.LogInfo($"开始创建新项目: {projectName}", "CreateNewProject");
                Logger.LogInfo($"模板路径: {templatePath}", "CreateNewProject");
                Logger.LogInfo($"目标路径: {targetProjectDir}", "CreateNewProject");
                
                // 使用TIA Portal Openness API创建新项目
                CreateProjectFromTemplate(templatePath, targetProjectDir, projectName);
                
                Logger.LogInfo($"新项目 '{projectName}' 创建成功", "CreateNewProject");
                
                // 询问用户是否打开新项目
                DialogResult openResult = MessageBox.Show(
                    $"项目 '{projectName}' 创建成功！\n\n是否现在打开该项目？", 
                    "创建成功", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Information);
                
                if (openResult == DialogResult.Yes)
                {
                    Logger.LogInfo($"用户选择打开新创建的项目: {projectName}", "CreateNewProject");
                    
                    // 隐藏启动窗体并打开主窗体
                    this.Hide();
                    frmMainForm mainForm = new frmMainForm();
                    mainForm.ShowDialog();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "CreateNewProject");
                MessageBox.Show($"创建新项目时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// 从模板创建新项目
        /// </summary>
        /// <param name="templatePath">模板文件路径</param>
        /// <param name="targetProjectDir">目标项目目录</param>
        /// <param name="projectName">项目名称</param>
        private void CreateProjectFromTemplate(string templatePath, string targetProjectDir, string projectName)
        {
            try
            {
                Logger.LogInfo("开始初始化TIA Portal", "CreateProjectFromTemplate");
                
                // 确保目标项目目录存在
                if (!System.IO.Directory.Exists(targetProjectDir))
                {
                    System.IO.Directory.CreateDirectory(targetProjectDir);
                    Logger.LogInfo($"创建目标项目目录: {targetProjectDir}", "CreateProjectFromTemplate");
                }
                
                // 初始化TIA Portal
                using (var tiaPortal = new Siemens.Engineering.TiaPortal(Siemens.Engineering.TiaPortalMode.WithoutUserInterface))
                {
                    Logger.LogInfo($"打开模板项目: {templatePath}", "CreateProjectFromTemplate");
                    
                    // 打开模板项目
                    var templateProject = tiaPortal.Projects.Open(new System.IO.FileInfo(templatePath));
                    
                    Logger.LogInfo($"开始另存为新项目到: {targetProjectDir}", "CreateProjectFromTemplate");
                    
                    // 另存为新项目到目标目录
                    templateProject.SaveAs(new System.IO.DirectoryInfo(targetProjectDir));
                    
                    // 关闭模板项目
                    templateProject.Close();
                    
                    Logger.LogInfo("模板项目已关闭", "CreateProjectFromTemplate");
                }
                
                Logger.LogInfo("TIA Portal已释放", "CreateProjectFromTemplate");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "CreateProjectFromTemplate");
                throw new Exception($"从模板创建项目失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 打开项目按钮点击事件
        /// </summary>
        private void btnOpenProject_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.LogInfo("用户点击打开项目按钮，准备跳转到主界面", "frmStartup.btnOpenProject_Click");
                
                // 隐藏启动窗体
                this.Hide();
                
                // 打开主窗体
                frmMainForm mainForm = new frmMainForm();
                mainForm.ShowDialog();
                
                // 主窗体关闭后，关闭启动窗体
                this.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "frmStartup.btnOpenProject_Click");
                MessageBox.Show($"打开主界面时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show(); // 发生错误时重新显示启动窗体
            }
        }
    }
}