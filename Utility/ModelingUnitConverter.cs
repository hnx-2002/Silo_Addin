using System;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 建模单位转换工具
    /// </summary>
    public static class ModelingUnitConverter
    {
        private const double FeetPerMeter = 3.280839895013123;

        /// <summary>
        /// 将米转换为英尺
        /// </summary>
        /// <param name="value">米制数值</param>
        /// <returns>英尺数值</returns>
        public static double MetersToFeet(double value)
        {
            return value * FeetPerMeter;
        }

        /// <summary>
        /// 将英尺转换为米
        /// </summary>
        /// <param name="value">英尺数值</param>
        /// <returns>米制数值</returns>
        public static double FeetToMeters(double value)
        {
            return value / FeetPerMeter;
        }

        /// <summary>
        /// 将角度转换为弧度
        /// </summary>
        /// <param name="value">角度数值</param>
        /// <returns>弧度数值</returns>
        public static double DegreesToRadians(double value)
        {
            return value * Math.PI / 180.0;
        }
    }
}
