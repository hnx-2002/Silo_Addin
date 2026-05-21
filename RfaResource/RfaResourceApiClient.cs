using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 族资源后端接口客户端
    /// </summary>
    public class RfaResourceApiClient
    {
        private readonly string _apiBaseUrl;
        private readonly string _coreApiBaseUrl;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// 初始化族资源后端接口客户端
        /// </summary>
        /// <param name="apiBaseUrl">业务接口基础地址</param>
        /// <param name="coreApiBaseUrl">核心接口基础地址</param>
        public RfaResourceApiClient(string apiBaseUrl, string coreApiBaseUrl)
        {
            _apiBaseUrl = apiBaseUrl.TrimEnd('/');
            _coreApiBaseUrl = coreApiBaseUrl.TrimEnd('/');
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// 上传族文件到OSS
        /// </summary>
        /// <param name="rfaFile">族文件数据</param>
        /// <returns>文件上传结果</returns>
        public ResUploadFile UploadRfa(RfaFileData rfaFile)
        {
            string url = _coreApiBaseUrl + "/UploadFile/UploadFile";
            using (var form = new MultipartFormDataContent())
            using (var content = new ByteArrayContent(rfaFile.Bytes))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                form.Add(content, "files", rfaFile.FileName);

                HttpResponseMessage response = _httpClient.PostAsync(url, form).GetAwaiter().GetResult();
                string responseText = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException("上传文件失败，状态码：" + response.StatusCode + "，内容：" + responseText);
                }

                var apiResponse = JsonConvert.DeserializeObject<TPResponse<ResUploadFile>>(responseText);
                EnsureResponse(apiResponse, "UploadFile/UploadFile");
                if (apiResponse.Result == null || !apiResponse.Result.Status)
                {
                    string message = apiResponse.Result == null ? apiResponse.Message : apiResponse.Result.Msg;
                    throw new InvalidOperationException("上传文件接口执行失败：" + message);
                }

                return apiResponse.Result;
            }
        }

        /// <summary>
        /// 新增族资源记录
        /// </summary>
        /// <param name="record">族资源记录</param>
        public void AddRfaResource(RfaResourceRecord record)
        {
            var response = Send<TPResponse<Res2Para>>(HttpMethod.Post, "/Rfa_resource/Add", record);
            EnsureResponse(response, "Rfa_resource/Add");
            if (response.Result == null || !response.Result.Status)
            {
                string message = response.Result == null ? response.Message : response.Result.Message;
                throw new InvalidOperationException("新增族资源记录失败：" + message);
            }
        }

        /// <summary>
        /// 发送业务接口请求
        /// </summary>
        /// <typeparam name="T">响应类型</typeparam>
        /// <param name="method">HTTP方法</param>
        /// <param name="path">接口路径</param>
        /// <param name="body">请求体</param>
        /// <returns>反序列化后的响应</returns>
        private T Send<T>(HttpMethod method, string path, object body)
        {
            string url = _apiBaseUrl + path;
            using (var request = new HttpRequestMessage(method, url))
            {
                string json = JsonConvert.SerializeObject(body);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.SendAsync(request).GetAwaiter().GetResult();
                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException("接口请求失败：" + method.Method + " " + url + "，状态码：" + response.StatusCode + "，内容：" + content);
                }

                return JsonConvert.DeserializeObject<T>(content);
            }
        }

        /// <summary>
        /// 校验后端统一响应
        /// </summary>
        /// <typeparam name="T">响应结果类型</typeparam>
        /// <param name="response">后端统一响应</param>
        /// <param name="action">接口名称</param>
        private static void EnsureResponse<T>(TPResponse<T> response, string action)
        {
            if (response == null)
            {
                throw new InvalidOperationException(action + "返回空响应。");
            }

            if (response.IsError == true)
            {
                throw new InvalidOperationException(action + "执行失败：" + response.Message);
            }
        }
    }
}
