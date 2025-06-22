using Demo.Models;

namespace Demo.Interfaces
{
    public interface ITaskService
    {
        List<TaskItem> GetTasksForEmployee(string employeeName);
        List<string> GetAllEmployees();
        TaskStatistics GetStatistics(string employeeName);
        void AddTask(TaskItem task);
        void UpdateTask(TaskItem task);
        void DeleteTask(int taskId);
    }
}
