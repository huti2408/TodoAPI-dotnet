using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoAPI.Models
{
    public class User
    {
        public long UserID { get; set; }
        public string? Name { get; set; }
        public string? username { get; set; }
        [JsonIgnore]
        public string? password { get; set; }
        public virtual ICollection<TodoItem>? TodoItems { get; set; }
    }
}
