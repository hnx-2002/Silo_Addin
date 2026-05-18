using System;

namespace SiloModelingTaskClient
{
    public class SiloTypeResolver
    {
        public Guid ResolveDictSiloId(string taskSiloType)
        {
            if (string.IsNullOrWhiteSpace(taskSiloType))
            {
                throw new InvalidOperationException("Modeling task silo_type is empty.");
            }

            Guid dictSiloId;
            if (!Guid.TryParse(taskSiloType, out dictSiloId))
            {
                throw new InvalidOperationException("Modeling task silo_type is not a dict_silo id: " + taskSiloType);
            }

            return dictSiloId;
        }

        public string ResolveTemplateKey(string dictSiloType)
        {
            if (string.IsNullOrWhiteSpace(dictSiloType))
            {
                throw new InvalidOperationException("Dict_silo silo_type is empty.");
            }

            string[] parts = dictSiloType.Split('_');
            return parts[0];
        }
    }
}
