using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// RFA资源保存执行器。
    /// </summary>
    public class TemplateSiloSaveExecutor
    {
        private const string ClientName = "SiloModelingTaskClient";

        private readonly TemplateSiloApiClient _apiClient;
        private readonly Guid _dictSiloId;
        private readonly string _dictSiloName;
        private readonly RfaFamilyCollector _collector;
        private readonly RfaFamilyFileExporter _exporter;

        /// <summary>
        /// 初始化RFA资源保存执行器。
        /// </summary>
        /// <param name="apiClient">RFA资源后端接口客户端。</param>
        /// <param name="dictSiloId">库型Id。</param>
        /// <param name="dictSiloName">库型名称。</param>
        public TemplateSiloSaveExecutor(TemplateSiloApiClient apiClient, Guid dictSiloId, string dictSiloName)
        {
            _apiClient = apiClient;
            _dictSiloId = dictSiloId;
            _dictSiloName = dictSiloName;
            _collector = new RfaFamilyCollector();
            _exporter = new RfaFamilyFileExporter();
        }

        /// <summary>
        /// 保存当前三维视图中的RFA资源和模板点。
        /// </summary>
        /// <param name="doc">Revit文档。</param>
        /// <param name="activeView">当前视图。</param>
        /// <param name="log">日志输出方法。</param>
        public void Execute(Document doc, View activeView, Action<string> log)
        {
            SetActive3DViewDetailLevel(doc, activeView);
            log("当前三维视图已设置为详细视图。");

            List<FamilyInstance> instances = _collector.CollectAllowedInstancesFromActive3DView(doc, activeView);
            log("当前三维视图目标族实例数量：" + instances.Count);

            _apiClient.DeleteRfaResourcesByDictSiloId(_dictSiloId);
            log("已删除当前库型旧族资源：" + _dictSiloName);

            var rfaPaths = new Dictionary<int, string>();
            var rfaFiles = new Dictionary<int, RfaFileData>();
            List<RfaFamilyExportItem> families = instances
                .GroupBy(x => x.Symbol.Family.Id.IntegerValue)
                .Select(g =>
                {
                    FamilyInstance instance = g.First();
                    return new RfaFamilyExportItem
                    {
                        Instance = instance,
                        FamilyName = instance.Symbol.Family.Name,
                        SymbolName = instance.Symbol.Name
                    };
                })
                .OrderBy(x => x.FamilyName)
                .ToList();

            foreach (RfaFamilyExportItem family in families)
            {
                log("开始上传族：" + family.FamilyName);
                RfaFileData file = _exporter.Export(doc, family);
                ResUploadFile upload = _apiClient.UploadRfa(file);
                int familyId = family.Instance.Symbol.Family.Id.IntegerValue;
                rfaPaths[familyId] = upload.FilePath;
                rfaFiles[familyId] = file;
                log("族文件已上传：" + family.FamilyName + "，文件地址：" + upload.FilePath);
            }

            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            int recordCount = 0;
            foreach (FamilyInstance instance in instances)
            {
                if (!(instance.Location is LocationPoint locationPoint))
                {
                    throw new InvalidOperationException("族实例不是点定位实例，无法读取点坐标。ElementId：" + instance.Id.IntegerValue);
                }

                int familyId = instance.Symbol.Family.Id.IntegerValue;
                XYZ point = locationPoint.Point;
                RfaFileData file = rfaFiles[familyId];
                var record = new RfaResourceRecord
                {
                    Id = Guid.NewGuid(),
                    DictSiloId = _dictSiloId,
                    SymbolName = instance.Symbol.Name,
                    RfaPath = rfaPaths[familyId],
                    FileName = file.FileName,
                    FileSize = file.Bytes.Length,
                    Note = string.Empty,
                    TemplateX = Convert.ToDecimal(point.X),
                    TemplateY = Convert.ToDecimal(point.Y),
                    TemplateZ = Convert.ToDecimal(point.Z),
                    CreateAccount = ClientName,
                    CreateUsername = ClientName,
                    CreateTime = now,
                    UpdateAccount = ClientName,
                    UpdateUsername = ClientName,
                    UpdateTime = now,
                    Remark = "Saved by Revit modeling plugin."
                };

                _apiClient.AddRfaResource(record);
                recordCount++;
            }

            log("族资源已保存：" + _dictSiloName + "，记录数量：" + recordCount);
        }

        /// <summary>
        /// 将当前三维视图设置为详细视图。
        /// </summary>
        /// <param name="doc">Revit文档。</param>
        /// <param name="activeView">当前视图。</param>
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
