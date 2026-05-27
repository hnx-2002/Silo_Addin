using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 库型模板保存ExternalEvent处理器
    /// </summary>
    public class TemplateSiloSaveExternalEventHandler : IExternalEventHandler
    {
        private TemplateSiloSaveExecutor _executor;
        private Action<string> _log;
        private bool _requested;

        /// <summary>
        /// 设置库型模板保存执行器
        /// </summary>
        /// <param name="executor">库型模板保存执行器</param>
        public void SetExecutor(TemplateSiloSaveExecutor executor)
        {
            _executor = executor;
        }

        /// <summary>
        /// 设置日志输出方法
        /// </summary>
        /// <param name="log">日志输出方法</param>
        public void SetLog(Action<string> log)
        {
            _log = log;
        }

        /// <summary>
        /// 标记一次库型模板保存请求
        /// </summary>
        public void Request()
        {
            _requested = true;
        }

        /// <summary>
        /// 在Revit API上下文中执行库型模板保存请求
        /// </summary>
        /// <param name="app">Revit应用程序对象</param>
        public void Execute(UIApplication app)
        {
            if (!_requested)
            {
                return;
            }

            _requested = false;
            try
            {
                if (_executor == null)
                {
                    Log("库型模板保存执行器未初始化。");
                    return;
                }

                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = uidoc.Document;
                View activeView = doc.ActiveView;
                _executor.Execute(doc, activeView, Log);
            }
            catch (Exception ex)
            {
                Log("保存库型模板失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 获取ExternalEvent处理器名称
        /// </summary>
        /// <returns>ExternalEvent处理器名称</returns>
        public string GetName()
        {
            return "筒仓库型模板保存器";
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="message">日志内容</param>
        private void Log(string message)
        {
            if (_log != null)
            {
                _log(message);
            }
        }
    }
}
