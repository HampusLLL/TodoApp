using Microsoft.EntityFrameworkCore;
using TodoMVC.Models;

namespace TodoMVC.Data
{
    public class TodoDBContext : DbContext
    {
        public TodoDBContext(DbContextOptions<TodoDBContext> options)
            :base(options)
        {
        }

        public DbSet<Notes> Notes { get; set; }
    }
}
