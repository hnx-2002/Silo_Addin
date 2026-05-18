using System;
using Newtonsoft.Json;

namespace SiloModelingTaskClient
{
    public class ModelingTask
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("task_title")]
        public string TaskTitle { get; set; }

        [JsonProperty("silo_type")]
        public string SiloType { get; set; }

        [JsonProperty("silo_diameter")]
        public decimal? SiloDiameter { get; set; }

        [JsonProperty("task_x")]
        public decimal? TaskX { get; set; }

        [JsonProperty("task_y")]
        public decimal? TaskY { get; set; }

        [JsonProperty("task_z")]
        public decimal? TaskZ { get; set; }

        [JsonProperty("rotation_angle")]
        public decimal? RotationAngle { get; set; }

        [JsonProperty("status")]
        public int? Status { get; set; }

        [JsonProperty("error_msg")]
        public string ErrorMsg { get; set; }

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
