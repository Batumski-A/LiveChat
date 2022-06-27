using LiveChat.Models;
using LiveChat.Models.User;
using Microsoft.EntityFrameworkCore;

namespace LiveChat.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
        }
        public DbSet<ChatUsers> ChatUsers { get; set; }
        public DbSet<Messages> Messages { get; set; }
    }
}
