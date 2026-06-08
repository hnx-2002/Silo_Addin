using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 后端筒仓计算结果。
    /// </summary>
    public class SiloCalculationResult
    {
        /// <summary>
        /// 族放置结果集合。
        /// </summary>
        [JsonProperty("placements")]
        public List<SiloPlacementResult> Placements { get; set; }
    }

    /// <summary>
    /// 后端单个族放置结果。
    /// </summary>
    public class SiloPlacementResult
    {
        /// <summary>
        /// RFA资源Id。
        /// </summary>
        [JsonProperty("rfa_resource_id")]
        public Guid RfaResourceId { get; set; }

        /// <summary>
        /// 布置标题。
        /// </summary>
        [JsonProperty("layout_title")]
        public string LayoutTitle { get; set; }

        /// <summary>
        /// RFA文件路径。
        /// </summary>
        [JsonProperty("rfa_path")]
        public string RfaPath { get; set; }

        /// <summary>
        /// 计算后的放置点。
        /// </summary>
        [JsonProperty("location")]
        public ApiXyz Location { get; set; }

        /// <summary>
        /// 旋转角度，单位为弧度。
        /// </summary>
        [JsonProperty("rotate_angle")]
        public decimal RotateAngle { get; set; }
    }

    /// <summary>
    /// 后端XYZ点或向量模型。
    /// </summary>
    public class ApiXyz
    {
        /// <summary>
        /// X坐标。
        /// </summary>
        [JsonProperty("x")]
        public double X { get; set; }

        /// <summary>
        /// Y坐标。
        /// </summary>
        [JsonProperty("y")]
        public double Y { get; set; }

        /// <summary>
        /// Z坐标。
        /// </summary>
        [JsonProperty("z")]
        public double Z { get; set; }
    }
}
