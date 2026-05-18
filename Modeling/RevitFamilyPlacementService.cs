using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SiloModelingTaskClient
{
    public class RevitFamilyPlacementService
    {
        private readonly RfaFileCache _fileCache;

        public RevitFamilyPlacementService(SiloTaskRepository repository)
        {
            _fileCache = new RfaFileCache(repository);
        }

        public void Place(Document doc, List<ModelingPlacementResult> placements, Dictionary<string, RfaResourceRecord> resources)
        {
            using (var transaction = new Transaction(doc, "Silo modeling task placement"))
            {
                transaction.Start();

                foreach (ModelingPlacementResult placement in placements)
                {
                    RfaResourceRecord resource = resources[placement.FamilyName];
                    string localPath = _fileCache.GetLocalPath(resource);
                    Family family = LoadFamily(doc, localPath, placement.FamilyName);
                    FamilySymbol symbol = FindSymbol(doc, family, placement.SymbolName);
                    if (!symbol.IsActive)
                    {
                        symbol.Activate();
                        doc.Regenerate();
                    }

                    var point = new XYZ(placement.X, placement.Y, placement.Z);
                    FamilyInstance instance = doc.Create.NewFamilyInstance(point, symbol, StructuralType.NonStructural);
                    if (Math.Abs(placement.RotationAngle) > 1e-9)
                    {
                        Line axis = Line.CreateBound(point, point + XYZ.BasisZ);
                        ElementTransformUtils.RotateElement(doc, instance.Id, axis, placement.RotationAngle);
                    }
                }

                transaction.Commit();
            }
        }

        private Family LoadFamily(Document doc, string localPath, string familyName)
        {
            Family existing = new FilteredElementCollector(doc)
                .OfClass(typeof(Family))
                .Cast<Family>()
                .FirstOrDefault(x => x.Name == familyName);
            if (existing != null)
            {
                return existing;
            }

            Family family;
            bool loaded = doc.LoadFamily(localPath, out family);
            if (!loaded || family == null)
            {
                throw new InvalidOperationException("载入族失败：" + familyName + "，文件：" + localPath);
            }

            return family;
        }

        private FamilySymbol FindSymbol(Document doc, Family family, string symbolName)
        {
            foreach (ElementId symbolId in family.GetFamilySymbolIds())
            {
                FamilySymbol symbol = doc.GetElement(symbolId) as FamilySymbol;
                if (symbol != null && symbol.Name == symbolName)
                {
                    return symbol;
                }
            }

            throw new InvalidOperationException("族中未找到指定类型：" + family.Name + " / " + symbolName);
        }
    }
}
