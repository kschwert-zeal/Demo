namespace Demo.Models
{
    public class TaskStatistics
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int OverdueTasks { get; set; }
        public Dictionary<TasksStatus, int> StatusBreakdown { get; set; } = new();
        public Dictionary<TaskPriority, int> PriorityBreakdown { get; set; } = new();
        public List<MonthlyTaskData> MonthlyData { get; set; } = new();
    }
}
