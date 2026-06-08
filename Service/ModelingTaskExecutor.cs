using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace SiloModelingTaskClient
{
    /// <summary>
    /// 建模任务执行器。
    /// </summary>
    public class ModelingTaskExecutor
    {
        private readonly SiloTaskRepository _repository;
        private readonly int _modelingDoneStatus;
        private readonly SiloModelingService _modelingService;

        /// <summary>
        /// 初始化建模任务执行器。
        /// </summary>
        /// <param name="repository">后端接口仓储。</param>
        /// <param name="modelingDoneStatus">建模完成状态值。</param>
        public ModelingTaskExecutor(SiloTaskRepository repository, int modelingDoneStatus)
        {
            _repository = repository;
            _modelingDoneStatus = modelingDoneStatus;
            _modelingService = new SiloModelingService(repository);
        }

        /// <summary>
        /// 执行建模任务并更新任务状态。
        /// </summary>
        /// <param name="doc">Revit文档。</param>
        /// <param name="task">建模任务。</param>
        /// <param name="log">日志回调。</param>
        public void Execute(Document doc, ModelingTask task, Action<string> log)
        {
            List<ModelingPlacementResult> placements = _modelingService.Execute(doc, task, log);
            _repository.UpdateTaskStatus(task.Id, _modelingDoneStatus);

            log("任务已建模，任务状态已更新为：" + _modelingDoneStatus + "，任务：" + task.Id);
        }
    }
}
