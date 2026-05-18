using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

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
                    Autodesk.Revit.UI.TaskDialog.Show("Notice", "Please open a Revit document before using this plugin.");
                    return Result.Cancelled;
                }
            }
            catch (Exception)
            {
                Autodesk.Revit.UI.TaskDialog.Show("Notice", "Please open a Revit document before using this plugin.");
                return Result.Cancelled;
            }

            var modelingHandler = new RevitModelingExternalEventHandler();
            var modelingEvent = ExternalEvent.Create(modelingHandler);
            var rfaResourceHandler = new RfaResourceSaveExternalEventHandler();
            var rfaResourceEvent = ExternalEvent.Create(rfaResourceHandler);
            var frm = new Form_SiloModelingTaskClient(modelingHandler, modelingEvent, rfaResourceHandler, rfaResourceEvent);
            frm.Show();

            return Result.Succeeded;
        }
    }
}
