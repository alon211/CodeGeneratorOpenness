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
                
                // 暂时不做任何处理，只记录日志
                MessageBox.Show("新建项目功能暂未实现", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "frmStartup.btnNewProject_Click");
                MessageBox.Show($"新建项目时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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