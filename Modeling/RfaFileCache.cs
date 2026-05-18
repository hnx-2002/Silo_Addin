using System;
using System.Collections.Generic;
using System.IO;

namespace SiloModelingTaskClient
{
    public class RfaFileCache
    {
        private readonly SiloTaskRepository _repository;
        private readonly Dictionary<Guid, string> _localPaths = new Dictionary<Guid, string>();

        public RfaFileCache(SiloTaskRepository repository)
        {
            _repository = repository;
        }

        public string GetLocalPath(RfaResourceRecord resource)
        {
            if (_localPaths.ContainsKey(resource.Id))
            {
                return _localPaths[resource.Id];
            }

            byte[] bytes = _repository.DownloadRfaResource(resource.RfaPath);
            string dir = Path.Combine(Path.GetTempPath(), "SiloModelingTaskClient", "rfa");
            Directory.CreateDirectory(dir);

            string localPath = Path.Combine(dir, resource.Id + ".rfa");
            File.WriteAllBytes(localPath, bytes);
            _localPaths[resource.Id] = localPath;
            return localPath;
        }
    }
}
