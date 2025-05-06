using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using TravelReviewApp.Models;

namespace TravelReviewApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RegularUser> RegularUsers { get; set; }
        public DbSet<VerifiedUser> VerifiedUsers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<FacilityAmenity> FacilityAmenities { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<AdminAction> AdminActions { get; set; }
        public DbSet<SearchLog> SearchLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many relationship between Facility and Amenity
            modelBuilder.Entity<FacilityAmenity>()
                .HasKey(fa => new { fa.FacilityID, fa.AmenityID });

            modelBuilder.Entity<FacilityAmenity>()
                .HasOne(fa => fa.Facility)
                .WithMany(f => f.FacilityAmenities)
                .HasForeignKey(fa => fa.FacilityID);

            modelBuilder.Entity<FacilityAmenity>()
                .HasOne(fa => fa.Amenity)
                .WithMany(a => a.FacilityAmenities)
                .HasForeignKey(fa => fa.AmenityID);

            // Configure one-to-one relationship between User and its subtypes
            modelBuilder.Entity<RegularUser>()
                .HasOne(ru => ru.User)
                .WithOne(u => u.RegularUser)
                .HasForeignKey<RegularUser>(ru => ru.UserID);

            modelBuilder.Entity<VerifiedUser>()
                .HasOne(vu => vu.User)
                .WithOne(u => u.VerifiedUser)
                .HasForeignKey<VerifiedUser>(vu => vu.UserID);

            modelBuilder.Entity<Admin>()
                .HasOne(a => a.User)
                .WithOne(u => u.Admin)
                .HasForeignKey<Admin>(a => a.UserID);

            // Configure one-to-one relationship between Facility and Location
            modelBuilder.Entity<Facility>()
                .HasOne(f => f.Location)
                .WithOne(l => l.Facility)
                .HasForeignKey<Location>(l => l.FacilityID);

            // Configure one-to-many relationship between Facility and Images
            modelBuilder.Entity<Image>()
                .HasOne(i => i.Facility)
                .WithMany(f => f.Images)
                .HasForeignKey(i => i.FacilityID);

            // Configure one-to-many relationship between Facility and Reviews
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Facility)
                .WithMany(f => f.Reviews)
                .HasForeignKey(r => r.FacilityID);

            // Configure one-to-many relationship between User and Reviews
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserID);

            // Configure one-to-many relationship between Review and Reports
            modelBuilder.Entity<Report>()
                .HasOne(r => r.Review)
                .WithMany(r => r.Reports)
                .HasForeignKey(r => r.ReviewID);

            // Configure relationships for Reports
            modelBuilder.Entity<Report>()
                .HasOne(r => r.ReportedByUser)
                .WithMany(u => u.Reports)
                .HasForeignKey(r => r.ReportedByUserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Report>()
                .HasOne(r => r.ResolvedByAdmin)
                .WithMany(a => a.ResolvedReports)
                .HasForeignKey(r => r.ResolvedByAdminID)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Configure relationships for Bookings
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Facility)
                .WithMany(f => f.Bookings)
                .HasForeignKey(b => b.FacilityID);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserID);

            // Configure relationships for AdminActions
            modelBuilder.Entity<AdminAction>()
                .HasOne(a => a.Admin)
                .WithMany(a => a.AdminActions)
                .HasForeignKey(a => a.AdminID);

            // Configure relationship between VerifiedUser and Facility
            modelBuilder.Entity<VerifiedUser>()
                .HasOne(vu => vu.Facility)
                .WithOne(f => f.Manager)
                .HasForeignKey<VerifiedUser>(vu => vu.FacilityID)
                .IsRequired(false);

            // Configure relationship for SearchLog
            modelBuilder.Entity<SearchLog>()
                .HasOne(sl => sl.User)
                .WithMany()
                .HasForeignKey(sl => sl.UserID)
                .IsRequired(false);

            // Add some seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserID = 1,
                    Email = "admin@travelreview.com",
                    Password = "AdminPassword123!", // In a real app, this would be hashed
                    FirstName = "Admin",
                    LastName = "User",
                    RegistrationDate = new DateTime(2023, 1, 1),
                    UserType = UserType.Admin,
                    IsActive = true
                }
            );

            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    UserID = 1,
                    AdminLevel = AdminLevel.SuperAdmin,
                    Permissions = "All",
                    LastActionTime = new DateTime(2023, 1, 1, 12, 0, 0) // Static value
                }
            );

            // Seed facility types (amenities)
            modelBuilder.Entity<Amenity>().HasData(
                new Amenity { AmenityID = 1, Name = "WiFi", Description = "Free WiFi", Category = "General" },
                new Amenity { AmenityID = 2, Name = "Swimming Pool", Description = "Outdoor swimming pool", Category = "Hotel" },
                new Amenity { AmenityID = 3, Name = "Parking", Description = "Free parking on premises", Category = "General" },
                new Amenity { AmenityID = 4, Name = "Restaurant", Description = "On-site restaurant", Category = "Dining" },
                new Amenity { AmenityID = 5, Name = "Bar", Description = "On-site bar", Category = "Dining" }
            );
        }
    }
}