using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 族资源保存执行器
    /// </summary>
    public class RfaResourceSaveExecutor
    {
        private readonly RfaFamilyCollector _collector;
        private readonly RfaFamilyFileExporter _exporter;
        private readonly RfaInstanceCoordinateExporter _coordinateExporter;
        private readonly RfaResourceApiClient _apiClient;

        /// <summary>
        /// 初始化族资源保存执行器
        /// </summary>
        /// <param name="apiClient">族资源后端接口客户端</param>
        /// <param name="coordinateOutputDir">坐标JSON输出目录</param>
        public RfaResourceSaveExecutor(RfaResourceApiClient apiClient, string coordinateOutputDir)
        {
            _collector = new RfaFamilyCollector();
            _exporter = new RfaFamilyFileExporter();
            _coordinateExporter = new RfaInstanceCoordinateExporter(coordinateOutputDir);
            _apiClient = apiClient;
        }

        /// <summary>
        /// 保存当前三维视图中的目标族资源并导出族实例坐标JSON
        /// </summary>
        /// <param name="doc">Revit文档</param>
        /// <param name="activeView">当前视图</param>
        /// <param name="log">日志输出方法</param>
        public void Execute(Document doc, View activeView, Action<string> log)
        {
            SetActive3DViewDetailLevel(doc, activeView);
            log("当前三维视图已设置为详细视图。");

            List<FamilyInstance> instances = _collector.CollectAllowedInstancesFromActive3DView(doc, activeView);
            string coordinateJsonPath = _coordinateExporter.Export(instances);
            log("族实例坐标文件已写入：" + coordinateJsonPath);

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
                log("族资源已保存：" + family.FamilyName + "，文件地址：" + upload.FilePath);
            }
        }

        /// <summary>
        /// 将当前三维视图设置为详细视图
        /// </summary>
        /// <param name="doc">Revit文档</param>
        /// <param name="activeView">当前视图</param>
        private void SetActive3DViewDetailLevel(Document doc, View activeView)
        {
            if (!(activeView is View3D))
            {
                throw new InvalidOperationException("当前视图不是三维视图，不能设置详细视图。");
            }

            using (var transaction = new Transaction(doc, "设置三维视图详细程度"))
            {
                transaction.Start();
                activeView.DetailLevel = ViewDetailLevel.Fine;
                transaction.Commit();
            }
        }
    }
}
