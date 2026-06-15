using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using System.Windows.Forms;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 插件鉴权HTTP工具。
    /// </summary>
    internal static class FunHttp
    {
        /// <summary>
        /// 添加鉴权通用HTTP参数。
        /// </summary>
        /// <param name="request">REST请求。</param>
        /// <param name="funcMsg">功能说明。</param>
        private static void AddCommonHttpParameters(RestRequest request, string funcMsg)
        {
            string token = FunCommon.ReadToken();
            request.AddCookie("tp_token", token);
            request.AddQueryParameter("funcMsg", funcMsg);
            request.AddQueryParameter("version", Config.Rev);
        }

        /// <summary>
        /// 判断主窗体是否可用。
        /// </summary>
        /// <returns>主窗体可用时返回true，否则返回false。</returns>
        public static bool FormAuth()
        {
            try
            {
                string url = Config.APIUrl.TrimEnd('/') + "/" + Config.ToolCode + "/Form/FormAuth";
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                AddCommonHttpParameters(request, "窗口鉴权");
                IRestResponse response = client.Execute(request);
                var result = JsonConvert.DeserializeObject<TPResponse<bool>>(response.Content);

                MessageBox.Show("response=" + response.StatusCode + "\r\n" + response.Content);
                MessageBox.Show(result.Result.ToString());

                return result.Result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 判断当前用户是否为管理员。
        /// </summary>
        /// <returns>当前用户是管理员时返回true。</returns>
        public static bool IsAdmin()
        {
            string url = Config.APIUrl.TrimEnd('/') + "/" + Config.ToolCode + "/Form/IsAdmin";
            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            AddCommonHttpParameters(request, "管理员鉴权");
            IRestResponse response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException("管理员鉴权请求失败，状态码：" + response.StatusCode + "，内容：" + response.Content);
            }

            var result = JsonConvert.DeserializeObject<TPResponse<bool>>(response.Content);

            return result.Result;
        }
    }
}
