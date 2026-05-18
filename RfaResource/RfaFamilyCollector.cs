using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SiloModelingTaskClient
{
    public class RfaFamilyCollector
    {
        public List<RfaFamilyExportItem> CollectFromActive3DView(Document doc, View activeView)
        {
            if (!(activeView is View3D))
            {
                throw new InvalidOperationException("当前视图不是三维视图，不能保存族资源。");
            }

            return new FilteredElementCollector(doc, activeView.Id)
                .OfClass(typeof(FamilyInstance))
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .Where(x => x.Symbol != null && x.Symbol.Family != null)
                .GroupBy(x => x.Symbol.Family.Id.IntegerValue)
                .Select(g =>
                {
                    FamilyInstance instance = g.First();
                    return new RfaFamilyExportItem
                    {
                        Instance = instance,
                        FamilyName = instance.Symbol.Family.Name,
                        SymbolName = instance.Symbol.Name
                    };
                })
                .OrderBy(x => x.FamilyName)
                .ToList();
        }
    }
}
