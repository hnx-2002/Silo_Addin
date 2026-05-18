using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SiloModelingTaskClient
{
    public class ModelingTaskPoller
    {
        private readonly SiloTaskRepository _repository;
        private readonly int _newTaskStatus;
        private readonly Action<ModelingTask> _onTaskDetected;
        private readonly Action<string> _log;
        private readonly Timer _timer;
        private bool _isPolling;

        public ModelingTaskPoller(SiloTaskRepository repository, int newTaskStatus, int intervalMilliseconds, Action<ModelingTask> onTaskDetected, Action<string> log)
        {
            _repository = repository;
            _newTaskStatus = newTaskStatus;
            _onTaskDetected = onTaskDetected;
            _log = log;
            _timer = new Timer();
            _timer.Interval = intervalMilliseconds;
            _timer.Tick += Timer_Tick;
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_isPolling)
            {
                return;
            }

            _isPolling = true;
            try
            {
                List<ModelingTask> tasks = _repository.GetNewTasks(_newTaskStatus);
                foreach (ModelingTask task in tasks)
                {
                    _log("监听到新建任务：" + task.Id + "，标题：" + task.TaskTitle);
                    _onTaskDetected(task);
                }
            }
            catch (Exception ex)
            {
                _log("监听失败：" + ex.Message);
            }
            finally
            {
                _isPolling = false;
            }
        }
    }
}
