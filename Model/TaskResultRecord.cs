using System;
using Newtonsoft.Json;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 建模任务结果表记录
    /// </summary>
    public class TaskResultRecord
    {
        /// <summary>
        /// 结果Id
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 建模任务Id
        /// </summary>
        [JsonProperty("task_base_id")]
        public Guid TaskBaseId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [JsonProperty("sort")]
        public int? Sort { get; set; }

        /// <summary>
        /// 布置标题
        /// </summary>
        [JsonProperty("layout_title")]
        public string LayoutTitle { get; set; }

        /// <summary>
        /// 族资源Id
        /// </summary>
        [JsonProperty("rfa_resource_id")]
        public Guid? RfaResourceId { get; set; }

        /// <summary>
        /// 布置类型
        /// </summary>
        [JsonProperty("layout_type")]
        public string LayoutType { get; set; }

        /// <summary>
        /// 布置X坐标
        /// </summary>
        [JsonProperty("location_x")]
        public decimal? LocationX { get; set; }

        /// <summary>
        /// 布置Y坐标
        /// </summary>
        [JsonProperty("location_y")]
        public decimal? LocationY { get; set; }

        /// <summary>
        /// 布置Z坐标
        /// </summary>
        [JsonProperty("location_z")]
        public decimal? LocationZ { get; set; }

        /// <summary>
        /// 旋转轴X方向
        /// </summary>
        [JsonProperty("normal_x")]
        public decimal? NormalX { get; set; }

        /// <summary>
        /// 旋转轴Y方向
        /// </summary>
        [JsonProperty("normal_y")]
        public decimal? NormalY { get; set; }

        /// <summary>
        /// 旋转轴Z方向
        /// </summary>
        [JsonProperty("normal_z")]
        public decimal? NormalZ { get; set; }

        /// <summary>
        /// 旋转角度
        /// </summary>
        [JsonProperty("rotate_angle")]
        public decimal? RotateAngle { get; set; }

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
