using Newtonsoft.Json;
using System.Collections.Generic;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 后端接口统一响应模型
    /// </summary>
    /// <typeparam name="T">响应结果类型</typeparam>
    public class TPResponse<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        /// <summary>
        /// 响应消息
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// 是否错误
        /// </summary>
        [JsonProperty("isError")]
        public bool? IsError { get; set; }

        /// <summary>
        /// 响应结果
        /// </summary>
        [JsonProperty("result")]
        public T Result { get; set; }
    }

    /// <summary>
    /// 后端分页响应模型
    /// </summary>
    /// <typeparam name="T">分页数据类型</typeparam>
    public class PagedResponse<T>
    {
        /// <summary>
        /// 总数量
        /// </summary>
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        /// <summary>
        /// 当前页数据
        /// </summary>
        [JsonProperty("datas")]
        public List<T> Datas { get; set; }
    }

    /// <summary>
    /// 后端操作响应模型
    /// </summary>
    public class Res2Para
    {
        /// <summary>
        /// 操作状态
        /// </summary>
        [JsonProperty("status")]
        public bool Status { get; set; }

        /// <summary>
        /// 操作消息
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
    }

    /// <summary>
    /// 文件上传响应模型
    /// </summary>
    public class ResUploadFile
    {
        /// <summary>
        /// 上传状态
        /// </summary>
        [JsonProperty("status")]
        public bool Status { get; set; }

        /// <summary>
        /// 文件OSS路径
        /// </summary>
        [JsonProperty("filePath")]
        public string FilePath { get; set; }

        /// <summary>
        /// 上传消息
        /// </summary>
        [JsonProperty("msg")]
        public string Msg { get; set; }

        /// <summary>
        /// 文件MD5
        /// </summary>
        [JsonProperty("md5")]
        public string Md5 { get; set; }
    }
}
