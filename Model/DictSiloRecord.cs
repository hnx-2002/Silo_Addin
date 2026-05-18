using Newtonsoft.Json;
using System;

namespace SiloModelingTaskClient
{
    public class DictSiloRecord
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("silo_type")]
        public string SiloType { get; set; }

        [JsonProperty("silo_name")]
        public string SiloName { get; set; }
    }
}
