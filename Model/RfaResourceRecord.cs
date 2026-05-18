using Newtonsoft.Json;
using System;

namespace SiloModelingTaskClient
{
    public class RfaResourceRecord
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("rfa_code")]
        public string RfaCode { get; set; }

        [JsonProperty("symbol_name")]
        public string SymbolName { get; set; }

        [JsonProperty("rfa_path")]
        public string RfaPath { get; set; }

        [JsonProperty("file_name")]
        public string FileName { get; set; }

        [JsonProperty("file_size")]
        public int FileSize { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

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
