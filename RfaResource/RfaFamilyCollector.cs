using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SiloModelingTaskClient
{
    public class RfaFamilyCollector
    {
        private static readonly HashSet<string> AllowedFamilyNames = new HashSet<string>
        {
            "结构库底板示意",
            "库底充气斜槽示意"
        };

        public List<RfaFamilyExportItem> CollectFromActive3DView(Document doc, View activeView)
        {
            if (!(activeView is View3D))
            {
                throw new InvalidOperationException("当前视图不是三维视图，不能保存族资源。");
            }

            return CollectAllowedInstancesFromActive3DView(doc, activeView)
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

        public List<FamilyInstance> CollectAllowedInstancesFromActive3DView(Document doc, View activeView)
        {
            if (!(activeView is View3D))
            {
                throw new InvalidOperationException("当前视图不是三维视图，不能读取族实例坐标。");
            }

            return new FilteredElementCollector(doc, activeView.Id)
                .OfClass(typeof(FamilyInstance))
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .Where(x => x.Symbol != null && x.Symbol.Family != null)
                .Where(x => AllowedFamilyNames.Contains(x.Symbol.Family.Name))
                .OrderBy(x => x.Symbol.Family.Name)
                .ThenBy(x => x.Id.IntegerValue)
                .ToList();
        }
    }
}
