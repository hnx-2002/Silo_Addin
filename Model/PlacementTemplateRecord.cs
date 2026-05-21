using Newtonsoft.Json;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 族实例布置模板记录
    /// </summary>
    public class PlacementTemplateRecord
    {
        /// <summary>
        /// Revit元素Id
        /// </summary>
        [JsonProperty("elementId")]
        public int ElementId { get; set; }

        /// <summary>
        /// 族名称
        /// </summary>
        [JsonProperty("familyName")]
        public string FamilyName { get; set; }

        /// <summary>
        /// 族类型名称
        /// </summary>
        [JsonProperty("symbolName")]
        public string SymbolName { get; set; }

        /// <summary>
        /// 模板X坐标
        /// </summary>
        [JsonProperty("x")]
        public double X { get; set; }

        /// <summary>
        /// 模板Y坐标
        /// </summary>
        [JsonProperty("y")]
        public double Y { get; set; }

        /// <summary>
        /// 模板Z坐标
        /// </summary>
        [JsonProperty("z")]
        public double Z { get; set; }
    }
}
