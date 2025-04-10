using Microsoft.Extensions.Hosting;
using Contract.Services;
using Domain.Constants.Enums;

namespace Service
{
    /// <summary>
    /// A hosted background service that periodically updates the statuses of pending tasks.
    /// </summary>
    public class TaskStatusHostedService : IHostedService, IDisposable
    {
        private readonly ITaskService _taskService;
        private Timer? _timer;
        private readonly Random _rand = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskStatusHostedService"/> class.
        /// </summary>
        /// <param name="taskService">The task service used to access and update tasks.</param>
        public TaskStatusHostedService(ITaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// Starts the background task when the application starts.
        /// Runs <see cref="UpdateTaskStatuses"/> every 10 seconds.
        /// </summary>
        /// <param name="cancellationToken">A token to signal cancellation.</param>
        /// <returns>A completed <see cref="Task"/>.</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Start immediately and run every 10 seconds
            _timer = new Timer(UpdateTaskStatuses, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates the statuses of all tasks currently marked as "Pending".
        /// Has a 50% chance to mark each pending task as "InProgress".
        /// </summary>
        /// <param name="state">Optional state object, unused.</param>
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

        /// <summary>
        /// Stops the background task when the application is shutting down.
        /// </summary>
        /// <param name="cancellationToken">A token to signal cancellation.</param>
        /// <returns>A completed <see cref="Task"/>.</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Disposes the timer used for scheduling the background task.
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
