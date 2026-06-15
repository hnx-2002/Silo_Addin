using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

namespace SiloModelingTaskClient
{
    [Transaction(TransactionMode.Manual)]
    public class Main_SiloModelingTaskClient : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            AssemblyResolver.Install();

            UIApplication app = commandData.Application;
            UIDocument uidoc = app.ActiveUIDocument;

            try
            {
                if (uidoc == null || uidoc.Document == null)
                {
                    Autodesk.Revit.UI.TaskDialog.Show("Notice", "请打开Revit文档");
                    return Result.Cancelled;
                }
            }
            catch (Exception)
            {
                Autodesk.Revit.UI.TaskDialog.Show("Notice", "请打开Revit文档");
                return Result.Cancelled;
            }


            var (apiUrl, webUrl) = FunCommon.ReadConfig();


#if DEBUG
#else
            Config.APIUrl = apiUrl;
            Config.WebUrl = webUrl;


            bool auth = FunHttp.FormAuth();
            if (!auth)
            {
                MessageBox.Show("鉴权失败");
                return Result.Cancelled;
            }
#endif

            var modelingHandler = new RevitModelingExternalEventHandler();
            var modelingEvent = ExternalEvent.Create(modelingHandler);
            var templateSiloHandler = new TemplateSiloSaveExternalEventHandler();
            var templateSiloEvent = ExternalEvent.Create(templateSiloHandler);
            var frm = new Form_SiloModelingTaskClient(modelingHandler, modelingEvent, templateSiloHandler, templateSiloEvent);
            frm.Show();

            return Result.Succeeded;
        }
    }
}
