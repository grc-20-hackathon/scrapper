
using Microsoft.EntityFrameworkCore;

namespace scrapper.Models
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<ProjectView> projectView => Set<ProjectView>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
