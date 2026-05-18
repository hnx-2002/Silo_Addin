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

        public ModelingTaskExecutor(SiloTaskRepository repository, int modelingDoneStatus, string templateRootDir)
        {
            _repository = repository;
            _modelingDoneStatus = modelingDoneStatus;
            _modelingService = new SiloModelingService(repository, templateRootDir);
        }

        public void Execute(Document doc, ModelingTask task, Action<string> log)
        {
            List<TaskResultRecord> oldResults = _repository.GetTaskResults(task.Id);
            log("Task_result current count: " + oldResults.Count + ", task: " + task.Id);

            List<ModelingPlacementResult> placements = _modelingService.Execute(doc, task, log);
            _repository.InsertModelingResultsAndUpdateStatus(task, placements, _modelingDoneStatus);

            log("Task_result written and task_base.status updated to " + _modelingDoneStatus + ", task: " + task.Id);
        }
    }
}
