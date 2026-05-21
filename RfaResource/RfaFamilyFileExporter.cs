using Autodesk.Revit.DB;
using System;
using System.IO;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// Revit族文件导出器
    /// </summary>
    public class RfaFamilyFileExporter
    {
        /// <summary>
        /// 将Revit族实例对应的族导出为族文件数据
        /// </summary>
        /// <param name="doc">Revit文档</param>
        /// <param name="item">待导出的族资源项</param>
        /// <returns>导出的族文件数据</returns>
        public RfaFileData Export(Document doc, RfaFamilyExportItem item)
        {
            if (item == null || item.Instance == null)
            {
                throw new InvalidOperationException("族实例为空，不能导出族文件。");
            }

            Family family = item.Instance.Symbol.Family;
            Document familyDoc = doc.EditFamily(family);
            if (familyDoc == null)
            {
                throw new InvalidOperationException("无法编辑族：" + item.FamilyName);
            }

            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".rfa");
            bool familyDocClosed = false;
            try
            {
                var saveOptions = new SaveAsOptions
                {
                    OverwriteExistingFile = true,
                    Compact = true
                };

                familyDoc.SaveAs(tempPath, saveOptions);
                familyDoc.Close(false);
                familyDocClosed = true;

                return new RfaFileData
                {
                    FamilyName = item.FamilyName,
                    SymbolName = item.SymbolName,
                    FileName = item.FamilyName + ".rfa",
                    Bytes = File.ReadAllBytes(tempPath)
                };
            }
            finally
            {
                if (!familyDocClosed)
                {
                    familyDoc.Close(false);
                }

                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }
    }
}
