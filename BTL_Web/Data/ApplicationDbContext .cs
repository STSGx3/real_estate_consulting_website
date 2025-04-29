using BTL_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BTL_Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } // Người dùng

        public DbSet<RealEstate> RealEstates { get; set; } // Bất động sản 
        public DbSet<Consultant> Consultants { get; set; }

        public DbSet<Message> Messages { get; set; }
    }


}
