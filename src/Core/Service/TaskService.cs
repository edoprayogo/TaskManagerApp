using System.Text.Json;
using Contract.Services;
using Domain.Constants.Enums;
using Domain.Entities;

namespace Service;

/// <summary>
/// Provides task management functionality including creating, updating, deleting,
/// saving, and loading tasks using a JSON file for persistence.
/// </summary>
public class TaskService : ITaskService
{
    #region prop and ctor

    /// <summary>
    /// The file path where tasks are persisted as JSON.
    /// </summary>
    private readonly string _filePath = Path.Combine(
        Directory
            .GetParent(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName)!
            .FullName,
        "taskdata.json"
    );

    /// <summary>
    /// In-memory list of task items.
    /// </summary>
    private readonly List<TaskItem> _tasks = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskService"/> class and loads existing tasks.
    /// </summary>
    public TaskService()
    {
        LoadTasks();
    }
    #endregion

    #region main method


    /// <summary>
    /// Retrieves all tasks currently in memory.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{TaskItem}"/> containing all tasks.</returns>
    public IEnumerable<TaskItem> GetAll() => _tasks;

    /// <summary>
    /// Adds a new task with a unique ID and default status of Pending.
    /// Automatically saves the updated task list to persistent storage.
    /// </summary>
    /// <param name="task">The task to add.</param>
    public void Add(TaskItem task)
    {
        task.Id = Guid.NewGuid();
        task.Status = Status.Pending.ToString();
        _tasks.Add(task);
        SaveTasks();
    }
    
    /// <summary>
    /// Marks a task as completed based on its unique identifier.
    /// Updates both the file and in-memory task list.
    /// </summary>
    /// <param name="id">The <see cref="Guid"/> of the task to complete.</param>
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

    /// <summary>
    /// Deletes a task from the list and persistent storage based on its ID.
    /// Updates the in-memory task list as well.
    /// </summary>
    /// <param name="id">The <see cref="Guid"/> of the task to delete.</param>
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

    /// <summary>
    /// Saves the current in-memory list of tasks to a JSON file.
    /// </summary>
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

    /// <summary>
    /// Loads tasks from the JSON file into memory if the file exists.
    /// </summary>
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

    #endregion
}
