using System;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 库型信息解析器
    /// </summary>
    public class SiloTypeResolver
    {
        /// <summary>
        /// 从任务库型字段解析库型字典Id
        /// </summary>
        /// <param name="taskSiloType">任务库型字段</param>
        /// <returns>库型字典Id</returns>
        public Guid ResolveDictSiloId(string taskSiloType)
        {
            if (string.IsNullOrWhiteSpace(taskSiloType))
            {
                throw new InvalidOperationException("建模任务的库型字段为空。");
            }

            Guid dictSiloId;
            if (!Guid.TryParse(taskSiloType, out dictSiloId))
            {
                throw new InvalidOperationException("建模任务的库型字段不是库型字典Id：" + taskSiloType);
            }

            return dictSiloId;
        }

        /// <summary>
        /// 从库型字典的库型字段解析模板目录名称
        /// </summary>
        /// <param name="dictSiloType">库型字段</param>
        /// <returns>模板目录名称</returns>
        public string ResolveTemplateKey(string dictSiloType)
        {
            if (string.IsNullOrWhiteSpace(dictSiloType))
            {
                throw new InvalidOperationException("库型字典中的库型字段为空。");
            }

            string[] parts = dictSiloType.Split('_');
            return parts[0];
        }
    }
}
