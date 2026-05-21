using System;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 建模族实例放置结果
    /// </summary>
    public class ModelingPlacementResult
    {
        /// <summary>
        /// 族资源Id
        /// </summary>
        public Guid RfaResourceId { get; set; }

        /// <summary>
        /// 族名称
        /// </summary>
        public string FamilyName { get; set; }

        /// <summary>
        /// 族类型名称
        /// </summary>
        public string SymbolName { get; set; }

        /// <summary>
        /// Revit内部X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Revit内部Y坐标
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Revit内部Z坐标
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Revit内部旋转角度
        /// </summary>
        public double RotationAngle { get; set; }

        /// <summary>
        /// X坐标米制值
        /// </summary>
        public double LocationXMeters { get; set; }

        /// <summary>
        /// Y坐标米制值
        /// </summary>
        public double LocationYMeters { get; set; }

        /// <summary>
        /// Z坐标米制值
        /// </summary>
        public double LocationZMeters { get; set; }

        /// <summary>
        /// 旋转角度度数值
        /// </summary>
        public double RotationAngleDegrees { get; set; }
    }
}
