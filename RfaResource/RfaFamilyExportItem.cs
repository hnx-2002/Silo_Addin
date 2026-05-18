using Autodesk.Revit.DB;

namespace SiloModelingTaskClient
{
    public class RfaFamilyExportItem
    {
        public FamilyInstance Instance { get; set; }
        public string FamilyName { get; set; }
        public string SymbolName { get; set; }
    }
}
