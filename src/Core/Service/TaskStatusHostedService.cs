using Microsoft.Extensions.Hosting;
using Contract.Services;
using Domain.Constants.Enums;

namespace Service
{
    public class TaskStatusHostedService : IHostedService, IDisposable
    {
        private readonly ITaskService _taskService;
        private Timer? _timer;
        private readonly Random _rand = new();

        public TaskStatusHostedService(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Start immediately and run every 10 seconds
            _timer = new Timer(UpdateTaskStatuses, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }

        private void UpdateTaskStatuses(object? state)
        {
            var tasks = _taskService.GetAll()
                .Where(t => t.Status == Status.Pending.ToString())
                .ToList();

            foreach (var task in tasks)
            {
                if (_rand.NextDouble() < 0.5)
                    task.Status = Status.InProgress.ToString();
            }

            _taskService.SaveTasks(); // Save changes to file
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
