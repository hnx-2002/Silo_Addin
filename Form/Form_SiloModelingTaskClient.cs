using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SiloModelingTaskClient
{
    public partial class Form_SiloModelingTaskClient : System.Windows.Forms.Form
    {
        private readonly RevitModelingExternalEventHandler _handler;
        private readonly ExternalEvent _externalEvent;
        private readonly RfaResourceSaveExternalEventHandler _rfaResourceHandler;
        private readonly ExternalEvent _rfaResourceEvent;
        private PluginConfig _config;
        private SiloTaskRepository _repository;
        private List<ModelingTask> _pendingTasks = new List<ModelingTask>();

        /// <summary>
        /// 初始化筒仓建模任务客户端窗口
        /// </summary>
        /// <param name="handler">建模任务ExternalEvent处理器</param>
        /// <param name="externalEvent">建模任务ExternalEvent</param>
        /// <param name="rfaResourceHandler">族资源保存ExternalEvent处理器</param>
        /// <param name="rfaResourceEvent">族资源保存ExternalEvent</param>
        public Form_SiloModelingTaskClient(
            RevitModelingExternalEventHandler handler,
            ExternalEvent externalEvent,
            RfaResourceSaveExternalEventHandler rfaResourceHandler,
            ExternalEvent rfaResourceEvent)
        {
            _handler = handler;
            _externalEvent = externalEvent;
            _rfaResourceHandler = rfaResourceHandler;
            _rfaResourceEvent = rfaResourceEvent;
            InitializeComponent();
        }

        /// <summary>
        /// 窗口加载时初始化标题和日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_SiloModelingTaskClient_Load(object sender, EventArgs e)
        {
            Text = "筒仓建模插件";
            AppendLog("插件已打开。点击“获取新任务”读取新建建模任务，点击“执行建模”执行当前任务列表。");
        }

        /// <summary>
        /// 点击获取新任务按钮后从接口读取新建任务并列入日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_GetNewTasks_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureRuntime();
                _pendingTasks = _repository.GetNewTasks(_config.NewTaskStatus);

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
        /// 点击执行建模按钮后执行当前已获取的新建任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_ExecuteModeling_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureRuntime();
                if (_pendingTasks.Count == 0)
                {
                    AppendLog("当前没有可执行的新任务，请先点击“获取新任务”。");
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
        /// 点击保存族资源按钮后触发当前三维视图族资源保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_SaveRfaResource_Click(object sender, EventArgs e)
        {
            try
            {
                _config = PluginConfig.Load();
                var apiClient = new RfaResourceApiClient(_config.ApiBaseUrl, _config.CoreApiBaseUrl);
                _rfaResourceHandler.SetExecutor(new RfaResourceSaveExecutor(apiClient, _config.TemplateTp3Dir));
                _rfaResourceHandler.SetLog(AppendLog);
                _rfaResourceHandler.Request();
                _rfaResourceEvent.Raise();
                AppendLog("已请求保存当前三维视图中的族资源。");
            }
            catch (Exception ex)
            {
                AppendLog("保存族资源启动失败：" + ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 确保插件运行配置和后端接口仓储已初始化
        /// </summary>
        private void EnsureRuntime()
        {
            if (_config == null)
            {
                _config = PluginConfig.Load();
            }

            if (_repository == null)
            {
                _repository = new SiloTaskRepository(_config.ApiBaseUrl);
            }

            _handler.SetExecutor(new ModelingTaskExecutor(_repository, _config.ModelingDoneStatus, _config.TemplateRootDir));
            _handler.SetLog(AppendLog);
        }

        /// <summary>
        /// 获取任务对应的库型显示文本
        /// </summary>
        /// <param name="task">建模任务</param>
        /// <returns>库型显示文本</returns>
        private string GetTaskSiloTypeText(ModelingTask task)
        {
            Guid dictSiloId;
            if (!Guid.TryParse(task.SiloType, out dictSiloId))
            {
                throw new InvalidOperationException("建模任务的库型字段不是库型字典Id：" + task.SiloType);
            }

            DictSiloRecord dictSilo = _repository.GetDictSilo(dictSiloId);
            return dictSilo.SiloType;
        }

        /// <summary>
        /// 向窗口日志文本框追加日志
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
