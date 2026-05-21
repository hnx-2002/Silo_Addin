using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 筒仓建模任务后端接口仓储
    /// </summary>
    public class SiloTaskRepository
    {
        private const string ClientName = "SiloModelingTaskClient";

        private readonly string _apiBaseUrl;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// 初始化筒仓建模任务后端接口仓储
        /// </summary>
        /// <param name="apiBaseUrl">业务接口基础地址</param>
        public SiloTaskRepository(string apiBaseUrl)
        {
            _apiBaseUrl = apiBaseUrl.TrimEnd('/');
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// 获取新建状态的建模任务
        /// </summary>
        /// <param name="newTaskStatus">新建任务状态码</param>
        /// <returns>新建建模任务列表</returns>
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
        /// 根据建模任务Id获取已有任务结果
        /// </summary>
        /// <param name="taskBaseId">建模任务Id</param>
        /// <returns>任务结果列表</returns>
        public List<TaskResultRecord> GetTaskResults(Guid taskBaseId)
        {
            var response = Get<TPResponse<List<TaskResultRecord>>>("/Task_result/Search_Task_base_id/" + taskBaseId);
            EnsureResponse(response, "Task_result/Search_Task_base_id");
            return response.Result;
        }

        /// <summary>
        /// 根据库型字典Id获取库型字典记录
        /// </summary>
        /// <param name="id">库型字典Id</param>
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
        /// 根据族编码获取族资源记录
        /// </summary>
        /// <param name="rfaCode">族编码</param>
        /// <returns>族资源记录</returns>
        public RfaResourceRecord GetRfaResourceByCode(string rfaCode)
        {
            var search = new
            {
                page = 1,
                pageSize = 9999,
                rfa_code = rfaCode,
                symbol_name = (string)null,
                rfa_path = (string)null,
                file_name = (string)null,
                create_account = (string)null,
                update_account = (string)null
            };

            var response = Send<TPResponse<PagedResponse<RfaResourceRecord>>>(HttpMethod.Post, "/Rfa_resource/MultiPagedSearch", search);
            EnsureResponse(response, "Rfa_resource/MultiPagedSearch");

            List<RfaResourceRecord> matches = response.Result.Datas
                .Where(x => x.RfaCode == rfaCode)
                .ToList();

            if (matches.Count == 0)
            {
                throw new InvalidOperationException("未找到族资源：" + rfaCode);
            }

            if (matches.Count > 1)
            {
                throw new InvalidOperationException("族资源不唯一：" + rfaCode);
            }

            return matches[0];
        }

        /// <summary>
        /// 下载族资源文件
        /// </summary>
        /// <param name="rfaPath">族资源OSS路径</param>
        /// <returns>族文件二进制内容</returns>
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
        /// 写入建模结果并更新任务状态
        /// </summary>
        /// <param name="task">建模任务</param>
        /// <param name="placements">族实例放置结果</param>
        /// <param name="modelingDoneStatus">建模完成状态码</param>
        public void InsertModelingResultsAndUpdateStatus(ModelingTask task, List<ModelingPlacementResult> placements, int modelingDoneStatus)
        {
            int sort = 1;
            foreach (ModelingPlacementResult placement in placements)
            {
                AddTaskResult(task, placement, sort);
                sort++;
            }

            UpdateTaskStatus(task.Id, modelingDoneStatus);
        }

        /// <summary>
        /// 新增单条建模任务结果
        /// </summary>
        /// <param name="task">建模任务</param>
        /// <param name="placement">族实例放置结果</param>
        /// <param name="sort">排序值</param>
        private void AddTaskResult(ModelingTask task, ModelingPlacementResult placement, int sort)
        {
            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var record = new TaskResultRecord
            {
                Id = Guid.NewGuid(),
                TaskBaseId = task.Id,
                Sort = sort,
                LayoutTitle = placement.FamilyName,
                RfaResourceId = placement.RfaResourceId,
                LayoutType = "放置",
                LocationX = Convert.ToDecimal(placement.LocationXMeters),
                LocationY = Convert.ToDecimal(placement.LocationYMeters),
                LocationZ = Convert.ToDecimal(placement.LocationZMeters),
                NormalX = 0,
                NormalY = 0,
                NormalZ = 1,
                RotateAngle = Convert.ToDecimal(placement.RotationAngleDegrees),
                CreateAccount = ClientName,
                CreateUsername = ClientName,
                CreateTime = now,
                UpdateAccount = ClientName,
                UpdateUsername = ClientName,
                UpdateTime = now,
                Remark = "Generated by Revit modeling plugin."
            };

            var response = Send<TPResponse<Res2Para>>(HttpMethod.Post, "/Task_result/Add", record);
            EnsureActionResponse(response, "Task_result/Add");
        }

        /// <summary>
        /// 更新建模任务状态
        /// </summary>
        /// <param name="taskId">建模任务Id</param>
        /// <param name="modelingDoneStatus">建模完成状态码</param>
        private void UpdateTaskStatus(Guid taskId, int modelingDoneStatus)
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
        /// <returns>反序列化后的响应</returns>
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
        /// <returns>反序列化后的响应</returns>
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
