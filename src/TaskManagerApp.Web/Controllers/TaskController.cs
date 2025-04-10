using Contract.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagerApp.Web.Controllers
{
 
    public class TaskController : Controller
    {
        #region  prop and ctor
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }
        #endregion

        #region  main method
        public IActionResult Index()
        {
            return View(_taskService.GetAll());
        }

        [HttpPost]
        public IActionResult Add(string name, string priority)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    _taskService.Add(new TaskItem { Name = name, Priority = priority });
                    TempData["Success"] = "Task added successfully ✅";
                }
                else
                {
                    TempData["Error"] = "Task name is required ❗";
                }
            }
            catch
            {
                TempData["Error"] = "Something went wrong while adding the task 😢";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Complete(Guid id)
        {
            try
            {
                _taskService.Complete(id);
                TempData["Success"] = "✅ Task marked as completed.";
            }
            catch
            {
                TempData["Error"] = "❌ Failed to complete the task.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _taskService.Delete(id);
                TempData["Success"] = "🗑 Task deleted successfully.";
            }
            catch
            {
                TempData["Error"] = "❌ Failed to delete the task.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Save()
        {
            try
            {
                _taskService.SaveTasks();
                TempData["Success"] = "💾 Tasks saved successfully.";
            }
            catch
            {
                TempData["Error"] = "❌ Failed to save tasks.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Load()
        {
            try
            {
                _taskService.LoadTasks();
                TempData["Success"] = "📂 Tasks loaded successfully.";
            }
            catch
            {
                TempData["Error"] = "❌ Failed to load tasks.";
            }

            return RedirectToAction("Index");
        }
        #endregion
    }
}
