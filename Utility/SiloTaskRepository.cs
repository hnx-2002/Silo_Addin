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
        private const string ClientName = "SiloModelingTaskClient";

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

        public DictSiloRecord GetDictSilo(Guid id)
        {
            var response = Get<TPResponse<DictSiloRecord>>("/Dict_silo/Get/" + id);
            EnsureResponse(response, "Dict_silo/Get");
            if (response.Result == null)
            {
                throw new InvalidOperationException("Dict_silo not found: " + id);
            }

            return response.Result;
        }

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
                throw new InvalidOperationException("Rfa resource not found: " + rfaCode);
            }

            if (matches.Count > 1)
            {
                throw new InvalidOperationException("Rfa resource is not unique: " + rfaCode);
            }

            return matches[0];
        }

        public byte[] DownloadRfaResource(string rfaPath)
        {
            if (string.IsNullOrWhiteSpace(rfaPath))
            {
                throw new InvalidOperationException("Rfa path is empty.");
            }

            string url = _apiBaseUrl + "/Rfa_resource/Download" + rfaPath;
            HttpResponseMessage response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
            byte[] bytes = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                string content = Encoding.UTF8.GetString(bytes);
                throw new InvalidOperationException("GET " + url + " failed: " + response.StatusCode + " " + content);
            }

            return bytes;
        }

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
                LayoutType = placement.SymbolName,
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
