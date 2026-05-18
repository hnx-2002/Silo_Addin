using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;

namespace SiloModelingTaskClient
{
    public class RevitModelingExternalEventHandler : IExternalEventHandler
    {
        private readonly Queue<ModelingTask> _tasks = new Queue<ModelingTask>();
        private ModelingTaskExecutor _executor;
        private Action<string> _log;

        public void SetExecutor(ModelingTaskExecutor executor)
        {
            _executor = executor;
        }

        public void SetLog(Action<string> log)
        {
            _log = log;
        }

        public void SetTask(ModelingTask task)
        {
            _tasks.Enqueue(task);
        }

        public void Execute(UIApplication app)
        {
            while (_tasks.Count > 0)
            {
                ModelingTask task = _tasks.Dequeue();
                try
                {
                    if (_executor == null)
                    {
                        Log("Executor is not initialized.");
                        continue;
                    }

                    _executor.Execute(task, Log);
                }
                catch (Exception ex)
                {
                    Log("Task execution failed: " + task.Id + ", " + ex.Message);
                }
            }
        }

        public string GetName()
        {
            return "Silo Modeling Task Listener";
        }

        private void Log(string message)
        {
            if (_log != null)
            {
                _log(message);
            }
        }
    }
}
