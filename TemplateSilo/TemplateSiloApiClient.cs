using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// RFA资源后端接口客户端。
    /// </summary>
    public class TemplateSiloApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _token;

        /// <summary>
        /// 初始化RFA资源后端接口客户端。
        /// </summary>
        public TemplateSiloApiClient()
        {
            _httpClient = new HttpClient();
            _token = FunCommon.ReadToken();
            _httpClient.DefaultRequestHeaders.Add("tp_token", _token);
        }

        /// <summary>
        /// 获取库型下拉选项。
        /// </summary>
        /// <returns>库型下拉选项集合。</returns>
        public List<SelectOption<Guid>> GetDictSiloOptions()
        {
            var response = Get<TPResponse<List<SelectOption<Guid>>>>("/Dict_silo/GetOptions");
            EnsureResponse(response, "Dict_silo/GetOptions");
            return response.Result;
        }

        /// <summary>
        /// 上传RFA文件。
        /// </summary>
        /// <param name="rfaFile">RFA文件数据。</param>
        /// <returns>文件上传结果。</returns>
        public ResUploadFile UploadRfa(RfaFileData rfaFile)
        {
            string url = BuildUrl("/UploadFile/UploadFile");
            using (var form = new MultipartFormDataContent())
            using (var content = new ByteArrayContent(rfaFile.Bytes))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                form.Add(content, "files", rfaFile.FileName);

                HttpResponseMessage response = _httpClient.PostAsync(url, form).GetAwaiter().GetResult();
                string responseText = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException("UploadFile/UploadFile request failed. StatusCode: " + response.StatusCode + ", Content: " + responseText);
                }

                var apiResponse = JsonConvert.DeserializeObject<TPResponse<ResUploadFile>>(responseText);
                EnsureResponse(apiResponse, "UploadFile/UploadFile");
                if (apiResponse.Result == null || !apiResponse.Result.Status)
                {
                    string message = apiResponse.Result == null ? apiResponse.Message : apiResponse.Result.Msg;
                    throw new InvalidOperationException("UploadFile/UploadFile action failed: " + message);
                }

                return apiResponse.Result;
            }
        }

        /// <summary>
        /// 根据库型Id删除旧RFA资源记录。
        /// </summary>
        /// <param name="dictSiloId">库型Id。</param>
        public void DeleteRfaResourcesByDictSiloId(Guid dictSiloId)
        {
            var searchResponse = Get<TPResponse<List<RfaResourceRecord>>>(
                "/Rfa_resource/Search_Dict_silo_id/" + dictSiloId);
            EnsureResponse(searchResponse, "Rfa_resource/Search_Dict_silo_id");

            var ids = new List<Guid>();
            foreach (RfaResourceRecord record in searchResponse.Result)
            {
                ids.Add(record.Id);
            }

            if (ids.Count == 0)
            {
                return;
            }

            var deleteResponse = Send<TPResponse<Res2Para>>(HttpMethod.Delete, "/Rfa_resource/Delete", ids.ToArray());
            EnsureActionResponse(deleteResponse, "Rfa_resource/Delete");
        }

        /// <summary>
        /// 新增RFA资源记录。
        /// </summary>
        /// <param name="record">RFA资源记录。</param>
        public void AddRfaResource(RfaResourceRecord record)
        {
            var response = Send<TPResponse<Res2Para>>(HttpMethod.Post, "/Rfa_resource/Add", record);
            EnsureActionResponse(response, "Rfa_resource/Add");
        }

        /// <summary>
        /// 发送GET请求。
        /// </summary>
        /// <typeparam name="T">响应类型。</typeparam>
        /// <param name="path">接口路径。</param>
        /// <returns>响应结果。</returns>
        private T Get<T>(string path)
        {
            return Send<T>(HttpMethod.Get, path, null);
        }

        /// <summary>
        /// 发送后端接口请求。
        /// </summary>
        /// <typeparam name="T">响应类型。</typeparam>
        /// <param name="method">HTTP方法。</param>
        /// <param name="path">接口路径。</param>
        /// <param name="body">请求体。</param>
        /// <returns>响应结果。</returns>
        private T Send<T>(HttpMethod method, string path, object body)
        {
            string url = BuildUrl(path);
            using (var request = new HttpRequestMessage(method, url))
            {
                if (body != null)
                {
                    string json = JsonConvert.SerializeObject(body);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                HttpResponseMessage response = _httpClient.SendAsync(request).GetAwaiter().GetResult();
                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException("API request failed. Method: " + method.Method + ", Url: " + url + ", StatusCode: " + response.StatusCode + ", Content: " + content);
                }

                return JsonConvert.DeserializeObject<T>(content);
            }
        }

        /// <summary>
        /// 拼接插件平台代理接口地址。
        /// </summary>
        /// <param name="path">接口路径。</param>
        /// <returns>完整接口地址。</returns>
        private static string BuildUrl(string path)
        {
            return Config.APIUrl.TrimEnd('/') + "/" + Config.ToolCode + path;
        }

        /// <summary>
        /// 校验后端统一响应。
        /// </summary>
        /// <typeparam name="T">响应结果类型。</typeparam>
        /// <param name="response">后端统一响应。</param>
        /// <param name="action">接口名称。</param>
        private static void EnsureResponse<T>(TPResponse<T> response, string action)
        {
            if (response == null)
            {
                throw new InvalidOperationException(action + " returned empty response.");
            }

            if (response.IsError == true)
            {
                throw new InvalidOperationException(action + " failed: " + response.Message);
            }
        }

        /// <summary>
        /// 校验后端操作响应。
        /// </summary>
        /// <param name="response">后端操作响应。</param>
        /// <param name="action">接口名称。</param>
        private static void EnsureActionResponse(TPResponse<Res2Para> response, string action)
        {
            EnsureResponse(response, action);
            if (response.Result == null || !response.Result.Status)
            {
                string message = response.Result == null ? response.Message : response.Result.Message;
                throw new InvalidOperationException(action + " failed: " + message);
            }
        }
    }
}
