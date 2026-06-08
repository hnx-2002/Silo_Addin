using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 筒仓建模任务后端仓储
    /// </summary>
    public class SiloTaskRepository
    {
        private const string ClientName = "SiloModelingTaskClient";

        private readonly string _apiBaseUrl;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// 初始化筒仓建模任务后端仓储
        /// </summary>
        /// <param name="apiBaseUrl">业务接口基础地址</param>
        public SiloTaskRepository(string apiBaseUrl)
        {
            _apiBaseUrl = apiBaseUrl.TrimEnd('/');
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// 获取后端新建状态的建模任务
        /// </summary>
        /// <param name="newTaskStatus">新建任务状态值</param>
        /// <returns>新建建模任务集合</returns>
        public List<ModelingTask> GetNewTasks(int newTaskStatus)
        {
            var search = new
            {
                page = 1,
                pageSize = 9999,
                task_title = (string)null,
                create_account = (string)null,
                update_account = (string)null
            };

            var response = Send<TPResponse<PagedResponse<ModelingTask>>>(HttpMethod.Post, "/Task_base/MultiPagedSearch", search);
            EnsureResponse(response, "Task_base/MultiPagedSearch");
            return response.Result.Datas.Where(x => x.Status == newTaskStatus)
                .OrderBy(x => x.CreateTime)
                .ToList();
        }

        /// <summary>
        /// 获取指定库型字典记录
        /// </summary>
        /// <param name="id">库型字典主键</param>
        /// <returns>库型字典记录</returns>
        public DictSiloRecord GetDictSilo(Guid id)
        {
            var response = Get<TPResponse<DictSiloRecord>>("/Dict_silo/Get/" + id);
            EnsureResponse(response, "Dict_silo/Get");
            if (response.Result == null)
            {
                throw new InvalidOperationException("未找到库型字典记录：" + id);
            }

            return response.Result;
        }

        /// <summary>
        /// 调用后端计算接口获取RFA资源放置结果
        /// </summary>
        /// <param name="taskId">建模任务主键</param>
        /// <returns>建模放置结果集合</returns>
        public List<ModelingPlacementResult> CalculateTemplatePlacements(Guid taskId)
        {
            var response = Get<TPResponse<ResCommon<SiloCalculationResult>>>("/Task_base/Calculate/" + taskId);
            EnsureResponse(response, "Task_base/Calculate");
            EnsureBusinessResponse(response.Result, "Task_base/Calculate");

            return response.Result.Entity.Placements.Select(x => new ModelingPlacementResult
            {
                RfaResourceId = x.RfaResourceId,
                SymbolName = x.LayoutTitle,
                RfaPath = x.RfaPath,
                X = x.Location.X,
                Y = x.Location.Y,
                Z = x.Location.Z,
                RotationAngle = decimal.ToDouble(x.RotateAngle)
            }).ToList();
        }

        /// <summary>
        /// 下载库型模板族文件
        /// </summary>
        /// <param name="rfaPath">族文件后端地址</param>
        /// <returns>族文件字节数据</returns>
        public byte[] DownloadRfaResource(string rfaPath)
        {
            if (string.IsNullOrWhiteSpace(rfaPath))
            {
                throw new InvalidOperationException("族文件路径为空。");
            }

            string url = _apiBaseUrl + "/Rfa_resource/Download" + rfaPath;
            HttpResponseMessage response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
            byte[] bytes = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                string content = Encoding.UTF8.GetString(bytes);
                throw new InvalidOperationException("下载族文件失败：" + url + "，状态码：" + response.StatusCode + "，内容：" + content);
            }

            return bytes;
        }

        /// <summary>
        /// 更新建模任务状态
        /// </summary>
        /// <param name="taskId">建模任务主键</param>
        /// <param name="modelingDoneStatus">建模完成状态值</param>
        public void UpdateTaskStatus(Guid taskId, int modelingDoneStatus)
        {
            var getResponse = Get<TPResponse<ModelingTask>>("/Task_base/Get/" + taskId);
            EnsureResponse(getResponse, "Task_base/Get");

            ModelingTask task = getResponse.Result;
            task.Status = modelingDoneStatus;
            task.UpdateAccount = ClientName;
            task.UpdateUsername = ClientName;
            task.UpdateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            var updateResponse = Send<TPResponse<Res2Para>>(HttpMethod.Put, "/Task_base/Update", task);
            EnsureActionResponse(updateResponse, "Task_base/Update");
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

        /// <summary>
        /// 校验后端业务响应。
        /// </summary>
        /// <typeparam name="T">业务实体类型。</typeparam>
        /// <param name="response">业务响应。</param>
        /// <param name="action">接口名称。</param>
        private static void EnsureBusinessResponse<T>(ResCommon<T> response, string action)
        {
            if (response == null || !response.Status)
            {
                string message = response == null ? string.Empty : response.Message;
                throw new InvalidOperationException(action + "执行失败：" + message);
            }
        }
    }
}
