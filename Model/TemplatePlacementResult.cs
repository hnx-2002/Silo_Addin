using Newtonsoft.Json;
using System;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 后端模板族放置计算结果
    /// </summary>
    public class TemplatePlacementResult
    {
        /// <summary>
        /// 库型模板Id
        /// </summary>
        [JsonProperty("template_silo_id")]
        public Guid TemplateSiloId { get; set; }

        /// <summary>
        /// 族类型名
        /// </summary>
        [JsonProperty("symbol_name")]
        public string SymbolName { get; set; }

        /// <summary>
        /// 族文件地址
        /// </summary>
        [JsonProperty("rfa_path")]
        public string RfaPath { get; set; }

        /// <summary>
        /// Revit内部X坐标
        /// </summary>
        [JsonProperty("location_x")]
        public double LocationX { get; set; }

        /// <summary>
        /// Revit内部Y坐标
        /// </summary>
        [JsonProperty("location_y")]
        public double LocationY { get; set; }

        /// <summary>
        /// Revit内部Z坐标
        /// </summary>
        [JsonProperty("location_z")]
        public double LocationZ { get; set; }

        /// <summary>
        /// Revit内部旋转角度
        /// </summary>
        [JsonProperty("rotate_angle")]
        public double RotateAngle { get; set; }
    }
}
