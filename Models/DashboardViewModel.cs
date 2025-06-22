namespace Demo.Models
{
    public class DashboardViewModel
    {
        public List<TaskItem> Tasks { get; set; } = new();
        public string SelectedEmployee { get; set; } = string.Empty;
        public List<string> Employees { get; set; } = new();
        public TaskStatistics Statistics { get; set; } = new();
    }
}
