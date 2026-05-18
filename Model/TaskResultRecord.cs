using System;
using Newtonsoft.Json;

namespace SiloModelingTaskClient
{
    public class TaskResultRecord
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("task_base_id")]
        public Guid TaskBaseId { get; set; }

        [JsonProperty("sort")]
        public int? Sort { get; set; }

        [JsonProperty("layout_title")]
        public string LayoutTitle { get; set; }

        [JsonProperty("rfa_resource_id")]
        public Guid? RfaResourceId { get; set; }

        [JsonProperty("layout_type")]
        public string LayoutType { get; set; }

        [JsonProperty("location_x")]
        public decimal? LocationX { get; set; }

        [JsonProperty("location_y")]
        public decimal? LocationY { get; set; }

        [JsonProperty("location_z")]
        public decimal? LocationZ { get; set; }

        [JsonProperty("normal_x")]
        public decimal? NormalX { get; set; }

        [JsonProperty("normal_y")]
        public decimal? NormalY { get; set; }

        [JsonProperty("normal_z")]
        public decimal? NormalZ { get; set; }

        [JsonProperty("rotate_angle")]
        public decimal? RotateAngle { get; set; }

        [JsonProperty("create_account")]
        public string CreateAccount { get; set; }

        [JsonProperty("create_username")]
        public string CreateUsername { get; set; }

        [JsonProperty("create_time")]
        public long? CreateTime { get; set; }

        [JsonProperty("update_account")]
        public string UpdateAccount { get; set; }

        [JsonProperty("update_username")]
        public string UpdateUsername { get; set; }

        [JsonProperty("update_time")]
        public long? UpdateTime { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }
    }
}
