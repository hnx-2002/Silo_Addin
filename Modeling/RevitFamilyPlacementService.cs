using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.IO;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// Revit族实例放置服务
    /// </summary>
    public class RevitFamilyPlacementService
    {
        private readonly SiloTaskRepository _repository;

        /// <summary>
        /// 初始化Revit族实例放置服务
        /// </summary>
        /// <param name="repository">后端接口仓储</param>
        public RevitFamilyPlacementService(SiloTaskRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 根据后端计算结果在Revit文档中放置族实例
        /// </summary>
        /// <param name="doc">Revit文档</param>
        /// <param name="taskId">建模任务主键</param>
        /// <param name="placements">族实例放置结果</param>
        public void Place(Document doc, Guid taskId, List<ModelingPlacementResult> placements)
        {
            var localPaths = new Dictionary<Guid, string>();
            try
            {
                using (var transaction = new Transaction(doc, "Silo modeling task placement"))
                {
                    transaction.Start();

                    foreach (ModelingPlacementResult placement in placements)
                    {
                        string localPath = GetLocalPath(doc, taskId, placement, localPaths);
                        Family family = LoadFamily(doc, localPath);
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
            finally
            {
                CleanupDownloadedFiles(localPaths);
            }
        }

        /// <summary>
        /// 获取族文件本地临时路径
        /// </summary>
        /// <param name="doc">Revit文档</param>
        /// <param name="taskId">建模任务主键</param>
        /// <param name="placement">族实例放置结果</param>
        /// <param name="localPaths">已下载族文件路径缓存</param>
        /// <returns>族文件本地临时路径</returns>
        private string GetLocalPath(
            Document doc,
            Guid taskId,
            ModelingPlacementResult placement,
            Dictionary<Guid, string> localPaths)
        {
            if (localPaths.ContainsKey(placement.TemplateSiloId))
            {
                return localPaths[placement.TemplateSiloId];
            }

            byte[] bytes = _repository.DownloadTemplateSiloRfa(placement.RfaPath);
            string dir = Path.Combine(Path.GetTempPath(), "SiloModelingTaskClient", "template_silo", placement.TemplateSiloId.ToString());
            Directory.CreateDirectory(dir);

            string fileName = Path.GetFileName(placement.RfaPath);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new InvalidOperationException("族文件路径缺少文件名：" + placement.RfaPath);
            }

            string localPath = Path.Combine(dir, fileName);
            File.WriteAllBytes(localPath, bytes);
            string renamedPath = RenameDownloadedFamily(doc, taskId, placement, localPath, dir);
            File.Delete(localPath);
            localPaths[placement.TemplateSiloId] = renamedPath;
            return renamedPath;
        }

        /// <summary>
        /// 将下载族改为任务唯一族名
        /// </summary>
        /// <param name="doc">Revit文档</param>
        /// <param name="taskId">建模任务主键</param>
        /// <param name="placement">族实例放置结果</param>
        /// <param name="localPath">下载族文件本地路径</param>
        /// <param name="dir">族文件临时目录</param>
        /// <returns>改名后的族文件本地路径</returns>
        private string RenameDownloadedFamily(
            Document doc,
            Guid taskId,
            ModelingPlacementResult placement,
            string localPath,
            string dir)
        {
            string familyName = Path.GetFileNameWithoutExtension(localPath);
            string uniqueFamilyName = familyName + "_" + taskId.ToString("N");
            string renamedPath = Path.Combine(dir, uniqueFamilyName + ".rfa");
            Document familyDoc = doc.Application.OpenDocumentFile(localPath);
            bool familyDocClosed = false;
            try
            {
                using (var transaction = new Transaction(familyDoc, "Rename downloaded template family"))
                {
                    transaction.Start();
                    familyDoc.OwnerFamily.Name = uniqueFamilyName;
                    EnsureFamilyTypeName(familyDoc, placement.SymbolName);
                    transaction.Commit();
                }

                var saveOptions = new SaveAsOptions
                {
                    OverwriteExistingFile = true,
                    Compact = true
                };
                familyDoc.SaveAs(renamedPath, saveOptions);
                familyDoc.Close(false);
                familyDocClosed = true;
                return renamedPath;
            }
            finally
            {
                if (!familyDocClosed)
                {
                    familyDoc.Close(false);
                }
            }
        }

        /// <summary>
        /// 保持下载族中的模板族类型名
        /// </summary>
        /// <param name="familyDoc">族文档</param>
        /// <param name="symbolName">模板族类型名</param>
        private void EnsureFamilyTypeName(Document familyDoc, string symbolName)
        {
            FamilyManager familyManager = familyDoc.FamilyManager;
            foreach (FamilyType familyType in familyManager.Types)
            {
                if (familyType.Name == symbolName)
                {
                    return;
                }
            }

            familyManager.RenameCurrentType(symbolName);
        }

        /// <summary>
        /// 从本地族文件加载Revit族
        /// </summary>
        /// <param name="doc">Revit文档</param>
        /// <param name="localPath">族文件本地临时路径</param>
        /// <returns>Revit族</returns>
        private Family LoadFamily(Document doc, string localPath)
        {
            Family family;
            bool loaded = doc.LoadFamily(localPath, new TemplateFamilyLoadOptions(), out family);
            if (!loaded || family == null)
            {
                throw new InvalidOperationException("载入族失败，文件：" + localPath);
            }

            return family;
        }

        /// <summary>
        /// 在Revit族中查找指定族类型
        /// </summary>
        /// <param name="doc">Revit文档</param>
        /// <param name="family">Revit族</param>
        /// <param name="symbolName">族类型名</param>
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

        /// <summary>
        /// 清理已下载的本地临时族文件
        /// </summary>
        /// <param name="localPaths">已下载族文件路径缓存</param>
        private void CleanupDownloadedFiles(Dictionary<Guid, string> localPaths)
        {
            foreach (string localPath in localPaths.Values)
            {
                if (File.Exists(localPath))
                {
                    File.Delete(localPath);
                }

                string dir = Path.GetDirectoryName(localPath);
                if (!string.IsNullOrWhiteSpace(dir) && Directory.Exists(dir))
                {
                    Directory.Delete(dir);
                }
            }
        }

        /// <summary>
        /// 模板族加载选项
        /// </summary>
        private class TemplateFamilyLoadOptions : IFamilyLoadOptions
        {
            /// <summary>
            /// 同名族已存在时使用下载的模板族覆盖
            /// </summary>
            /// <param name="familyInUse">族是否正在使用</param>
            /// <param name="overwriteParameterValues">是否覆盖参数值</param>
            /// <returns>是否继续加载族</returns>
            public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
            {
                overwriteParameterValues = true;
                return true;
            }

            /// <summary>
            /// 共享族已存在时使用下载的模板族覆盖
            /// </summary>
            /// <param name="sharedFamily">已存在的共享族</param>
            /// <param name="familyInUse">族是否正在使用</param>
            /// <param name="source">族来源</param>
            /// <param name="overwriteParameterValues">是否覆盖参数值</param>
            /// <returns>是否继续加载族</returns>
            public bool OnSharedFamilyFound(
                Family sharedFamily,
                bool familyInUse,
                out FamilySource source,
                out bool overwriteParameterValues)
            {
                source = FamilySource.Family;
                overwriteParameterValues = true;
                return true;
            }
        }
    }
}
