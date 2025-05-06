using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TravelReviewApp.Data;
using TravelReviewApp.Models;
using TravelReviewApp.ViewModels;

namespace TravelReviewApp.Controllers
{
    public class FacilityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacilityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Facility
        public async Task<IActionResult> Index()
        {
            var facilities = await _context.Facilities
                .Include(f => f.Location)
                .ToListAsync();
            return View(facilities);
        }

        // GET: Facility/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var facility = await _context.Facilities
                .Include(f => f.Location)
                .Include(f => f.Images)
                .Include(f => f.Reviews).ThenInclude(r => r.User)
                .Include(f => f.FacilityAmenities).ThenInclude(fa => fa.Amenity)
                .FirstOrDefaultAsync(f => f.FacilityID == id);

            if (facility == null) return NotFound();

            return View(facility);
        }

        // GET: Facility/Create
        public IActionResult Create()
        {
            var viewModel = new FacilityViewModel
            {
                FacilityTypes = Enum.GetValues(typeof(FacilityType))
                    .Cast<FacilityType>()
                    .Select(t => new SelectListItem
                    {
                        Value = ((int)t).ToString(),
                        Text = t.ToString()
                    }).ToList(),
                AmenityList = _context.Amenities
                    .Select(a => new SelectListItem
                    {
                        Value = a.AmenityID.ToString(),
                        Text = a.Name
                    }).ToList()
            };

            return View(viewModel);
        }

        // POST: Facility/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FacilityViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var facility = new Facility
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Type = viewModel.Type,
                    DateAdded = DateTime.Now,
                    AddedByUserID = 1 // demo
                };
                _context.Facilities.Add(facility);
                await _context.SaveChangesAsync();

                var location = new Location
                {
                    FacilityID = facility.FacilityID,
                    Address = viewModel.Address,
                    City = viewModel.City,
                    Country = viewModel.Country,
                    Latitude = viewModel.Latitude,
                    Longitude = viewModel.Longitude
                };
                _context.Locations.Add(location);

                if (viewModel.SelectedAmenities != null && viewModel.SelectedAmenities.Any())
                {
                    foreach (var amenityId in viewModel.SelectedAmenities)
                    {
                        var facilityAmenity = new FacilityAmenity
                        {
                            FacilityID = facility.FacilityID,
                            AmenityID = amenityId
                        };
                        _context.FacilityAmenities.Add(facilityAmenity);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            viewModel.FacilityTypes = Enum.GetValues(typeof(FacilityType))
                .Cast<FacilityType>()
                .Select(t => new SelectListItem
                {
                    Value = ((int)t).ToString(),
                    Text = t.ToString()
                }).ToList();

            viewModel.AmenityList = _context.Amenities
                .Select(a => new SelectListItem
                {
                    Value = a.AmenityID.ToString(),
                    Text = a.Name
                }).ToList();

            return View(viewModel);
        }

        // GET: Facility/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var facility = await _context.Facilities
                .Include(f => f.Location)
                .Include(f => f.FacilityAmenities)
                .FirstOrDefaultAsync(f => f.FacilityID == id);

            if (facility == null) return NotFound();

            var viewModel = new FacilityViewModel
            {
                FacilityID = facility.FacilityID,
                Name = facility.Name,
                Description = facility.Description,
                Type = facility.Type,
                Address = facility.Location?.Address,
                City = facility.Location?.City,
                Country = facility.Location?.Country,
                Latitude =  facility.Location.Latitude,
                Longitude = facility.Location.Longitude,
                SelectedAmenities = facility.FacilityAmenities.Select(fa => fa.AmenityID).ToList(),
                FacilityTypes = Enum.GetValues(typeof(FacilityType))
                    .Cast<FacilityType>()
                    .Select(t => new SelectListItem
                    {
                        Value = ((int)t).ToString(),
                        Text = t.ToString()
                    }).ToList(),
                AmenityList = _context.Amenities
                    .Select(a => new SelectListItem
                    {
                        Value = a.AmenityID.ToString(),
                        Text = a.Name
                    }).ToList()
            };

            return View(viewModel);
        }

        // POST: Facility/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FacilityViewModel viewModel)
        {
            if (id != viewModel.FacilityID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var facility = await _context.Facilities.FindAsync(id);
                    if (facility == null) return NotFound();

                    facility.Name = viewModel.Name;
                    facility.Description = viewModel.Description;
                    facility.Type = viewModel.Type;
                    facility.LastModifiedByUserID = 1;

                    _context.Facilities.Update(facility);

                    var location = await _context.Locations.FirstOrDefaultAsync(l => l.FacilityID == id);
                    if (location == null)
                    {
                        location = new Location
                        {
                            FacilityID = id,
                            Address = viewModel.Address,
                            City = viewModel.City,
                            Country = viewModel.Country,
                            Latitude = viewModel.Latitude,
                            Longitude = viewModel.Longitude
                        };
                        _context.Locations.Add(location);
                    }
                    else
                    {
                        location.Address = viewModel.Address;
                        location.City = viewModel.City;
                        location.Country = viewModel.Country;
                        location.Latitude = viewModel.Latitude;
                        location.Longitude = viewModel.Longitude;
                        _context.Locations.Update(location);
                    }

                    // Remove old amenities
                    var oldAmenities = await _context.FacilityAmenities
                        .Where(fa => fa.FacilityID == id).ToListAsync();
                    _context.FacilityAmenities.RemoveRange(oldAmenities);

                    // Add new ones
                    if (viewModel.SelectedAmenities != null && viewModel.SelectedAmenities.Any())
                    {
                        foreach (var amenityId in viewModel.SelectedAmenities)
                        {
                            _context.FacilityAmenities.Add(new FacilityAmenity
                            {
                                FacilityID = id,
                                AmenityID = amenityId
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacilityExists(viewModel.FacilityID)) return NotFound();
                    throw;
                }
            }

            // Reload lists in case of error
            viewModel.FacilityTypes = Enum.GetValues(typeof(FacilityType))
                .Cast<FacilityType>()
                .Select(t => new SelectListItem
                {
                    Value = ((int)t).ToString(),
                    Text = t.ToString()
                }).ToList();

            viewModel.AmenityList = _context.Amenities
                .Select(a => new SelectListItem
                {
                    Value = a.AmenityID.ToString(),
                    Text = a.Name
                }).ToList();

            return View(viewModel);
        }

        // GET: Facility/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var facility = await _context.Facilities
                .Include(f => f.Location)
                .FirstOrDefaultAsync(f => f.FacilityID == id);

            if (facility == null) return NotFound();

            return View(facility);
        }

        // POST: Facility/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facility = await _context.Facilities.FindAsync(id);
            if (facility == null) return NotFound();

            _context.Facilities.Remove(facility);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FacilityExists(int id)
        {
            return _context.Facilities.Any(f => f.FacilityID == id);
        }
    }
}
