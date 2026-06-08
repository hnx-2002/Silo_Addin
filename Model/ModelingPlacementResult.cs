using System;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 建模族实例放置结果
    /// </summary>
    public class ModelingPlacementResult
    {
        /// <summary>
        /// RFA资源Id
        /// </summary>
        public Guid RfaResourceId { get; set; }

        /// <summary>
        /// 族类型名
        /// </summary>
        public string SymbolName { get; set; }

        /// <summary>
        /// 族文件地址
        /// </summary>
        public string RfaPath { get; set; }

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

    }
}
