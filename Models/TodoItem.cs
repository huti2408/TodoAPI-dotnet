namespace TodoAPI.Models
{
    public class TodoItem
    {
        public long TodoItemId { get; set; }
        public string? Name { get; set; }
        public bool IsComplete { get; set; }
        public long UserID { get; set; }
        public virtual User? User { get; set; } = null!;
    }
}
