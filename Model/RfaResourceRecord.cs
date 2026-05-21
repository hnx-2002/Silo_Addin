using Newtonsoft.Json;
using System;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 族资源表记录
    /// </summary>
    public class RfaResourceRecord
    {
        /// <summary>
        /// 族资源Id
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 族编码
        /// </summary>
        [JsonProperty("rfa_code")]
        public string RfaCode { get; set; }

        /// <summary>
        /// 族类型名称
        /// </summary>
        [JsonProperty("symbol_name")]
        public string SymbolName { get; set; }

        /// <summary>
        /// 族文件OSS路径
        /// </summary>
        [JsonProperty("rfa_path")]
        public string RfaPath { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        [JsonProperty("file_name")]
        public string FileName { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [JsonProperty("file_size")]
        public int FileSize { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        [JsonProperty("note")]
        public string Note { get; set; }

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
