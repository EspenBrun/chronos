using Microsoft.EntityFrameworkCore;

namespace Chronos.Models
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
        }

        public DbSet<TodoItem> ToDoItems { get; set; }
        public DbSet<TimeBlock> TimeBlocks { get; set; }
    }
}