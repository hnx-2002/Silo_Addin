using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace SiloModelingTaskClient
{
    public class ModelingTaskExecutor
    {
        private readonly SiloTaskRepository _repository;
        private readonly int _modelingDoneStatus;
        private readonly SiloModelingService _modelingService;

        public ModelingTaskExecutor(SiloTaskRepository repository, int modelingDoneStatus)
        {
            _repository = repository;
            _modelingDoneStatus = modelingDoneStatus;
            _modelingService = new SiloModelingService(repository);
        }

        public void Execute(Document doc, ModelingTask task, Action<string> log)
        {
            List<TaskResultRecord> oldResults = _repository.GetTaskResults(task.Id);
            log("已读取任务结果，当前数量：" + oldResults.Count + "，任务：" + task.Id);

            List<ModelingPlacementResult> placements = _modelingService.Execute(doc, task, log);
            _repository.InsertModelingResultsAndUpdateStatus(task, placements, _modelingDoneStatus);

            log("任务结果已写入，任务状态已更新为：" + _modelingDoneStatus + "，任务：" + task.Id);
        }
    }
}
