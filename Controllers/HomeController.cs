using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TravelReviewApp.Data;
using TravelReviewApp.Models;

namespace TravelReviewApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get top rated facilities for the homepage
            var topFacilities = await _context.Facilities
                .Include(f => f.Location)
                .OrderByDescending(f => f.AverageRating)
                .Where(f => f.IsApproved)
                .Take(4)
                .ToListAsync();

            return View(topFacilities);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}