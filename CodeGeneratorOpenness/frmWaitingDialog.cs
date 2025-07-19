using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace CodeGeneratorOpenness
{
    /// <summary>
    /// 通用等待对话框，用于显示长时间操作的等待界面
    /// </summary>
    public partial class frmWaitingDialog : Form
    {
        private string _mainText;
        private string _waitText;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mainText">主要显示文本</param>
        /// <param name="waitText">等待提示文本，默认为"Please wait..."</param>
        public frmWaitingDialog(string mainText, string waitText = "Please wait...")
        {
            _mainText = mainText;
            _waitText = waitText;
            InitializeComponent();
            SetupLabels();
        }
        
        /// <summary>
        /// 设置标签文本
        /// </summary>
        private void SetupLabels()
        {
            label1.Text = _mainText;
            label2.Text = _waitText;
            
            // 根据文本长度调整标签位置，使其居中
            CenterLabels();
        }
        
        /// <summary>
        /// 居中显示标签
        /// </summary>
        private void CenterLabels()
        {
            // 计算label1的居中位置
            int label1X = (this.ClientSize.Width - label1.Width) / 2;
            label1.Location = new Point(label1X, label1.Location.Y);
            
            // 计算label2的居中位置
            int label2X = (this.ClientSize.Width - label2.Width) / 2;
            label2.Location = new Point(label2X, label2.Location.Y);
        }
        
        /// <summary>
        /// 显示等待对话框并执行异步操作
        /// </summary>
        /// <param name="operation">要执行的异步操作</param>
        /// <param name="mainText">主要显示文本</param>
        /// <param name="waitText">等待提示文本</param>
        /// <returns>操作结果</returns>
        public static async Task<T> ShowAndExecuteAsync<T>(Func<Task<T>> operation, string mainText, string waitText = "Please wait...")
        {
            frmWaitingDialog waitingDialog = null;
            T result = default(T);
            
            try
            {
                // 在UI线程中创建和显示等待对话框
                waitingDialog = new frmWaitingDialog(mainText, waitText);
                
                // 异步显示对话框
                Task showTask = Task.Run(() =>
                {
                    if (waitingDialog.InvokeRequired)
                    {
                        waitingDialog.Invoke(new Action(() => waitingDialog.ShowDialog()));
                    }
                    else
                    {
                        waitingDialog.ShowDialog();
                    }
                });
                
                // 等待一小段时间确保对话框显示
                await Task.Delay(100);
                
                // 执行实际操作
                result = await operation();
                
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError($"等待对话框执行异步操作时发生错误: {ex.Message}", "frmWaitingDialog");
                throw;
            }
            finally
            {
                // 关闭等待对话框
                if (waitingDialog != null)
                {
                    if (waitingDialog.InvokeRequired)
                    {
                        waitingDialog.Invoke(new Action(() =>
                        {
                            if (!waitingDialog.IsDisposed)
                            {
                                waitingDialog.Close();
                                waitingDialog.Dispose();
                            }
                        }));
                    }
                    else
                    {
                        if (!waitingDialog.IsDisposed)
                        {
                            waitingDialog.Close();
                            waitingDialog.Dispose();
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// 显示等待对话框并执行同步操作
        /// </summary>
        /// <param name="operation">要执行的操作</param>
        /// <param name="mainText">主要显示文本</param>
        /// <param name="waitText">等待提示文本</param>
        public static void ShowAndExecute(Action operation, string mainText, string waitText = "Please wait...")
        {
            frmWaitingDialog waitingDialog = null;
            
            try
            {
                waitingDialog = new frmWaitingDialog(mainText, waitText);
                
                // 使用BackgroundWorker执行操作
                using (BackgroundWorker worker = new BackgroundWorker())
                {
                    worker.DoWork += (sender, e) =>
                    {
                        try
                        {
                            operation();
                        }
                        catch (Exception ex)
                        {
                            e.Result = ex;
                        }
                    };
                    
                    worker.RunWorkerCompleted += (sender, e) =>
                    {
                        if (waitingDialog != null && !waitingDialog.IsDisposed)
                        {
                            waitingDialog.Close();
                        }
                        
                        if (e.Result is Exception ex)
                        {
                            throw ex;
                        }
                    };
                    
                    worker.RunWorkerAsync();
                    waitingDialog.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"等待对话框执行操作时发生错误: {ex.Message}", "frmWaitingDialog");
                if (waitingDialog != null && !waitingDialog.IsDisposed)
                {
                    waitingDialog.Close();
                }
                throw;
            }
        }
        
        /// <summary>
        /// 更新等待文本
        /// </summary>
        /// <param name="newWaitText">新的等待文本</param>
        public void UpdateWaitText(string newWaitText)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateWaitText), newWaitText);
                return;
            }
            
            _waitText = newWaitText;
            label2.Text = _waitText;
            CenterLabels();
        }
        
        /// <summary>
        /// 更新主文本
        /// </summary>
        /// <param name="newMainText">新的主文本</param>
        public void UpdateMainText(string newMainText)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateMainText), newMainText);
                return;
            }
            
            _mainText = newMainText;
            label1.Text = _mainText;
            CenterLabels();
        }
    }
}