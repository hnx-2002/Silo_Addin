using Newtonsoft.Json;
using System;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 库型模板表记录
    /// </summary>
    public class TemplateSiloRecord
    {
        /// <summary>
        /// 库型模板Id
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 库型名称
        /// </summary>
        [JsonProperty("silo_name")]
        public string SiloName { get; set; }

        /// <summary>
        /// 族类型名
        /// </summary>
        [JsonProperty("symbol_name")]
        public string SymbolName { get; set; }

        /// <summary>
        /// 族文件地址
        /// </summary>
        [JsonProperty("rfa_path")]
        public string RfaPath { get; set; }

        /// <summary>
        /// 模板X坐标
        /// </summary>
        [JsonProperty("template_x")]
        public decimal TemplateX { get; set; }

        /// <summary>
        /// 模板Y坐标
        /// </summary>
        [JsonProperty("template_y")]
        public decimal TemplateY { get; set; }

        /// <summary>
        /// 模板Z坐标
        /// </summary>
        [JsonProperty("template_z")]
        public decimal TemplateZ { get; set; }

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
