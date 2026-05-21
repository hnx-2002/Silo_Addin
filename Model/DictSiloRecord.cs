using Newtonsoft.Json;
using System;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 库型字典表记录
    /// </summary>
    public class DictSiloRecord
    {
        /// <summary>
        /// 库型字典Id
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 库型
        /// </summary>
        [JsonProperty("silo_type")]
        public string SiloType { get; set; }

        /// <summary>
        /// 库型名称
        /// </summary>
        [JsonProperty("silo_name")]
        public string SiloName { get; set; }
    }
}
