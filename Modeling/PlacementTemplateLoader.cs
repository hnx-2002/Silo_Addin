using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace SiloModelingTaskClient
{
    public class PlacementTemplateLoader
    {
        private readonly string _templateRootDir;

        public PlacementTemplateLoader(string templateRootDir)
        {
            _templateRootDir = templateRootDir;
        }

        public List<PlacementTemplateRecord> Load(string finalSiloType)
        {
            string templatePath = Path.Combine(_templateRootDir, finalSiloType, "rfa_instance_coordinates.json");
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException("未找到库型模板JSON：" + templatePath);
            }

            string json = File.ReadAllText(templatePath);
            var records = JsonConvert.DeserializeObject<List<PlacementTemplateRecord>>(json);
            if (records == null)
            {
                throw new InvalidOperationException("库型模板JSON解析失败：" + templatePath);
            }

            return records;
        }
    }
}
