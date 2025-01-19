using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Models;

namespace PMS.Controllers
{
    public class PropertyManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PropertyManagerController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult PMDashboard()
        {
            return View();
        }
        public IActionResult PMManageLease()
        {
            return View();
        }
        public IActionResult PMAssignMaintenance()
        {
            return View();
        }

        public async Task<IActionResult> PMManageUnits()
        {
            try
            {
                // Fetch data from the Units table
                var units = await _context.Units
                    .Where(u => u.UnitStatus == "Active") // Filter for active units
                    .Select(u => new UnitViewModel
                    {
                        UnitId = u.UnitID,
                        UnitName = u.UnitName,
                        PricePerMonth = u.PricePerMonth,
                        UnitType = u.UnitType,
                        UnitStatus = u.UnitStatus
                    })
                    .ToListAsync();

                // Pass the list of units to the view
                return View(units);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading units: {ex.Message}";
                return View(new List<UnitViewModel>()); // Return an empty list in case of error
            }
        }
        public IActionResult PMPayments()
        {
            return View();
        }

        public IActionResult PMRequest()
        {
            return View();
        }
        public async Task<IActionResult> PMTenants()
        {
            try
            {
                // Fetch data from the Tenants table
                var tenants = await _context.Tenants
                    .Include(t => t.User) // Include User data
                                          //.Include(t => t.Unit) // Include Unit data
                    .Select(t => new TenantViewModel
                    {
                        TenantID = t.TenantID,
                        UserId = t.UserId,
                        PhoneNumber = t.PhoneNumber,
                        //UnitID = t.UnitID,
                        User = t.User, // Optional: Pass the User object if needed
                        //Unit = t.Unit  // Optional: Pass the Unit object if needed
                    })
                    .ToListAsync();

                // Pass the list of tenants to the view
                return View(tenants);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading tenants: {ex.Message}";
                return View(new List<TenantViewModel>()); // Return an empty list in case of error
            }
        }

        public async Task<IActionResult> PMManageUsers()
        {
            try
            {
                // Fetch data from the users table
                var users = await _context.Users
                    .Where(u => u.Role != "Tenant")
                    .Select(u => new UserViewModel
                    {
                        UserID = u.UserID,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        Role = u.Role,
                        IsActive = u.IsActive,

                    })
                    .ToListAsync();

                // Pass the list of users to the view
                return View(users);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading users: {ex.Message}";
                return View(new List<UserViewModel>()); // Return an empty list in case of error
            }
        }

        public IActionResult PMEditProfile()
        {
            return View();
        }

        public IActionResult AddUnitPage()
        {
            return View();
        }

        public async Task<IActionResult> EditUnitPage(int id)
        {
            try
            {
                // Fetch the unit details by UnitId
                var unit = await _context.Units.FirstOrDefaultAsync(u => u.UnitID == id && u.UnitStatus == "Active");

                if (unit == null)
                {
                    TempData["ErrorMessage"] = "Unit not found.";
                    return RedirectToAction("PMManageUnits");
                }

                // Fetch images
                var images = _context.UnitImages.Where(i => i.UnitId == id).ToList();
                unit.Images = images; 

                return View(unit);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading unit: {ex.Message}";
                return RedirectToAction("PMManageUnits");
            }
        }


        // Action for adding units
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUnit(Unit model, IFormFileCollection Images)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(model); // Return the form with validation messages
            //}

            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    // Save Unit details to Units table
                    var newUnit = new Unit
                    {
                        UnitName = model.UnitName,
                        UnitType = model.UnitType,
                        UnitStatus = model.UnitStatus,
                        UnitOwner = model.UnitOwner,
                        Description = model.Description,
                        PricePerMonth = model.PricePerMonth,
                        SecurityDeposit = model.SecurityDeposit,
                        Town = model.Town,
                        Location = model.Location,
                        Country = model.Country,
                        State = model.State,
                        City = model.City,
                        ZipCode = model.ZipCode,
                        NumberOfUnits = model.NumberOfUnits,
                        NumberOfBedrooms = model.NumberOfBedrooms,
                        NumberOfBathrooms = model.NumberOfBathrooms,
                        NumberOfGarages = model.NumberOfGarages,
                        NumberOfFloors = model.NumberOfFloors
                    };

                    _context.Units.Add(newUnit);
                    await _context.SaveChangesAsync();

                    // Save images to UnitImages table and the filesystem
                    if (Images != null && Images.Any())
                    {
                        var imagesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "units");

                        // Ensure the units folder exists
                        if (!Directory.Exists(imagesFolderPath))
                        {
                            Directory.CreateDirectory(imagesFolderPath);
                        }

                        foreach (var image in Images)
                        {
                            if (image.Length > 0)
                            {
                                // Define the file path to save the image
                                var filePath = Path.Combine(imagesFolderPath, image.FileName);

                                // Save the image to the server
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await image.CopyToAsync(stream);
                                }

                                // Save metadata to the UnitImages table
                                var unitImage = new UnitImage
                                {
                                    UnitId = newUnit.UnitID, // Link to the newly created unit
                                    FilePath = $"/images/units/{image.FileName}" // Relative path for access in views
                                };
                                _context.UnitImages.Add(unitImage);
                            }
                        }

                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                    TempData["ShowPopup"] = true; // Indicate that the popup shou
                    TempData["PopupMessage"] = "Unit added successfully!";
                }

                return RedirectToAction("PMManageUnits"); // Redirect to the list of units
            }
            catch (Exception ex)
            {
                // Rollback transaction in case of an error
                TempData["ErrorMessage"] = $"Error adding unit: {ex.Message}";
                return RedirectToAction("AddUnitPage");
            }
        }


        // POST: EditUnit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUnit(Unit unit, IFormFileCollection Images)
        {
            if (!ModelState.IsValid)
            {
                // Return to the same page with validation errors
                return View("EditUnitPage", unit);
            }

            // Retrieve the unit from the database using the UnitID
            var existingUnit = await _context.Units.FindAsync(unit.UnitID);
            if (existingUnit == null)
            {
                return NotFound();
            }

            // Update the properties of the unit
            existingUnit.UnitName = unit.UnitName;
            existingUnit.UnitType = unit.UnitType;
            existingUnit.UnitOwner = unit.UnitOwner;
            existingUnit.Description = unit.Description;
            existingUnit.PricePerMonth = unit.PricePerMonth;
            existingUnit.SecurityDeposit = unit.SecurityDeposit;
            existingUnit.Town = unit.Town;
            existingUnit.Location = unit.Location;
            existingUnit.Country = unit.Country;
            existingUnit.State = unit.State;
            existingUnit.City = unit.City;
            existingUnit.ZipCode = unit.ZipCode;
            existingUnit.NumberOfUnits = unit.NumberOfUnits;
            existingUnit.NumberOfBedrooms = unit.NumberOfBedrooms;
            existingUnit.NumberOfBathrooms = unit.NumberOfBathrooms;
            existingUnit.NumberOfGarages = unit.NumberOfGarages;
            existingUnit.NumberOfFloors = unit.NumberOfFloors;
            existingUnit.UnitStatus = unit.UnitStatus;


            // Handle image upload if provided
            if (Images != null && Images.Any())
            {
                // Define the folder path where images will be stored
                var imagesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "units");

                // Ensure the directory exists
                if (!Directory.Exists(imagesFolderPath))
                {
                    Directory.CreateDirectory(imagesFolderPath);
                }

                // Remove existing images from the database (optional)
                // _context.UnitImages.RemoveRange(existingUnit.Images);
                // await _context.SaveChangesAsync();

                foreach (var image in Images)
                {
                    if (image.Length > 0)
                    {
                        // Define the file path to save the image
                        var filePath = Path.Combine(imagesFolderPath, image.FileName);

                        // Save the image to the server
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        // Save the image metadata to the UnitImages table
                        var unitImage = new UnitImage
                        {
                            UnitId = existingUnit.UnitID, // Link to the existing unit
                            FilePath = $"/images/units/{image.FileName}" // Relative path for access in views
                        };

                        _context.UnitImages.Add(unitImage);
                    }
                }
            }



            try
            {
                // Save changes to the database
                _context.Update(existingUnit);
                await _context.SaveChangesAsync();
                TempData["ShowPopup"] = true; // Indicate that the popup shou
                TempData["PopupMessage"] = "Unit updated successfully!";
                //TempData.Keep("ShowPopup");
                //TempData.Keep("PopupMessage");

            }
            catch (Exception ex)
            {

                // Log the error and display an error message
                TempData["ErrorMessage"] = $"An error occurred while updating the unit: {ex.Message}";
            }
            // Redirect to the Units page or any other desired page
            return RedirectToAction("PMManageUnits");
        }


        // Soft delete unit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUnit(int id)
        {
            // Retrieve the unit from the database using the UnitID
            var unit = _context.Units.FirstOrDefault(u => u.UnitID == id);
            if (unit == null)
            {
                return NotFound();
            }

            // Set the UnitStatus to "Inactive" for soft deletion
            unit.UnitStatus = "Inactive";

            try
            {
                // Update the unit status and save changes to the database
                _context.Update(unit);
                _context.SaveChanges();  // Synchronous save

                // Set a success message to show in the view
                TempData["ShowPopup"] = true;  // Indicate that the popup should be shown
                TempData["PopupMessage"] = "Unit deleted successfully!";
            }
            catch (Exception ex)
            {
                // Log the error and show an error message
                TempData["ErrorMessage"] = $"An error occurred while deactivating the unit: {ex.Message}";
            }

            // Redirect to the Units page after the action
            return RedirectToAction("PMManageUnits");
        }


        // Action method to search units by name or type
        [HttpGet]
        public IActionResult SearchUnits(string query)
        {
            // Filter out units with "Inactive" UnitStatus
            var activeUnits = _context.Units
                .Where(u => u.UnitStatus == "Active"); // Only include units with UnitStatus as "Active"

            // If query is null or empty, return all units
            if (string.IsNullOrEmpty(query))
            {
                var allActiveUnits = activeUnits.ToList();
                return Json(allActiveUnits); // Return all active units as JSON
            }

            // Filter by UnitName or UnitType (case-insensitive)
            var filteredUnits = activeUnits
                .Where(u => u.UnitName.ToLower().Contains(query.ToLower()) ||
                            u.UnitType.ToLower().Contains(query.ToLower())) // Match query with UnitName or UnitType
                .ToList();

            return Json(filteredUnits); // Return filtered units as JSON
        }


    }
}
