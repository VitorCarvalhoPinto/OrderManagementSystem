using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Models;

namespace Loja.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Order> Order { get; set; }
        
    }
}
