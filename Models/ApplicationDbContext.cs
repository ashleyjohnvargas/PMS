using Azure.Core;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PMS.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<PropertyManager> PropertyManagers { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Lease> Leases { get; set; }
        public DbSet<LeaseDetails> LeaseDetails { get; set; }

        public DbSet<Unit> Units { get; set; }
        public DbSet<UnitImage> UnitImages { get; set; }
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
        public DbSet<Request> Requests { get; set; }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<User> Users { get; set; }
        public required DbSet<Profile> UserProfiles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Set default values
            modelBuilder.Entity<Lease>()
                .Property(l => l.LeaseStatus)
                .HasDefaultValue("Pending");

            modelBuilder.Entity<MaintenanceRequest>()
                .Property(mr => mr.RequestStatus)
                .HasDefaultValue("Pending");

            modelBuilder.Entity<Payment>()
                .Property(p => p.PaymentStatus)
                .HasDefaultValue("Pending");

            modelBuilder.Entity<Unit>()
                .Property(u => u.UnitStatus)
                .HasDefaultValue("Active");

            modelBuilder.Entity<Unit>()
                .Property(u => u.AvailabilityStatus)
                .HasDefaultValue("Available");

            modelBuilder.Entity<User>()
                .Property(u => u.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Tenant>()
                .Property(t => t.IsActualTenant)
                .HasDefaultValue(false);
           
            modelBuilder.Entity<Tenant>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tenants)
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<Staff>()
                .Property(s => s.IsVacant)
                .HasDefaultValue(true);

            modelBuilder.Entity<Request>()
                .Property(r => r.RequestStatus)
                .HasDefaultValue("Pending");
        }
    }
}
        //// Set default values
        //modelBuilder.Entity<Lease>()
        //    .Property(l => l.LeaseStatus)
        //    .HasDefaultValue("Active");

        //modelBuilder.Entity<MaintenanceRequest>()
        //    .Property(mr => mr.RequestStatus)
        //    .HasDefaultValue("Pending");

        //modelBuilder.Entity<Payment>()
        //    .Property(p => p.PaymentStatus)
        //    .HasDefaultValue("Pending");

        //modelBuilder.Entity<Unit>()
        //    .Property(u => u.UnitStatus)
        //    .HasDefaultValue("Active");

        //modelBuilder.Entity<User>()
        //    .Property(u => u.IsActive)
        //    .HasDefaultValue(true);


        //modelBuilder.Entity<User>().HasData(
        //new User
        //{
        //    UserID = 1,
        //    FirstName = "Test",
        //    LastName = "User",
        //    Email = "test@example.com",
        //    Password = BCrypt.Net.BCrypt.HashPassword("Password123"),
        //    Role = "Tenant",
        //    DateCreated = DateTime.Now
        //}
        //);

        //modelBuilder.Entity<Profile>().HasData(
        //    new Profile
        //    {
        //        Id = 1,
        //        FirstName = "Test",
        //        LastName = "User",
        //        Email = "test@example.com"
        //    }
        //);

    
