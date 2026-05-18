using Newtonsoft.Json;
using System.Collections.Generic;

namespace SiloModelingTaskClient
{
    public class TPResponse<T>
    {
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("isError")]
        public bool? IsError { get; set; }

        [JsonProperty("result")]
        public T Result { get; set; }
    }

    public class PagedResponse<T>
    {
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("datas")]
        public List<T> Datas { get; set; }
    }

    public class Res2Para
    {
        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class ResUploadFile
    {
        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("filePath")]
        public string FilePath { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("md5")]
        public string Md5 { get; set; }
    }
}
