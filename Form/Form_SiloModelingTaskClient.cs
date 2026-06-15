using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 筒仓建模任务客户端窗体
    /// </summary>
    public partial class Form_SiloModelingTaskClient : System.Windows.Forms.Form
    {
        private readonly RevitModelingExternalEventHandler _handler;
        private readonly ExternalEvent _externalEvent;
        private readonly TemplateSiloSaveExternalEventHandler _templateSiloHandler;
        private readonly ExternalEvent _templateSiloEvent;
        private SiloTaskRepository _repository;
        private TemplateSiloApiClient _rfaResourceApiClient;
        private List<ModelingTask> _pendingTasks = new List<ModelingTask>();
        private bool _isAdmin;

        /// <summary>
        /// 初始化筒仓建模任务客户端窗体
        /// </summary>
        /// <param name="handler">建模任务ExternalEvent处理器</param>
        /// <param name="externalEvent">建模任务ExternalEvent</param>
        /// <param name="templateSiloHandler">库型模板保存ExternalEvent处理器</param>
        /// <param name="templateSiloEvent">库型模板保存ExternalEvent</param>
        public Form_SiloModelingTaskClient(
            RevitModelingExternalEventHandler handler,
            ExternalEvent externalEvent,
            TemplateSiloSaveExternalEventHandler templateSiloHandler,
            ExternalEvent templateSiloEvent)
        {
            _handler = handler;
            _externalEvent = externalEvent;
            _templateSiloHandler = templateSiloHandler;
            _templateSiloEvent = templateSiloEvent;
            InitializeComponent();
            comboBox_DictSilo.Enabled = false;
            UpdateSaveTemplateButtonState();
        }

        /// <summary>
        /// 窗体加载时初始化标题和日志
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件参数</param>
        private void Form_SiloModelingTaskClient_Load(object sender, EventArgs e)
        {
            Text = "筒仓建模插件";
            AppendLog("插件已打开。点击“获取新任务”读取新建建模任务，点击“执行建模”执行当前任务列表。");
            LoadIsAdmin();
            if (_isAdmin)
            {
                LoadDictSiloOptions();
            }
        }

        /// <summary>
        /// 加载当前用户管理员状态。
        /// </summary>
        private void LoadIsAdmin()
        {
            _isAdmin = FunHttp.IsAdmin();
            comboBox_DictSilo.Enabled = _isAdmin;
            UpdateSaveTemplateButtonState();
            AppendLog(_isAdmin ? "当前用户是管理员。" : "当前用户不是管理员。");
        }

        /// <summary>
        /// 点击获取新任务按钮后读取新建建模任务
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件参数</param>
        private void button_GetNewTasks_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureRuntime();
                _pendingTasks = _repository.GetNewTasks(Config.NewTaskStatus);

                AppendLog("获取新任务完成，数量：" + _pendingTasks.Count);
                foreach (ModelingTask task in _pendingTasks)
                {
                    AppendLog("新建任务：标题：" + task.TaskTitle + "，库型：" + GetTaskSiloTypeText(task));
                }
            }
            catch (Exception ex)
            {
                AppendLog("获取新任务失败：" + ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 点击执行建模按钮后提交当前任务列表到Revit外部事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件参数</param>
        private void button_ExecuteModeling_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureRuntime();
                if (_pendingTasks.Count == 0)
                {
                    AppendLog("当前没有可执行的新建任务，请先点击“获取新任务”。");
                    return;
                }

                foreach (ModelingTask task in _pendingTasks)
                {
                    _handler.SetTask(task);
                    AppendLog("已提交建模任务：标题：" + task.TaskTitle + "，库型：" + GetTaskSiloTypeText(task));
                }

                _externalEvent.Raise();
            }
            catch (Exception ex)
            {
                AppendLog("执行建模启动失败：" + ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 点击保存族资源按钮后保存当前三维视图中的库型模板
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件参数</param>
        private void button_SaveRfaResource_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureRfaResourceRuntime();
                Guid dictSiloId = (Guid)comboBox_DictSilo.SelectedValue;
                string dictSiloName = comboBox_DictSilo.Text;
                _templateSiloHandler.SetExecutor(new TemplateSiloSaveExecutor(_rfaResourceApiClient, dictSiloId, dictSiloName));
                _templateSiloHandler.SetLog(AppendLog);
                _templateSiloHandler.Request();
                _templateSiloEvent.Raise();
                AppendLog("已请求保存当前三维视图中的族资源，库型：" + dictSiloName);
            }
            catch (Exception ex)
            {
                AppendLog("保存族资源启动失败：" + ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 加载库型下拉选项。
        /// </summary>
        private void LoadDictSiloOptions()
        {
            try
            {
                EnsureRfaResourceRuntime();
                List<SelectOption<Guid>> options = _rfaResourceApiClient.GetDictSiloOptions();
                comboBox_DictSilo.DisplayMember = "Label";
                comboBox_DictSilo.ValueMember = "Value";
                comboBox_DictSilo.DataSource = options;
                UpdateSaveTemplateButtonState();
                AppendLog("库型选项已加载，数量：" + options.Count);
            }
            catch (Exception ex)
            {
                AppendLog("加载库型选项失败：" + ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 确保RFA资源接口客户端已初始化。
        /// </summary>
        private void EnsureRfaResourceRuntime()
        {
            if (_rfaResourceApiClient == null)
            {
                _rfaResourceApiClient = new TemplateSiloApiClient();
            }
        }

        /// <summary>
        /// 确保运行时配置、后端仓储和建模执行器已初始化
        /// </summary>
        private void EnsureRuntime()
        {
            if (_repository == null)
            {
                _repository = new SiloTaskRepository();
            }

            _handler.SetExecutor(new ModelingTaskExecutor(_repository, Config.ModelingDoneStatus));
            _handler.SetLog(AppendLog);
        }

        /// <summary>
        /// 获取建模任务对应的库型显示文本
        /// </summary>
        /// <param name="task">建模任务</param>
        /// <returns>库型显示文本</returns>
        private string GetTaskSiloTypeText(ModelingTask task)
        {
            DictSiloRecord dictSilo = _repository.GetDictSilo(task.DictSiloId);
            return dictSilo.SiloType;
        }

        /// <summary>
        /// 库型下拉框选项变化时刷新保存按钮状态
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件参数</param>
        private void comboBox_DictSilo_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSaveTemplateButtonState();
        }

        /// <summary>
        /// 根据库型是否已选择更新保存族资源按钮启用状态
        /// </summary>
        private void UpdateSaveTemplateButtonState()
        {
            button_SaveRfaResource.Enabled = _isAdmin && comboBox_DictSilo.SelectedValue is Guid;
        }

        /// <summary>
        /// 向窗体日志文本框追加日志
        /// </summary>
        /// <param name="message">日志内容</param>
        private void AppendLog(string message)
        {
            if (textBox_Log.InvokeRequired)
            {
                textBox_Log.BeginInvoke(new Action<string>(AppendLog), message);
                return;
            }

            textBox_Log.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + message + Environment.NewLine);
        }
    }
}
