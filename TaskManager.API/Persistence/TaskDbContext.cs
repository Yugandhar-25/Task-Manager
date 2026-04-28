using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Task_Manager_API.Models;

namespace Task_Manager_API.Persistence
{
    public class TaskDbContext(DbContextOptions<TaskDbContext> options): DbContext(options)
    {
        public DbSet<TaskItem> Tasks { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
