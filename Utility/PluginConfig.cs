using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SiloModelingTaskClient
{
    public class PluginConfig
    {
        public string ApiBaseUrl { get; set; }
        public string CoreApiBaseUrl { get; set; }
        public string TemplateRootDir { get; set; }
        public string TemplateTp3Dir { get; set; }
        public int PollIntervalMilliseconds { get; set; }
        public int NewTaskStatus { get; set; }
        public int ModelingDoneStatus { get; set; }

        public static PluginConfig Load()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = Path.Combine(dir, "SiloModelingTaskClient.config");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("未找到插件配置文件：" + path);
            }

            var values = ReadKeyValues(path);
            return new PluginConfig
            {
                ApiBaseUrl = Require(values, "ApiBaseUrl").TrimEnd('/'),
                CoreApiBaseUrl = Require(values, "CoreApiBaseUrl").TrimEnd('/'),
                TemplateRootDir = Require(values, "TemplateRootDir"),
                TemplateTp3Dir = Require(values, "TemplateTp3Dir"),
                PollIntervalMilliseconds = int.Parse(Require(values, "PollIntervalMilliseconds")),
                NewTaskStatus = int.Parse(Require(values, "NewTaskStatus")),
                ModelingDoneStatus = int.Parse(Require(values, "ModelingDoneStatus"))
            };
        }

        private static Dictionary<string, string> ReadKeyValues(string path)
        {
            var res = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string rawLine in File.ReadAllLines(path))
            {
                string line = rawLine.Trim();
                if (line.Length == 0 || line.StartsWith("[") || line.StartsWith("#"))
                {
                    continue;
                }

                int index = line.IndexOf('=');
                if (index < 0)
                {
                    continue;
                }

                string key = line.Substring(0, index).Trim();
                string value = line.Substring(index + 1).Trim();
                res[key] = value;
            }

            return res;
        }

        private static string Require(Dictionary<string, string> values, string key)
        {
            if (!values.ContainsKey(key) || string.IsNullOrWhiteSpace(values[key]))
            {
                throw new InvalidOperationException("插件配置缺少：" + key);
            }

            return values[key];
        }
    }
}
