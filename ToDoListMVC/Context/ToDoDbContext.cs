using Microsoft.EntityFrameworkCore;
namespace ToDoListMVC.Context
{
    public class ToDoDbContext : DbContext
    {
        private readonly IConfiguration configuration;

        public ToDoDbContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection");
            if (connectionString is not null)
            {
                optionsBuilder.UseMySQL(connectionString);
            }
        }

        public DbSet<ToDoListMVC.Models.ToDoModel> ToDoModel { get; set; } = default!;

    }
}
