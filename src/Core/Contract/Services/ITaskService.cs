using System;
using Domain.Entities;

namespace Contract.Services;

public interface ITaskService
{
        IEnumerable<TaskItem> GetAll();
        void Add(TaskItem task);
        void Complete(Guid id);
        void Delete(Guid id);
        void SaveTasks();
        void LoadTasks();
}
