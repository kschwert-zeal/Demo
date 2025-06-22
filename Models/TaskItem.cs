using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public string AssignedTo { get; set; } = string.Empty;

        public TasksStatus Status { get; set; } = TasksStatus.NotStarted;

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? DueDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public int EstimatedHours { get; set; }

        public int ActualHours { get; set; }
    }

    public enum TasksStatus
    {
        NotStarted,
        InProgress,
        OnHold,
        Completed,
        Cancelled
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Critical
    }
}
