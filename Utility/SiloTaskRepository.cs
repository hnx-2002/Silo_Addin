using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace SiloModelingTaskClient
{
    public class SiloTaskRepository
    {
        private readonly string _apiBaseUrl;
        private readonly HttpClient _httpClient;

        public SiloTaskRepository(string apiBaseUrl)
        {
            _apiBaseUrl = apiBaseUrl.TrimEnd('/');
            _httpClient = new HttpClient();
        }

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

        public List<TaskResultRecord> GetTaskResults(Guid taskBaseId)
        {
            var response = Get<TPResponse<List<TaskResultRecord>>>("/Task_result/Search_Task_base_id/" + taskBaseId);
            EnsureResponse(response, "Task_result/Search_Task_base_id");
            return response.Result;
        }

        public void InsertModelingResultAndUpdateStatus(ModelingTask task, int modelingDoneStatus)
        {
            AddTaskResult(task);
            UpdateTaskStatus(task.Id, modelingDoneStatus);
        }

        private void AddTaskResult(ModelingTask task)
        {
            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var record = new TaskResultRecord
            {
                Id = Guid.NewGuid(),
                TaskBaseId = task.Id,
                Sort = 1,
                LayoutTitle = "插件框架建模结果",
                RfaResourceId = Guid.Empty,
                LayoutType = "框架",
                LocationX = task.TaskX,
                LocationY = task.TaskY,
                LocationZ = task.TaskZ,
                NormalX = 0,
                NormalY = 0,
                NormalZ = 1,
                RotateAngle = task.RotationAngle,
                CreateAccount = "SiloModelingTaskClient",
                CreateUsername = "SiloModelingTaskClient",
                CreateTime = now,
                UpdateAccount = "SiloModelingTaskClient",
                UpdateUsername = "SiloModelingTaskClient",
                UpdateTime = now,
                Remark = "当前版本仅搭建监听和结果写入框架，未在Revit中实际建模。"
            };

            var response = Send<TPResponse<Res2Para>>(HttpMethod.Post, "/Task_result/Add", record);
            EnsureActionResponse(response, "Task_result/Add");
        }

        private void UpdateTaskStatus(Guid taskId, int modelingDoneStatus)
        {
            var getResponse = Get<TPResponse<ModelingTask>>("/Task_base/Get/" + taskId);
            EnsureResponse(getResponse, "Task_base/Get");

            ModelingTask task = getResponse.Result;
            task.Status = modelingDoneStatus;
            task.UpdateAccount = "SiloModelingTaskClient";
            task.UpdateUsername = "SiloModelingTaskClient";
            task.UpdateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            var updateResponse = Send<TPResponse<Res2Para>>(HttpMethod.Put, "/Task_base/Update", task);
            EnsureActionResponse(updateResponse, "Task_base/Update");
        }

        private T Get<T>(string path)
        {
            return Send<T>(HttpMethod.Get, path, null);
        }

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
                    throw new InvalidOperationException(method.Method + " " + url + " failed: " + response.StatusCode + " " + content);
                }

                return JsonConvert.DeserializeObject<T>(content);
            }
        }

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
