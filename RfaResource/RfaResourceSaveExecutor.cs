using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace SiloModelingTaskClient
{
    public class RfaResourceSaveExecutor
    {
        private readonly RfaFamilyCollector _collector;
        private readonly RfaFamilyFileExporter _exporter;
        private readonly RfaInstanceCoordinateExporter _coordinateExporter;
        private readonly RfaResourceApiClient _apiClient;

        public RfaResourceSaveExecutor(RfaResourceApiClient apiClient, string coordinateOutputDir)
        {
            _collector = new RfaFamilyCollector();
            _exporter = new RfaFamilyFileExporter();
            _coordinateExporter = new RfaInstanceCoordinateExporter(coordinateOutputDir);
            _apiClient = apiClient;
        }

        public void Execute(Document doc, View activeView, Action<string> log)
        {
            List<FamilyInstance> instances = _collector.CollectAllowedInstancesFromActive3DView(doc, activeView);
            string coordinateJsonPath = _coordinateExporter.Export(instances);
            log("族实例坐标JSON已写入：" + coordinateJsonPath);

            List<RfaFamilyExportItem> families = _collector.CollectFromActive3DView(doc, activeView);
            log("当前三维视图目标族数量：" + families.Count);

            foreach (RfaFamilyExportItem family in families)
            {
                log("开始保存族：" + family.FamilyName);
                RfaFileData file = _exporter.Export(doc, family);
                ResUploadFile upload = _apiClient.UploadRfa(file);

                long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                var record = new RfaResourceRecord
                {
                    Id = Guid.NewGuid(),
                    RfaCode = family.FamilyName,
                    SymbolName = family.SymbolName,
                    RfaPath = upload.FilePath,
                    FileName = file.FileName,
                    FileSize = file.Bytes.Length,
                    Note = "插件从当前三维视图保存族资源",
                    CreateAccount = "SiloModelingTaskClient",
                    CreateUsername = "SiloModelingTaskClient",
                    CreateTime = now,
                    UpdateAccount = "SiloModelingTaskClient",
                    UpdateUsername = "SiloModelingTaskClient",
                    UpdateTime = now,
                    Remark = "OSS路径：" + upload.FilePath
                };

                _apiClient.AddRfaResource(record);
                log("族资源已保存：" + family.FamilyName + "，OSS：" + upload.FilePath);
            }
        }
    }
}
