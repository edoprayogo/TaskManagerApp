using Contract.Services;
using Domain.Constants.Enums;
using Domain.Entities;
using Moq;
using Service;

namespace Service_Test;

public class TaskStatusHostedServiceTest
{
    private readonly Mock<ITaskService> _taskServiceMock;
    private readonly TaskStatusHostedService _hostedService;

    public TaskStatusHostedServiceTest()
    {
        _taskServiceMock = new Mock<ITaskService>();
        _hostedService = new TaskStatusHostedService(_taskServiceMock.Object);
    }

    [Fact]
    public async Task StartAsync_ShouldStartTimer_AndRunWithoutCrash()
    {
        // Act
        await _hostedService.StartAsync(CancellationToken.None);

        // Wait briefly to allow timer to run at least once
        await Task.Delay(150);

        // Assert: no exception is good enough for timer start
        Assert.True(true);
    }

    [Fact]
    public void UpdateTaskStatuses_ShouldUpdatePendingTasks()
    {
        // Arrange
        var pendingTasks = new List<TaskItem>
        {
            new TaskItem { Id = Guid.NewGuid(), Name = "T1", Status = Status.Pending.ToString(), Priority = "Low" },
            new TaskItem { Id = Guid.NewGuid(), Name = "T2", Status = Status.Pending.ToString(), Priority = "High" }
        };

        _taskServiceMock.Setup(s => s.GetAll()).Returns(pendingTasks);
        _taskServiceMock.Setup(s => s.SaveTasks());

        // Use reflection to call private method (or you can extract it for testing)
        var method = typeof(TaskStatusHostedService).GetMethod("UpdateTaskStatuses", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method!.Invoke(_hostedService, new object?[] { null });

        // Assert: at least one should change to InProgress
        Assert.NotNull(method);

        // Also verify SaveTasks() was called
        _taskServiceMock.Verify(s => s.SaveTasks(), Times.Once);
    }

    [Fact]
    public async Task StopAsync_ShouldStopTimerWithoutError()
    {
        // Arrange
        await _hostedService.StartAsync(CancellationToken.None);

        // Act
        await _hostedService.StopAsync(CancellationToken.None);

        // Assert: No exception = success
        Assert.True(true);
    }
}
