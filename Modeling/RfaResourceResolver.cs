using System.Collections.Generic;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 族资源解析器
    /// </summary>
    public class RfaResourceResolver
    {
        private readonly SiloTaskRepository _repository;

        /// <summary>
        /// 初始化族资源解析器
        /// </summary>
        /// <param name="repository">后端接口仓储</param>
        public RfaResourceResolver(SiloTaskRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 根据模板中的族名称查询对应族资源
        /// </summary>
        /// <param name="templateRecords">族实例坐标模板记录</param>
        /// <returns>族名称与族资源记录的映射</returns>
        public Dictionary<string, RfaResourceRecord> Resolve(List<PlacementTemplateRecord> templateRecords)
        {
            var result = new Dictionary<string, RfaResourceRecord>();
            foreach (PlacementTemplateRecord record in templateRecords)
            {
                if (!result.ContainsKey(record.FamilyName))
                {
                    result[record.FamilyName] = _repository.GetRfaResourceByCode(record.FamilyName);
                }
            }

            return result;
        }
    }
}
