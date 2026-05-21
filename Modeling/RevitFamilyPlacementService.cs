using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// Revit族实例放置服务
    /// </summary>
    public class RevitFamilyPlacementService
    {
        private readonly RfaFileCache _fileCache;

        /// <summary>
        /// 初始化Revit族实例放置服务
        /// </summary>
        /// <param name="repository">后端接口仓储</param>
        public RevitFamilyPlacementService(SiloTaskRepository repository)
        {
            _fileCache = new RfaFileCache(repository);
        }

        /// <summary>
        /// 在Revit文档中放置族实例
        /// </summary>
        /// <param name="doc">Revit文档</param>
        /// <param name="placements">族实例放置结果</param>
        /// <param name="resources">族资源字典</param>
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

        /// <summary>
        /// 从本地族文件加载Revit族
        /// </summary>
        /// <param name="doc">Revit文档</param>
        /// <param name="localPath">本地族文件路径</param>
        /// <param name="familyName">族名称</param>
        /// <returns>Revit族</returns>
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

        /// <summary>
        /// 在Revit族中查找指定族类型
        /// </summary>
        /// <param name="doc">Revit文档</param>
        /// <param name="family">Revit族</param>
        /// <param name="symbolName">族类型名称</param>
        /// <returns>Revit族类型</returns>
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
