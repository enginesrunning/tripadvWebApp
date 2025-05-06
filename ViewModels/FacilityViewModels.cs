using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TravelReviewApp.Models;

namespace TravelReviewApp.ViewModels
{
    public class FacilityViewModel
    {
        public int FacilityID { get; set; }

        [Required]
        [Display(Name = "Facility Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Facility Type")]
        public FacilityType Type { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Latitude")]
        public float Latitude { get; set; }

        [Display(Name = "Longitude")]
        public float Longitude { get; set; }

        [Display(Name = "Amenities")]
        public List<int> SelectedAmenities { get; set; }

        // For dropdowns and multiselect lists
        public List<SelectListItem> FacilityTypes { get; set; }
        public List<SelectListItem> AmenityList { get; set; }
    }
}