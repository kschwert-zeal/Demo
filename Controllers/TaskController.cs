using Demo.Interfaces;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public IActionResult Index(string employee = "")
        {
            var viewModel = new DashboardViewModel
            {
                SelectedEmployee = employee,
                Employees = _taskService.GetAllEmployees(),
                Tasks = _taskService.GetTasksForEmployee(employee),
                Statistics = _taskService.GetStatistics(employee)
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateTask(TaskItem task)
        {
            if (ModelState.IsValid)
            {
                _taskService.AddTask(task);
                return RedirectToAction("Index", new { employee = task.AssignedTo });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateTasksStatus(int taskId, TasksStatus status)
        {
            var tasks = _taskService.GetTasksForEmployee("");
            var task = tasks.FirstOrDefault(t => t.Id == taskId);

            if (task != null)
            {
                task.Status = status;
                _taskService.UpdateTask(task);
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult DeleteTask(int taskId)
        {
            _taskService.DeleteTask(taskId);
            return Json(new { success = true });
        }
    }
}

