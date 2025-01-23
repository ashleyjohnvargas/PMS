using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Models;

namespace PMS.Controllers
{
    public class PTenantController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PTenantController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult PTenantHomePage()
        {
            return View();
        }

        public IActionResult PTenantContacts()
        {
            return View();
        }


        //public async Task<IActionResult> PTenantUnits()
        //{
        //    var units = await _context.Units
        //    .Where(u => u.UnitStatus == "Active") // Filter for Active units
        //    .Include(u => u.Images)  // Include images
        //    .ToListAsync();

        //    var unitViewModels = units.Select(u => new PTenantUnitViewModel
        //    {
        //        UnitId = u.UnitID,
        //        UnitName = u.UnitName,
        //        Description = u.Description,
        //        //NumberOfUnits = u.NumberOfUnits ?? 0,
        //        NumberOfRooms = u.NumberOfBedrooms ?? 0, //+ (u.NumberOfBathrooms ?? 0), // Assuming total rooms = bedrooms + bathrooms
        //        //NumberOfBathrooms = u.NumberOfBathrooms ?? 0,

        //        FirstImagePath = u.Images?.FirstOrDefault()?.FilePath ?? "" // Get the first image path
        //    }).ToList();

        //    return View(unitViewModels); // Return the view with the unit list
        //}
        public async Task<IActionResult> PTenantUnits()
        {
            var units = await _context.Units
            .Where(u => u.UnitStatus == "Active") // Filter for Active units
            .Include(u => u.Images)  // Include images
            .ToListAsync();



            var unitViewModels = units.Select(u => new PTenantUnitViewModel
            {
                UnitId = u.UnitID,
                UnitName = u.UnitName,
                Description = u.Description,
                NumberOfUnits = u.NumberOfUnits ?? 0,
                NumberOfRooms = u.NumberOfBedrooms ?? 0 + (u.NumberOfBathrooms ?? 0), // Assuming total rooms = bedrooms + bathrooms
                FirstImagePath = u.Images?.FirstOrDefault()?.FilePath ?? "" // Get the first image path
            }).ToList();

            foreach (var unit in unitViewModels)
            {
                Console.WriteLine($"Unit ID from Units list: {unit.UnitId}"); // For debugging
            }


            return View(unitViewModels); // Return the view with the unit list
        }

        [HttpGet]
        public IActionResult PTenantDetails(int id)
        {
            // Fetch the unit details based on the provided ID
            var unit = _context.Units
                .Include(u => u.Images) // Include associated images
                .FirstOrDefault(u => u.UnitID == id);

            // Check if the unit exists
            if (unit == null)
            {
                return NotFound(); // Return a 404 error if the unit is not found
            }

            // Prepare the model for the view
            var model = new
            {
                UnitId = unit.UnitID,
                UnitNAME = unit.UnitName,
                Desc = unit.Description,
                NumberOfBedrooms = unit.NumberOfBedrooms ?? 0,
                NumberOfBathrooms = unit.NumberOfBathrooms ?? 0,
                MonthlyRent = unit.PricePerMonth.HasValue ? unit.PricePerMonth.Value : 0,
                PropertyAddress = $"{unit.Location} {unit.Town} {unit.City} {unit.State} {unit.Country} {unit.ZipCode}",
                MainImage = unit.Images?.FirstOrDefault()?.FilePath, // First image for main display
                GalleryImages = unit.Images?.Skip(1).Select(i => i.FilePath).ToList() // Other images for the gallery
            };

            return View(model); // Render the PTenantDetails view with the model
        }



        //[HttpGet]
        //public IActionResult PTenantDetails(int id)
        //{
        //    // Console.WriteLine("Unit ID of the selected unit: " + id); // For debugging
        //    // Fetch the unit details based on the provided ID
        //    var unit = _context.Units
        //        .Include(u => u.Images) // Include associated images
        //        .FirstOrDefault(u => u.UnitID == id);

        //    // Check if the unit exists
        //    if (unit == null)
        //    {
        //        return NotFound(); // Return a 404 error if the unit is not found
        //    }

        //    // Prepare the model for the view
        //    var model = new
        //    {
        //        UnitId = unit.UnitID,
        //        UnitNAME = unit.UnitName,
        //        Desc = unit.Description,
        //        NumberOfBedrooms = unit.NumberOfBedrooms ?? 0,
        //        NumberOfBathrooms = unit.NumberOfBathrooms ?? 0,
        //        MonthlyRent = unit.PricePerMonth.HasValue ? unit.PricePerMonth.Value : 0,
        //        PropertyAddress = $"{unit.Location} {unit.Town} {unit.City} {unit.State} {unit.Country} {unit.ZipCode}",
        //        MainImage = unit.Images?.FirstOrDefault()?.FilePath, // First image for main display
        //        GalleryImages = unit.Images?.Skip(1).Select(i => i.FilePath).ToList() // Other images for the gallery
        //    };
        //    Console.WriteLine("Unit ID to be passed to Book This Unit Now: " + model.UnitId); // For debugging

        //    return View(model); // Render the PTenantDetails view with the model
        //}

        public IActionResult PTenantApply(int unitId)
        {
            Console.WriteLine($"unitId received: {unitId}"); // For debugging
            return View(unitId);
        }



        [HttpPost]
        public IActionResult ApplyForLease(LeaseApplication leaseApplication)
        {
            // Log all values of the LeaseApplication object to the console
            Console.WriteLine("LeaseApplication object values:");
            Console.WriteLine($"FullName: {leaseApplication.FullName}");
            Console.WriteLine($"BirthDate: {leaseApplication.BirthDate}");
            Console.WriteLine($"ContactNumber: {leaseApplication.ContactNumber}");
            Console.WriteLine($"Email: {leaseApplication.Email}");
            Console.WriteLine($"CurrentAddress: {leaseApplication.CurrentAddress}");
            Console.WriteLine($"EmploymentStatus: {leaseApplication.EmploymentStatus}");
            Console.WriteLine($"EmployerName: {leaseApplication.EmployerName}");
            Console.WriteLine($"MonthlyIncome: {leaseApplication.MonthlyIncome}");
            Console.WriteLine($"UnitId: {leaseApplication.UnitId}");
            Console.WriteLine($"LeaseStartDate: {leaseApplication.LeaseStartDate}");
            Console.WriteLine($"LeaseDuration: {leaseApplication.LeaseDuration}");
            Console.WriteLine($"TermsAndConditions: {leaseApplication.TermsAndConditions}");
            if (leaseApplication == null)
            {
                return BadRequest("Invalid lease application.");
            }



            // Validate required fields
            if (leaseApplication.UnitId == null ||
                leaseApplication.LeaseStartDate == null || leaseApplication.LeaseDuration == null ||
                leaseApplication.TermsAndConditions != true)
            {
                ModelState.AddModelError("", "All required fields must be filled out.");
                return RedirectToAction("PTenantApply", new { unitId = leaseApplication.UnitId });
            }

            // Retrieve UserId from session
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized("User not logged in.");
            }

            // Retrieve TenantId from the Tenants table based on UserId
            var tenant = _context.Tenants.FirstOrDefault(t => t.UserId == userId);
            if (tenant == null)
            {
                return BadRequest("Tenant not found for the logged-in user.");
            }

            // Calculate LeaseEndDate based on LeaseDuration and LeaseStartDate
            DateTime? leaseEndDate = leaseApplication.LeaseStartDate?.AddMonths(leaseApplication.LeaseDuration.Value);

            // Create Lease instance
            var lease = new Lease
            {
                TenantID = tenant.TenantID,
                UnitId = leaseApplication.UnitId,
                LeaseStartDate = leaseApplication.LeaseStartDate,
                LeaseDuration = leaseApplication.LeaseDuration,
                LeaseEndDate = leaseEndDate,
                LeaseStatus = "Pending", // Default value for LeaseStatus
                TermsAndConditions = leaseApplication.TermsAndConditions
            };
            _context.Leases.Add(lease);
            _context.SaveChanges();

            // Create LeaseDetails instance
            var leaseDetails = new LeaseDetails
            {
                LeaseID = lease.LeaseID,
                FullName = leaseApplication.FullName,
                BirthDate = leaseApplication.BirthDate,
                ContactNumber = leaseApplication.ContactNumber,
                Email = leaseApplication.Email,
                CurrentAddress = leaseApplication.CurrentAddress,
                EmploymentStatus = leaseApplication.EmploymentStatus,
                EmployerName = leaseApplication.EmployerName,
                MonthlyIncome = leaseApplication.MonthlyIncome
            };

            _context.LeaseDetails.Add(leaseDetails);
            _context.SaveChanges();

            try
            {
                TempData["ShowPopup"] = true; // Indicate that the popup should be shown
                TempData["PopupMessage"] = "Your application has been sent successfully!";
                return RedirectToAction("PTenantHomePage"); // Redirect to a success page or confirmation
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the lease application: " + ex.Message);
                return RedirectToAction("PTenantApply", new { unitId = leaseApplication.UnitId });
            }
        }

    }
}
