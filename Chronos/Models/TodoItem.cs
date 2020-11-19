using System.ComponentModel.DataAnnotations;

namespace Chronos.Models
{
    public class TodoItem
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Notes { get; set; }

        public bool Done { get; set; }
        public int Difficulty { get; set; }
        public string Category { get; set; }
        public string Tag { get; set; }
    }
}