using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace SiloModelingTaskClient
{
    public class RfaResourceSaveExternalEventHandler : IExternalEventHandler
    {
        private RfaResourceSaveExecutor _executor;
        private Action<string> _log;
        private bool _requested;

        public void SetExecutor(RfaResourceSaveExecutor executor)
        {
            _executor = executor;
        }

        public void SetLog(Action<string> log)
        {
            _log = log;
        }

        public void Request()
        {
            _requested = true;
        }

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
                    Log("RFA resource executor is not initialized.");
                    return;
                }

                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = uidoc.Document;
                View activeView = doc.ActiveView;
                _executor.Execute(doc, activeView, Log);
            }
            catch (Exception ex)
            {
                Log("保存族资源失败：" + ex.Message);
            }
        }

        public string GetName()
        {
            return "Silo RFA Resource Saver";
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
