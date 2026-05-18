using Autodesk.Revit.UI;
using System;
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
        private ModelingTaskPoller _poller;

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

        private void Form_SiloModelingTaskClient_Load(object sender, EventArgs e)
        {
            Text = "Silo Modeling Task Client";
            AppendLog("Plugin opened. Click Start to listen for new modeling tasks.");
        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            try
            {
                _config = PluginConfig.Load();
                _repository = new SiloTaskRepository(_config.ApiBaseUrl);
                _handler.SetExecutor(new ModelingTaskExecutor(_repository, _config.ModelingDoneStatus));
                _handler.SetLog(AppendLog);
                _poller = new ModelingTaskPoller(_repository, _config.NewTaskStatus, _config.PollIntervalMilliseconds, OnTaskDetected, AppendLog);
                _poller.Start();

                button_Start.Enabled = false;
                button_Stop.Enabled = true;
                AppendLog("Listening started. Poll interval: " + _config.PollIntervalMilliseconds + "ms, new task status: " + _config.NewTaskStatus + ", done status: " + _config.ModelingDoneStatus + ".");
            }
            catch (Exception ex)
            {
                AppendLog("Start failed: " + ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void button_SaveRfaResource_Click(object sender, EventArgs e)
        {
            try
            {
                _config = PluginConfig.Load();
                var apiClient = new RfaResourceApiClient(_config.ApiBaseUrl, _config.CoreApiBaseUrl);
                _rfaResourceHandler.SetExecutor(new RfaResourceSaveExecutor(apiClient));
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

        private void button_Stop_Click(object sender, EventArgs e)
        {
            StopListening();
            AppendLog("Listening stopped.");
        }

        private void Form_SiloModelingTaskClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopListening();
        }

        private void OnTaskDetected(ModelingTask task)
        {
            _handler.SetTask(task);
            _externalEvent.Raise();
        }

        private void StopListening()
        {
            if (_poller != null)
            {
                _poller.Stop();
                _poller = null;
            }

            button_Start.Enabled = true;
            button_Stop.Enabled = false;
        }

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
