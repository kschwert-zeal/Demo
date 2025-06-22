using Demo.Interfaces;
using Demo.Models;

namespace Demo.Services
{
    public class TaskService : ITaskService
    {
        private static List<TaskItem> _tasks = GenerateSampleTasks();

        public List<TaskItem> GetTasksForEmployee(string employeeName)
        {
            if (string.IsNullOrEmpty(employeeName))
                return _tasks;

            return _tasks.Where(t => t.AssignedTo.Equals(employeeName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<string> GetAllEmployees()
        {
            return _tasks.Select(t => t.AssignedTo).Distinct().OrderBy(e => e).ToList();
        }

        public TaskStatistics GetStatistics(string employeeName)
        {
            var tasks = GetTasksForEmployee(employeeName);
            var now = DateTime.Now;

            var stats = new TaskStatistics
            {
                TotalTasks = tasks.Count,
                CompletedTasks = tasks.Count(t => t.Status == TasksStatus.Completed),
                InProgressTasks = tasks.Count(t => t.Status == TasksStatus.InProgress),
                OverdueTasks = tasks.Count(t => t.DueDate.HasValue && t.DueDate < now && t.Status != TasksStatus.Completed)
            };

            // Status breakdown
            foreach (TasksStatus status in Enum.GetValues<TasksStatus>())
            {
                stats.StatusBreakdown[status] = tasks.Count(t => t.Status == status);
            }

            // Priority breakdown
            foreach (TaskPriority priority in Enum.GetValues<TaskPriority>())
            {
                stats.PriorityBreakdown[priority] = tasks.Count(t => t.Priority == priority);
            }

            // Monthly data for the last 6 months
            for (int i = 5; i >= 0; i--)
            {
                var month = now.AddMonths(-i);
                var monthTasks = tasks.Where(t => t.CreatedDate.Year == month.Year && t.CreatedDate.Month == month.Month);
                var completedInMonth = tasks.Where(t => t.CompletedDate.HasValue &&
                    t.CompletedDate.Value.Year == month.Year && t.CompletedDate.Value.Month == month.Month);

                stats.MonthlyData.Add(new MonthlyTaskData
                {
                    Month = month.ToString("MMM yyyy"),
                    Created = monthTasks.Count(),
                    Completed = completedInMonth.Count()
                });
            }

            return stats;
        }

        public void AddTask(TaskItem task)
        {
            task.Id = _tasks.Any() ? _tasks.Max(t => t.Id) + 1 : 1;
            _tasks.Add(task);
        }

        public void UpdateTask(TaskItem task)
        {
            var existingTask = _tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existingTask != null)
            {
                existingTask.Title = task.Title;
                existingTask.Description = task.Description;
                existingTask.Status = task.Status;
                existingTask.Priority = task.Priority;
                existingTask.DueDate = task.DueDate;
                existingTask.EstimatedHours = task.EstimatedHours;
                existingTask.ActualHours = task.ActualHours;

                if (task.Status == TasksStatus.Completed && existingTask.CompletedDate == null)
                {
                    existingTask.CompletedDate = DateTime.Now;
                }
            }
        }

        public void DeleteTask(int taskId)
        {
            _tasks.RemoveAll(t => t.Id == taskId);
        }

        private static List<TaskItem> GenerateSampleTasks()
        {
            var random = new Random();
            var employees = new[] { "John Smith", "Sarah Johnson", "Mike Davis", "Emily Wilson", "David Brown" };
            var taskTitles = new[]
            {
                "Update user authentication system",
                "Fix bug in payment processing",
                "Create API documentation",
                "Implement new dashboard features",
                "Code review for mobile app",
                "Database performance optimization",
                "Setup CI/CD pipeline",
                "Write unit tests",
                "Security vulnerability assessment",
                "UI/UX improvements"
            };

            var tasks = new List<TaskItem>();

            for (int i = 1; i <= 25; i++)
            {
                var createdDate = DateTime.Now.AddDays(-random.Next(0, 180));
                var status = (TasksStatus)random.Next(0, 5);

                tasks.Add(new TaskItem
                {
                    Id = i,
                    Title = taskTitles[random.Next(taskTitles.Length)],
                    Description = $"Description for task {i}",
                    AssignedTo = employees[random.Next(employees.Length)],
                    Status = status,
                    Priority = (TaskPriority)random.Next(0, 4),
                    CreatedDate = createdDate,
                    DueDate = createdDate.AddDays(random.Next(7, 30)),
                    CompletedDate = status == TasksStatus.Completed ? createdDate.AddDays(random.Next(1, 20)) : null,
                    EstimatedHours = random.Next(2, 40),
                    ActualHours = status == TasksStatus.Completed ? random.Next(1, 50) : 0
                });
            }

            return tasks;
        }
    }
}

