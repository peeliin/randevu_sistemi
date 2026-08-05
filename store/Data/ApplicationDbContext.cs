using Microsoft.EntityFrameworkCore;
using store.Models;

namespace store.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        { }
        
        public DbSet<Service> Services { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<Category> Categories { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // password: admin123 
            modelBuilder.Entity<AdminUser>().HasData(
                new AdminUser() 
                { 
                    Id = 1, 
                    Username = "admin", 
                    Password = "admin123", // In production, use password hashing
                    Email = "admin@guzellikmerkezi.com",
                    FirstName = "Admin",
                    LastName = "User",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                }
            );
            
            modelBuilder.Entity<Service>().HasData(
                new Service() 
                { 
                    Id = 1, 
                    Name = "Saç Kesimi", 
                    Description = "Profesyonel saç kesimi hizmeti",
                    Price = 150,
                    Duration = 30,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new Service() 
                { 
                    Id = 2, 
                    Name = "Saç Boyama", 
                    Description = "Profesyonel saç boyama hizmeti",
                    Price = 300,
                    Duration = 120,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new Service() 
                { 
                    Id = 3, 
                    Name = "Cilt Bakımı", 
                    Description = "Profesyonel cilt bakımı hizmeti",
                    Price = 250,
                    Duration = 60,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                }
            );
        }
    }
}
