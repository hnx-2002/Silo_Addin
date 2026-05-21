using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 筒仓建模任务编排服务
    /// </summary>
    public class SiloModelingService
    {
        private readonly SiloTaskRepository _repository;
        private readonly SiloTypeResolver _siloTypeResolver;
        private readonly PlacementTemplateLoader _templateLoader;
        private readonly RfaResourceResolver _rfaResourceResolver;
        private readonly PlacementTransformCalculator _transformCalculator;
        private readonly RevitFamilyPlacementService _placementService;

        /// <summary>
        /// 初始化筒仓建模任务编排服务
        /// </summary>
        /// <param name="repository">后端接口仓储</param>
        /// <param name="templateRootDir">模板根目录</param>
        public SiloModelingService(SiloTaskRepository repository, string templateRootDir)
        {
            _repository = repository;
            _siloTypeResolver = new SiloTypeResolver();
            _templateLoader = new PlacementTemplateLoader(templateRootDir);
            _rfaResourceResolver = new RfaResourceResolver(repository);
            _transformCalculator = new PlacementTransformCalculator();
            _placementService = new RevitFamilyPlacementService(repository);
        }

        /// <summary>
        /// 执行单个筒仓建模任务
        /// </summary>
        /// <param name="doc">Revit文档</param>
        /// <param name="task">建模任务</param>
        /// <param name="log">日志输出方法</param>
        /// <returns>族实例放置结果</returns>
        public List<ModelingPlacementResult> Execute(Document doc, ModelingTask task, Action<string> log)
        {
            Guid dictSiloId = _siloTypeResolver.ResolveDictSiloId(task.SiloType);
            DictSiloRecord dictSilo = _repository.GetDictSilo(dictSiloId);
            string finalSiloType = _siloTypeResolver.ResolveTemplateKey(dictSilo.SiloType);
            log("模板库型：" + finalSiloType);

            List<PlacementTemplateRecord> templateRecords = _templateLoader.Load(finalSiloType);
            log("模板族实例数量：" + templateRecords.Count);

            Dictionary<string, RfaResourceRecord> resources = _rfaResourceResolver.Resolve(templateRecords);
            log("族资源数量：" + resources.Count);

            List<ModelingPlacementResult> placements = _transformCalculator.Calculate(templateRecords, task, resources);
            _placementService.Place(doc, placements, resources);
            log("族放置完成，实例数量：" + placements.Count);

            return placements;
        }
    }
}
