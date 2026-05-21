using Autodesk.Revit.DB;
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
                        Log("建模执行器未初始化。");
                        continue;
                    }

                    UIDocument uidoc = app.ActiveUIDocument;
                    if (uidoc == null || uidoc.Document == null)
                    {
                        throw new InvalidOperationException("当前没有打开的Revit文档。");
                    }

                    _executor.Execute(uidoc.Document, task, Log);
                }
                catch (Exception ex)
                {
                    Log("任务执行失败：" + task.Id + "，" + ex.Message);
                }
            }
        }

        public string GetName()
        {
            return "筒仓建模任务执行器";
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
