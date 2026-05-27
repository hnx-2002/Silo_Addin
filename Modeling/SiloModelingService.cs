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
        private readonly RevitFamilyPlacementService _placementService;

        /// <summary>
        /// 初始化筒仓建模任务编排服务
        /// </summary>
        /// <param name="repository">后端接口仓储</param>
        public SiloModelingService(SiloTaskRepository repository)
        {
            _repository = repository;
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
            List<ModelingPlacementResult> placements = _repository.CalculateTemplatePlacements(task.Id);
            log("后端计算结果数量：" + placements.Count);

            _placementService.Place(doc, task.Id, placements);
            log("族放置完成，实例数量：" + placements.Count);

            return placements;
        }
    }
}
