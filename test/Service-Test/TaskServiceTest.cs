using System;
using Domain.Entities;
using Service;

namespace Service_Test;

public class TaskServiceTest
{
    private readonly string _testFilePath;
    private readonly TaskService _taskService;

    public TaskServiceTest()
    {
        // Setup: use temp file to avoid real file pollution
        _testFilePath = Path.GetTempFileName();

        // Inject into a derived testable TaskService
        _taskService = new TestableTaskService(_testFilePath);
    }

    [Fact]
    public void Add_ShouldAddTask_AndSaveToFile()
    {
        // Arrange
        var task = new TaskItem { Name = "Test Task", Priority = "High" };

        // Act
        _taskService.Add(task);
        var tasks = _taskService.GetAll();

        // Assert
        Assert.Single(tasks);
        Assert.Equal("Test Task", tasks.First().Name);
        Assert.Equal("Pending", tasks.First().Status);
    }

    [Fact]
    public void Complete_ShouldMarkTaskAsCompleted()
    {
        var task = new TaskItem { Name = "Complete Me", Priority = "Low" };
        _taskService.Add(task);
        var taskId = _taskService.GetAll().First().Id;

        // Act
        _taskService.Complete(taskId);
        var updated = _taskService.GetAll().First(t => t.Id == taskId);

        // Assert
        Assert.Equal("Completed", updated.Status);
    }

    [Fact]
    public void Delete_ShouldRemoveTaskFromList()
    {
        var task = new TaskItem { Name = "Delete Me", Priority = "Low" };
        _taskService.Add(task);
        var taskId = _taskService.GetAll().First().Id;

        // Act
        _taskService.Delete(taskId);
        var tasks = _taskService.GetAll();

        // Assert
        Assert.Empty(tasks);
    }

    public void Dispose()
    {
        // Cleanup temp file
        if (File.Exists(_testFilePath))
            File.Delete(_testFilePath);
    }

    /// <summary>
    /// A test-friendly version of TaskService that lets us inject custom file path
    /// </summary>
    private class TestableTaskService : TaskService
    {
        public TestableTaskService(string path)
        {
            typeof(TaskService)
                .GetField("_filePath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .SetValue(this, path);

            LoadTasks(); // load from temp file
        }
    }
}
