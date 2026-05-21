using System;
using System.Collections.Generic;
using System.IO;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 族文件本地缓存
    /// </summary>
    public class RfaFileCache
    {
        private readonly SiloTaskRepository _repository;
        private readonly Dictionary<Guid, string> _localPaths = new Dictionary<Guid, string>();

        /// <summary>
        /// 初始化族文件本地缓存
        /// </summary>
        /// <param name="repository">后端接口仓储</param>
        public RfaFileCache(SiloTaskRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 获取族资源对应的本地族文件路径
        /// </summary>
        /// <param name="resource">族资源记录</param>
        /// <returns>本地族文件路径</returns>
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
