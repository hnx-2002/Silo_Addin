using System;
using Newtonsoft.Json;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 建模任务表记录
    /// </summary>
    public class ModelingTask
    {
        /// <summary>
        /// 任务Id
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 任务标题
        /// </summary>
        [JsonProperty("task_title")]
        public string TaskTitle { get; set; }

        /// <summary>
        /// 库型字典Id
        /// </summary>
        [JsonProperty("silo_type")]
        public string SiloType { get; set; }

        /// <summary>
        /// 储库直径
        /// </summary>
        [JsonProperty("silo_diameter")]
        public decimal? SiloDiameter { get; set; }

        /// <summary>
        /// 项目基点X坐标
        /// </summary>
        [JsonProperty("task_x")]
        public decimal? TaskX { get; set; }

        /// <summary>
        /// 项目基点Y坐标
        /// </summary>
        [JsonProperty("task_y")]
        public decimal? TaskY { get; set; }

        /// <summary>
        /// 项目基点Z坐标
        /// </summary>
        [JsonProperty("task_z")]
        public decimal? TaskZ { get; set; }

        /// <summary>
        /// 旋转角度
        /// </summary>
        [JsonProperty("rotation_angle")]
        public decimal? RotationAngle { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        [JsonProperty("status")]
        public int? Status { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [JsonProperty("error_msg")]
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 创建账号
        /// </summary>
        [JsonProperty("create_account")]
        public string CreateAccount { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [JsonProperty("create_username")]
        public string CreateUsername { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty("create_time")]
        public long? CreateTime { get; set; }

        /// <summary>
        /// 更新账号
        /// </summary>
        [JsonProperty("update_account")]
        public string UpdateAccount { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        [JsonProperty("update_username")]
        public string UpdateUsername { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [JsonProperty("update_time")]
        public long? UpdateTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [JsonProperty("remark")]
        public string Remark { get; set; }
    }
}
