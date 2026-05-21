using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 库型布置模板读取器
    /// </summary>
    public class PlacementTemplateLoader
    {
        private readonly string _templateRootDir;

        /// <summary>
        /// 初始化库型布置模板读取器
        /// </summary>
        /// <param name="templateRootDir">模板根目录</param>
        public PlacementTemplateLoader(string templateRootDir)
        {
            _templateRootDir = templateRootDir;
        }

        /// <summary>
        /// 根据最终库型读取族实例坐标模板
        /// </summary>
        /// <param name="finalSiloType">最终库型</param>
        /// <returns>族实例坐标模板记录</returns>
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
