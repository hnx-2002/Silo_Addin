using Autodesk.Revit.DB;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 待导出的族资源项
    /// </summary>
    public class RfaFamilyExportItem
    {
        /// <summary>
        /// Revit族实例
        /// </summary>
        public FamilyInstance Instance { get; set; }

        /// <summary>
        /// 族名称
        /// </summary>
        public string FamilyName { get; set; }

        /// <summary>
        /// 族类型名称
        /// </summary>
        public string SymbolName { get; set; }
    }
}
