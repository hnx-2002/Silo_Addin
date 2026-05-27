using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 库型模板后端接口客户端
    /// </summary>
    public class TemplateSiloApiClient
    {
        private readonly string _apiBaseUrl;
        private readonly string _coreApiBaseUrl;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// 初始化库型模板后端接口客户端
        /// </summary>
        /// <param name="apiBaseUrl">业务接口基础地址</param>
        /// <param name="coreApiBaseUrl">核心接口基础地址</param>
        public TemplateSiloApiClient(string apiBaseUrl, string coreApiBaseUrl)
        {
            _apiBaseUrl = apiBaseUrl.TrimEnd('/');
            _coreApiBaseUrl = coreApiBaseUrl.TrimEnd('/');
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// 上传族文件
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
        /// 根据库型名称删除旧库型模板记录
        /// </summary>
        /// <param name="siloName">库型名称</param>
        public void DeleteTemplateSiloBySiloName(string siloName)
        {
            var searchResponse = Get<TPResponse<List<TemplateSiloRecord>>>(
                "/Template_silo/Search_Silo_name/" + Uri.EscapeDataString(siloName));
            EnsureResponse(searchResponse, "Template_silo/Search_Silo_name");

            var ids = new List<Guid>();
            foreach (TemplateSiloRecord record in searchResponse.Result)
            {
                ids.Add(record.Id);
            }

            if (ids.Count == 0)
            {
                return;
            }

            var deleteResponse = Send<TPResponse<Res2Para>>(HttpMethod.Delete, "/Template_silo/Delete", ids.ToArray());
            EnsureActionResponse(deleteResponse, "Template_silo/Delete");
        }

        /// <summary>
        /// 批量新增库型模板记录
        /// </summary>
        /// <param name="records">库型模板记录集合</param>
        public void AddTemplateSiloBatch(List<TemplateSiloRecord> records)
        {
            var response = Send<TPResponse<Res2Para>>(HttpMethod.Post, "/Template_silo/AddBatch", records);
            EnsureActionResponse(response, "Template_silo/AddBatch");
        }

        /// <summary>
        /// 发送GET请求
        /// </summary>
        /// <typeparam name="T">响应类型</typeparam>
        /// <param name="path">接口路径</param>
        /// <returns>响应结果</returns>
        private T Get<T>(string path)
        {
            return Send<T>(HttpMethod.Get, path, null);
        }

        /// <summary>
        /// 发送后端接口请求
        /// </summary>
        /// <typeparam name="T">响应类型</typeparam>
        /// <param name="method">HTTP方法</param>
        /// <param name="path">接口路径</param>
        /// <param name="body">请求体</param>
        /// <returns>响应结果</returns>
        private T Send<T>(HttpMethod method, string path, object body)
        {
            string url = _apiBaseUrl + path;
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

        /// <summary>
        /// 校验后端操作响应
        /// </summary>
        /// <param name="response">后端操作响应</param>
        /// <param name="action">接口名称</param>
        private static void EnsureActionResponse(TPResponse<Res2Para> response, string action)
        {
            EnsureResponse(response, action);
            if (response.Result == null || !response.Result.Status)
            {
                string message = response.Result == null ? response.Message : response.Result.Message;
                throw new InvalidOperationException(action + "执行失败：" + message);
            }
        }
    }
}
