using Boat_2.Models;
using Microsoft.EntityFrameworkCore;

namespace Boat_2.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
        }
        public DbSet<Users> users; 
    }
}
