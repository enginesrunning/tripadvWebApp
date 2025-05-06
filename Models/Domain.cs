using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelReviewApp.Models
{
    public enum UserType
    {
        Regular,
        Verified,
        Admin
    }

    public enum AdminLevel
    {
        Junior,
        Senior,
        SuperAdmin
    }

    public enum FacilityType
    {
        Hotel,
        Restaurant,
        Attraction
    }

    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Registration Date")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [Display(Name = "User Type")]
        public UserType UserType { get; set; } = UserType.Regular;

        [Display(Name = "Active Status")]
        public bool IsActive { get; set; } = true;

        public int FailedLoginAttempts { get; set; } = 0;

        [Display(Name = "Last Login")]
        public DateTime? LastLoginDate { get; set; }

        // Navigation properties
        public virtual RegularUser RegularUser { get; set; }
        public virtual VerifiedUser VerifiedUser { get; set; }
        public virtual Admin Admin { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }

    public class RegularUser
    {
        [Key]
        [ForeignKey("User")]
        public int UserID { get; set; }

        public string Preferences { get; set; } // Stored as JSON or serialized
        public string RecentSearches { get; set; } // Stored as JSON or serialized

        // Navigation property
        public virtual User User { get; set; }
    }

    public class VerifiedUser
    {
        [Key]
        [ForeignKey("User")]
        public int UserID { get; set; }

        public int? FacilityID { get; set; }

        [Display(Name = "Verification Document")]
        public string VerificationDocument { get; set; }

        [Display(Name = "Verification Date")]
        public DateTime? VerificationDate { get; set; }

        [Display(Name = "Verified Status")]
        public bool IsVerified { get; set; } = false;

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Facility Facility { get; set; }
    }

    public class Admin
    {
        [Key]
        [ForeignKey("User")]
        public int UserID { get; set; }

        [Display(Name = "Admin Level")]
        public AdminLevel AdminLevel { get; set; }

        public string Permissions { get; set; } // Stored as JSON or serialized

        [Display(Name = "Last Action Time")]
        public DateTime LastActionTime { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual ICollection<Report> ResolvedReports { get; set; }
        public virtual ICollection<AdminAction> AdminActions { get; set; }
    }

    public class Facility
    {
        [Key]
        public int FacilityID { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public FacilityType Type { get; set; }

        [Display(Name = "Average Rating")]
        public float AverageRating { get; set; } = 0;

        [Display(Name = "Approval Status")]
        public bool IsApproved { get; set; } = false;

        [Display(Name = "Date Added")]
        public DateTime DateAdded { get; set; } = DateTime.Now;

        [Display(Name = "Added By")]
        public int AddedByUserID { get; set; }

        [Display(Name = "Last Modified By")]
        public int? LastModifiedByUserID { get; set; }

        // Navigation properties
        public virtual Location Location { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<FacilityAmenity> FacilityAmenities { get; set; }
        public virtual VerifiedUser Manager { get; set; }
    }

    public class Location
    {
        [Key]
        public int LocationID { get; set; }

        [ForeignKey("Facility")]
        public int FacilityID { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        public float Latitude { get; set; }
        public float Longitude { get; set; }

        // Navigation property
        public virtual Facility Facility { get; set; }
    }

    public class Image
    {
        [Key]
        public int ImageID { get; set; }

        [ForeignKey("Facility")]
        public int FacilityID { get; set; }

        [Required]
        public string Url { get; set; }

        public string Caption { get; set; }

        [Display(Name = "Uploaded By")]
        public int UploadedByUserID { get; set; }

        [Display(Name = "Upload Date")]
        public DateTime UploadDate { get; set; } = DateTime.Now;

        // Navigation property
        public virtual Facility Facility { get; set; }
    }

    public class Amenity
    {
        [Key]
        public int AmenityID { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        // Navigation property
        public virtual ICollection<FacilityAmenity> FacilityAmenities { get; set; }
    }

    public class FacilityAmenity
    {
        [Key, Column(Order = 0)]
        public int FacilityID { get; set; }

        [Key, Column(Order = 1)]
        public int AmenityID { get; set; }

        // Navigation properties
        public virtual Facility Facility { get; set; }
        public virtual Amenity Amenity { get; set; }
    }

    public class Review
    {
        [Key]
        public int ReviewID { get; set; }

        [ForeignKey("Facility")]
        public int FacilityID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string Text { get; set; }

        [Display(Name = "Post Date")]
        public DateTime PostDate { get; set; } = DateTime.Now;

        [Display(Name = "Edit Date")]
        public DateTime? EditDate { get; set; }

        [Display(Name = "Reported Status")]
        public bool IsReported { get; set; } = false;

        [Display(Name = "Spam Status")]
        public bool IsSpam { get; set; } = false;

        // Navigation properties
        public virtual Facility Facility { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
    }

    public class Report
    {
        [Key]
        public int ReportID { get; set; }

        [ForeignKey("Review")]
        public int ReviewID { get; set; }

        [Display(Name = "Reported By")]
        public int ReportedByUserID { get; set; }

        [Required]
        public string Reason { get; set; }

        [Display(Name = "Report Date")]
        public DateTime ReportDate { get; set; } = DateTime.Now;

        [Display(Name = "Resolution Status")]
        public bool IsResolved { get; set; } = false;

        [Display(Name = "Resolved By")]
        public int? ResolvedByAdminID { get; set; }

        // Navigation properties
        public virtual Review Review { get; set; }
        public virtual User ReportedByUser { get; set; }
        public virtual Admin ResolvedByAdmin { get; set; }
    }

    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        [ForeignKey("Facility")]
        public int FacilityID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        [Display(Name = "Provider URL")]
        public string ProviderURL { get; set; }

        public bool Clicked { get; set; } = false;

        // Navigation properties
        public virtual Facility Facility { get; set; }
        public virtual User User { get; set; }
    }

    public class AdminAction
    {
        [Key]
        public int ActionID { get; set; }

        [ForeignKey("Admin")]
        public int AdminID { get; set; }

        [Required]
        [Display(Name = "Action Type")]
        public string ActionType { get; set; }

        public string Description { get; set; }

        [Display(Name = "Target ID")]
        public int? TargetID { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        // Navigation property
        public virtual Admin Admin { get; set; }
    }

    public class SearchLog
    {
        [Key]
        public int SearchID { get; set; }

        public int? UserID { get; set; }

        [Required]
        public string Query { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public string Filters { get; set; } // Stored as JSON or serialized

        [Display(Name = "Results Count")]
        public int ResultsCount { get; set; }

        // Navigation property
        public virtual User User { get; set; }
    }
}