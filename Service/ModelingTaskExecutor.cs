using System;
using System.Collections.Generic;

namespace SiloModelingTaskClient
{
    public class ModelingTaskExecutor
    {
        private readonly SiloTaskRepository _repository;
        private readonly int _modelingDoneStatus;

        public ModelingTaskExecutor(SiloTaskRepository repository, int modelingDoneStatus)
        {
            _repository = repository;
            _modelingDoneStatus = modelingDoneStatus;
        }

        public void Execute(ModelingTask task, Action<string> log)
        {
            List<TaskResultRecord> oldResults = _repository.GetTaskResults(task.Id);
            log("已读取task_result，任务：" + task.Id + "，现有结果数量：" + oldResults.Count);

            _repository.InsertModelingResultAndUpdateStatus(task, _modelingDoneStatus);
            log("已写入task_result，并将task_base.status更新为：" + _modelingDoneStatus + "，任务：" + task.Id);
        }
    }
}
