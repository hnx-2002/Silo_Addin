using System.Collections.Generic;

namespace SiloModelingTaskClient
{
    public class RfaResourceResolver
    {
        private readonly SiloTaskRepository _repository;

        public RfaResourceResolver(SiloTaskRepository repository)
        {
            _repository = repository;
        }

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
