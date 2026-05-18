using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SiloModelingTaskClient
{
    public class RfaResourceApiClient
    {
        private readonly string _apiBaseUrl;
        private readonly string _coreApiBaseUrl;
        private readonly HttpClient _httpClient;

        public RfaResourceApiClient(string apiBaseUrl, string coreApiBaseUrl)
        {
            _apiBaseUrl = apiBaseUrl.TrimEnd('/');
            _coreApiBaseUrl = coreApiBaseUrl.TrimEnd('/');
            _httpClient = new HttpClient();
        }

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
                    throw new InvalidOperationException("UploadFile failed: " + response.StatusCode + " " + responseText);
                }

                var apiResponse = JsonConvert.DeserializeObject<TPResponse<ResUploadFile>>(responseText);
                EnsureResponse(apiResponse, "UploadFile/UploadFile");
                if (apiResponse.Result == null || !apiResponse.Result.Status)
                {
                    string message = apiResponse.Result == null ? apiResponse.Message : apiResponse.Result.Msg;
                    throw new InvalidOperationException("UploadFile/UploadFile failed: " + message);
                }

                return apiResponse.Result;
            }
        }

        public void AddRfaResource(RfaResourceRecord record)
        {
            var response = Send<TPResponse<Res2Para>>(HttpMethod.Post, "/Rfa_resource/Add", record);
            EnsureResponse(response, "Rfa_resource/Add");
            if (response.Result == null || !response.Result.Status)
            {
                string message = response.Result == null ? response.Message : response.Result.Message;
                throw new InvalidOperationException("Rfa_resource/Add failed: " + message);
            }
        }

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
    }
}
