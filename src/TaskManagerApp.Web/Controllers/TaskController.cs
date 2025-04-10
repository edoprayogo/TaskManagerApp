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

        /// <summary>
        /// Displays the main view with a list of all tasks.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> that renders the view containing all tasks retrieved
        /// from the <see cref="_taskService"/>.
        /// </returns>
        public IActionResult Index()
        {
            return View(_taskService.GetAll());
        }

        /// <summary>
        /// Handles the addition of a new task via a POST request.
        /// </summary>
        /// <param name="name">The name of the task to be added.</param>
        /// <param name="priority">The priority level of the task.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> that redirects to the Index action,
        /// with success or error message stored in <see cref="TempData"/>.
        /// </returns>
        /// <remarks>
        /// If the task name is empty or whitespace, an error message will be set in <see cref="TempData"/>.
        /// If an exception occurs, a generic error message will be shown.
        /// </remarks>
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

        /// <summary>
        /// Marks a specific task as completed using its unique identifier.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> of the task to mark as completed.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> that redirects to the Index action,
        /// with a success or error message stored in <see cref="TempData"/>.
        /// </returns>
        /// <remarks>
        /// If the task completion fails (e.g., invalid ID or internal error), an error message is stored in <see cref="TempData"/>.
        /// </remarks>
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

        /// <summary>
        /// Deletes a specific task by its unique identifier.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> of the task to be deleted.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> that redirects to the Index action,
        /// with a success or error message stored in <see cref="TempData"/>.
        /// </returns>
        /// <remarks>
        /// If the deletion fails (e.g., invalid ID or internal error), an error message is stored in <see cref="TempData"/>.
        /// </remarks>
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

        /// <summary>
        /// Saves all current tasks to a persistent storage (e.g., JSON file).
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> that redirects to the Index action,
        /// with a success or error message stored in <see cref="TempData"/>.
        /// </returns>
        /// <remarks>
        /// If the save operation fails due to an exception, an error message is stored in <see cref="TempData"/>.
        /// </remarks>
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

        /// <summary>
        /// Loads tasks from persistent storage (e.g., a JSON file) into the application.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> that redirects to the Index action,
        /// with a success or error message stored in <see cref="TempData"/>.
        /// </returns>
        /// <remarks>
        /// If the load operation fails due to an exception (e.g., file not found or corrupted),
        /// an error message is stored in <see cref="TempData"/>.
        /// </remarks>
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
