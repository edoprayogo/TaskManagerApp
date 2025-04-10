using System.Text.Json;
using Contract.Services;
using Domain.Constants.Enums;
using Domain.Entities;

namespace Service;

public class TaskService : ITaskService
{
    #region prop and ctor
    private readonly string _filePath = Path.Combine(
        Directory
            .GetParent(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName)!
            .FullName,
        "taskdata.json"
    );
    private readonly List<TaskItem> _tasks = new();
    //private readonly Timer _backgroundTimer;

    public TaskService()
    {
        LoadTasks();
       // _backgroundTimer = new Timer(UpdateTaskStatus, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
    }
    #endregion

    #region main method
    public IEnumerable<TaskItem> GetAll() => _tasks;
    public void Add(TaskItem task)
    {
        task.Id = Guid.NewGuid();
        task.Status = Status.Pending.ToString();
        _tasks.Add(task);
        SaveTasks();
    }
    public void Complete(Guid id)
    {
        try
        {
            if (!File.Exists(_filePath)) return;

            var json = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(json)) return;

            var tasks = JsonSerializer.Deserialize<List<TaskItem>>(json);
            if (tasks == null) return;

            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                task.Status = Status.Completed.ToString();

                File.WriteAllText(_filePath, JsonSerializer.Serialize(tasks, new JsonSerializerOptions
                {
                    WriteIndented = true
                }));

                // Update in-memory list too
                _tasks.Clear();
                _tasks.AddRange(tasks);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Failed to complete task in JSON: " + ex.Message);
        }
    }

    public void Delete(Guid id)
    {
        try
        {
            if (!File.Exists(_filePath)) return;

            var json = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(json)) return;

            var tasks = JsonSerializer.Deserialize<List<TaskItem>>(json);
            if (tasks == null) return;

            var updatedTasks = tasks.Where(t => t.Id != id).ToList();

            File.WriteAllText(_filePath, JsonSerializer.Serialize(updatedTasks, new JsonSerializerOptions
            {
                WriteIndented = true
            }));

            // Optionally update in-memory list too
            _tasks.Clear();
            _tasks.AddRange(updatedTasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Failed to delete task from JSON: " + ex.Message);
        }
    }

    public void SaveTasks()
    {
        try
        {
            var json = JsonSerializer.Serialize(_tasks, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Error saving tasks: " + ex.Message);
        }
    }

    public void LoadTasks()
    {
            try
            {
                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        var loaded = JsonSerializer.Deserialize<List<TaskItem>>(json);
                        if (loaded != null)
                        {
                            _tasks.Clear();
                            _tasks.AddRange(loaded);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error loading tasks: " + ex.Message);
            }
    }

    // private void UpdateTaskStatus(object? state)
    // {
    //     var pending = _tasks.Where(t => t.Status == Status.Pending.ToString()).ToList();
    //     var rand = new Random();

    //     foreach (var task in pending)
    //     {
    //         if (rand.NextDouble() < 0.5)
    //             task.Status = "In Progress";
    //     }
    // }
    #endregion
}
