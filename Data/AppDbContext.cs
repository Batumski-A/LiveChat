using LiveChat.Models;
using Microsoft.EntityFrameworkCore;

namespace LiveChat.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
        }
        public DbSet<ChatUsers> ChatUsers { get; set; }
    }
}
