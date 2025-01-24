using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using PMS.Models;

namespace PMS.Controllers
{
    public class PropertyManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PropertyManagerController> _logger;
        public PropertyManagerController(ApplicationDbContext context, ILogger<PropertyManagerController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult PMDashboard()
        {
            return View();
        }
        public IActionResult PMManageLease()
        {
            // Assuming _context is your DbContext instance
            var leases = _context.Leases
                .Include(l => l.Unit) // Include the related Unit entity
                .Include(l => l.LeaseDetails) // Include the related LeaseDetail entity
                .Where(l => l.LeaseStatus == "Pending") // Filter leases with LeaseStatus of 'Pending'
                .Select(l => new LeaseViewModel
                {
                    LeaseID = l.LeaseID,
                    TenantName = l.LeaseDetails.FullName, // Tenant's FullName from LeaseDetail
                    UnitName = l.Unit.UnitName, // UnitName from Unit
                    MonthlyRent = l.Unit.PricePerMonth, // MonthlyRent from Unit
                    LeaseStartDate = l.LeaseStartDate,
                    LeaseEndDate = l.LeaseEndDate,
                    LeaseStatus = l.LeaseStatus
                })
                .ToList();

            return View(leases);
        }

        public IActionResult PMActiveLease()
        {
            // Assuming _context is your DbContext instance
            var leases = _context.Leases
                .Include(l => l.Unit) // Include the related Unit entity
                .Include(l => l.LeaseDetails) // Include the related LeaseDetail entity
                .Where(l => l.LeaseStatus == "Active") // Filter leases with LeaseStatus of 'Pending'
                .Select(l => new LeaseViewModel
                {
                    LeaseID = l.LeaseID,
                    TenantName = l.LeaseDetails.FullName, // Tenant's FullName from LeaseDetail
                    UnitName = l.Unit.UnitName, // UnitName from Unit
                    MonthlyRent = l.Unit.PricePerMonth, // MonthlyRent from Unit
                    LeaseStartDate = l.LeaseStartDate,
                    LeaseEndDate = l.LeaseEndDate,
                    LeaseStatus = l.LeaseStatus
                })
                .ToList();

            return View(leases);
        }


        public IActionResult DownloadLeaseAgreement(int id)
        {
            // Retrieve the lease using the LeaseID
            var lease = _context.Leases.FirstOrDefault(l => l.LeaseID == id);

            if (lease == null || string.IsNullOrEmpty(lease.LeaseAgreementFilePath))
            {
                // If no lease found or file path is missing, return an error message
                TempData["ErrorMessage"] = "Lease agreement not found.";
                return RedirectToAction("PMManageLease"); // Redirect to a relevant page
            }

            // Combine the base path (wwwroot) and the relative file path stored in the database
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", lease.LeaseAgreementFilePath.TrimStart('/'));

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                TempData["ErrorMessage"] = "Lease agreement file not found.";
                return RedirectToAction("PMManageLease"); // Redirect to a relevant page
            }

            // Get the file content
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            // Return the file for download with appropriate content type and filename
            return File(fileBytes, "application/pdf", Path.GetFileName(filePath));
        }




        //public IActionResult PMLeasePending()
        //{
        //    return View();
        //}
        //public IActionResult PMLeaseActive()
        //{
        //    return View();
        //}
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

        //public async Task<IActionResult> PMManageUnits()
        //{
        //    try
        //    {
        //        // Fetch data from the Units table
        //        var units = await _context.Units
        //            .Where(u => u.UnitStatus == "Active") // Filter for active units
        //            .Select(u => new UnitViewModel
        //            {
        //                UnitId = u.UnitID,
        //                UnitName = u.UnitName,
        //                PricePerMonth = u.PricePerMonth,
        //                UnitStatus = u.UnitStatus
        //            })
        //            .ToListAsync();

        //        // Pass the list of units to the view
        //        return View(units);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = $"Error loading units: {ex.Message}";
        //        return View(new List<UnitViewModel>()); // Return an empty list in case of error
        //    }
        //}


        [HttpPost]
        public IActionResult UpdateLeaseStatus(LeaseValidationModel model)
        {
            // Check if at least two valid IDs are selected
            if (model.ValidIDs.Count < 2)
            {
                TempData["ShowPopup"] = true; // Indicate that the popup should be shown
                TempData["PopupMessage"] = "Please select at least two valid IDs.";
                TempData["PopupTitle"] = "Insufficient ID!";  // Set the custom title
                TempData["PopupIcon"] = "warning";  // Set the icon dynamically (can be success, error, info, warning)
                ModelState.AddModelError("", "At least two valid IDs must be selected.");
                return RedirectToAction("PMManageLease"); // Return the view with validation errors
            }

            // Check if all selected IDs are valid
            foreach (var id in model.ValidIDs)
            {
                if (!model.IDValidationResults.TryGetValue(id, out bool isValid) || !isValid)
                {
                    TempData["ShowPopup"] = true; // Indicate that the popup should be shown
                    TempData["PopupMessage"] = "Selected IDs must be valid to proceed.";
                    TempData["PopupTitle"] = "Invalid requirement!";  // Set the custom title
                    TempData["PopupIcon"] = "warning";  // Set the icon dynamically (can be success, error, info, warning)
                    ModelState.AddModelError("", "At least two valid IDs must be selected.");
                    ModelState.AddModelError("", $"The ID '{id}' is not valid.");
                    return RedirectToAction("PMManageLease");
                }
            }

            // Check if the lease agreement and security deposit are confirmed
            if (!model.LeaseAgreement || !model.SecurityDeposit)
            {
                TempData["ShowPopup"] = true; // Indicate that the popup should be shown
                TempData["PopupMessage"] = "Applicant must have signed the lease and paid the security deposit.";
                TempData["PopupTitle"] = "Missing requirement!";  // Set the custom title
                TempData["PopupIcon"] = "warning";  // Set the icon dynamically (can be success, error, info, warning)
                ModelState.AddModelError("", "At least two valid IDs must be selected.");
                ModelState.AddModelError("", "Lease agreement and security deposit must be confirmed.");
                return RedirectToAction("PMManageLease");
            }

            // Check if a payment method is selected
            if (string.IsNullOrWhiteSpace(model.PaymentMethod))
            {
                TempData["ShowPopup"] = true; // Indicate that the popup should be shown
                TempData["PopupMessage"] = "Please select the payment method used by the applicant.";
                TempData["PopupTitle"] = "Missing requirement!";  // Set the custom title
                TempData["PopupIcon"] = "warning";  // Set the icon dynamically (can be success, error, info, warning)
                ModelState.AddModelError("", "A payment method must be selected.");
                return RedirectToAction("PMManageLease");
            }

            if (model.LeaseAgreementFile == null)
            {
                TempData["ShowPopup"] = true; // Indicate that the popup should be shown
                TempData["PopupMessage"] = "Please upload the contract.";
                TempData["PopupTitle"] = "Missing requirement!";  // Set the custom title
                TempData["PopupIcon"] = "warning";  // Set the icon dynamically (can be success, error, info, warning)
                return RedirectToAction("PMManageLease");
            }


            // Process the uploaded lease agreement PDF file
            if (model.LeaseAgreementFile != null && model.LeaseAgreementFile.Length > 0)
            {
                // Define the path to save the file in wwwroot/contracts
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "contracts");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath); // Create the directory if it does not exist
                }

                var filePath = Path.Combine(folderPath, model.LeaseAgreementFile.FileName);

                // Save the file to the defined path
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.LeaseAgreementFile.CopyTo(stream);
                }

                // Update the LeaseAgreementFilePath in the Lease model
                var currentLease = _context.Leases.FirstOrDefault(l => l.LeaseID == model.LeaseID);
                if (currentLease != null)
                {
                    currentLease.LeaseAgreementFilePath = "/contracts/" + model.LeaseAgreementFile.FileName; // Store relative file path
                    _context.SaveChanges();
                }
            }



            // All validations passed, update the lease status
            // Update the LeaseStatus in the database (pseudo code)
            var lease = _context.Leases.Find(model.LeaseID);
            lease.LeaseStatus = "Active";

            var tenant = _context.Tenants.FirstOrDefault(t => t.TenantID == lease.TenantID);
            tenant.IsActualTenant = true; // Set the tenant as the actual tenant
            _context.SaveChanges();

            TempData["ShowPopup"] = true; // Indicate that the popup should be shown
            TempData["PopupMessage"] = "Lease application has been successfully confirmed";
            TempData["PopupTitle"] = "Tenant Confirmed";  // Set the custom title
            TempData["PopupIcon"] = "success";  // Set the icon dynamically (can be success, error, info, warning)
            return RedirectToAction("PMActiveLease"); // Redirect to a relevant page
        }




        public IActionResult PMPaymentsOrig()
        {
            // Fetch payments and related data from the database
            var payments = _context.Payments
                .Include(p => p.Lease) // Include the Lease navigation property
                .ThenInclude(l => l.LeaseDetails) // Include LeaseDetails for tenant's full name
                .Include(p => p.Lease.Unit) // Include Unit for unit name and monthly rent
                .Select(p => new ManagerPaymentDisplayModel
                {
                    PaymentId = p.PaymentID,
                    TenantFullName = p.Lease.LeaseDetails.FullName, // Tenant full name from LeaseDetails
                    Amount = p.Amount, // Amount from Payments
                    UnitName = p.Lease.Unit.UnitName, // Unit name from Units table
                    MonthlyRent = p.Lease.Unit.PricePerMonth, // Monthly rent from Units table
                    PaymentDate = p.PaymentDate.HasValue ? p.PaymentDate.Value.ToString("MM/dd/yyyy") : "N/A",
                    PaymentMethod = p.PaymentMethod // Payment method from Payments table
                })
                .ToList();

            return View(payments); // Pass the list of PaymentDisplayModel to the view
        }


        public IActionResult ManagerPreviewInvoice(int id)
        {
            // Retrieve the Payment record based on PaymentID (id)
            var payment = _context.Payments
                                  .Include(p => p.Lease)
                                      .ThenInclude(l => l.Unit)
                                  .Include(p => p.Lease)
                                      .ThenInclude(l => l.LeaseDetails)
                                  .FirstOrDefault(p => p.PaymentID == id);

            if (payment == null)
            {
                return NotFound(); // Handle the case where the payment record doesn't exist
            }

            // Get Lease and Tenant Details
            var lease = payment.Lease;
            var tenantName = lease?.LeaseDetails?.FullName;

            // Get Unit Name
            var unitName = lease?.Unit?.UnitName;

            // Prepare the Invoice Preview Model
            var invoicePreviewModel = new InvoicePreviewModel
            {
                PaymentId = payment.PaymentID,
                LeaseNumber = lease?.LeaseID.ToString() ?? "N/A",
                UnitName = unitName ?? "N/A",
                MonthlyRent = payment.Amount ?? 0,
                TenantName = tenantName ?? "N/A",
                PaymentDate = payment.PaymentDate?.Date ?? DateTime.Now.Date,
                PaymentTime = payment.PaymentDate?.ToString("hh:mm tt") ?? "N/A",
                PaymentMethod = payment.PaymentMethod ?? "N/A",
                PaymentStatus = payment.PaymentStatus
            };

            // Pass the model to the view
            return View(invoicePreviewModel);
        }


        public IActionResult PMRequest()
        {
            return View();
        }

        //public async Task<IActionResult> PMTenants()
        //{
        //    try
        //    {
        //        // Fetch the list of tenants with the required details
        //        var tenantList = await _context.Tenants
        //         .Include(t => t.Unit) // Example for relationships
        //         .Include(t => t.Lease) // Include the Lease entity to get lease status
        //         .Where(tenant => tenant.UserId != null && tenant.User.IsActive && tenant.IsActualTenant)
        //         .Select(tenant => new TenantViewModel
        //         {
        //             TenantID = tenant.TenantID,
        //             TenantName = tenant.User.FirstName + " " + tenant.User.LastName,
        //             Email = tenant.User.Email,
        //             UnitName = tenant.Unit.UnitName,
        //             LeaseStatus = tenant.Lease.LeaseStatus,
        //             //PaymentStatus = tenant.payment.PaymentStatus
        //         })
        //         .ToListAsync();
        //        // Pass the list of tenants to the view
        //        return View(tenantList);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = $"Error loading tenants: {ex.Message}";
        //        return View(new List<TenantViewModel>()); // Return an empty list in case of error
        //    }
        //}
        //revert
        public async Task<IActionResult> PMTenants()
        {
            try
            {
                // Fetch the list of property managers with email
                var tenantList = await _context.Tenants
                    .Where(tenant => tenant.UserId != null && tenant.User.IsActive && tenant.IsActualTenant) // Filter by active users
                    .Select(tenant => new TenantViewModel
                    {
                        TenantID = tenant.TenantID,
                        TenantName = tenant.User.FirstName + " " + tenant.User.LastName, // Combine FirstName and LastName
                        Email = tenant.User.Email, // Get the Email of the User related to the Manager
                    })
                    .ToListAsync();

                // Pass the list of managers to the view
                return View(tenantList);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading managers: {ex.Message}";
                return View(new List<TenantViewModel>()); // Return an empty list in case of error
            }
        }
        //public async Task<IActionResult> PMTenants()
        //{
        //    try
        //    {
        //        // Fetch the list of tenants along with related user, profile, unit, and lease status
        //        var tenantList = await _context.Tenants
        //            .Where(tenant => tenant.UserId != null && tenant.User.IsActive && tenant.IsActualTenant) // Filter by active users
        //            .Include(t => t.User) // Include the related User entity
        //            .ThenInclude(u => u.Profile) // Include the Profile associated with the User
        //            //.Include(t => t.Unit) // Include the related Unit entity
        //            //.Include(t => t.Lease) // Include the Lease entity to get lease status
        //            .Select(tenant => new TenantViewModel
        //            {
        //                TenantID = tenant.TenantID,
        //                TenantName = tenant.User.FirstName + " " + tenant.User.LastName, // Combine FirstName and LastName
        //                Email = tenant.User.Email, // Get the Email of the User related to the Tenant
        //                PhoneNumber = tenant.User.Profile.PhoneNumber, // Access the PhoneNumber from Profile
        //                //UnitName = tenant.Unit.UnitName, // Access the Unit's name
        //                //LeaseStatus = tenant.Lease.LeaseStatus, // Access the lease status (e.g., Active, Expired)
        //                IsActive = tenant.User.IsActive
        //            })
        //            .ToListAsync();

        //        // Pass the list of tenants to the view
        //        return View(tenantList);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = $"Error loading tenants: {ex.Message}";
        //        return View(new List<TenantViewModel>()); // Return an empty list in case of error
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> AddTenant(AddTenantViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest("All fields are required.");
        //    }

        //    try
        //    {
        //        // Hash the password using BCrypt
        //        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
        //        // Create instance of the Users table
        //        var newUser = new User
        //        {
        //            FirstName = model.FirstName,
        //            LastName = model.LastName,
        //            Email = model.Email,
        //            Password = hashedPassword, // hashed password
        //            Role = "Tenant", // Default role for all tenants
        //            TermsAndConditions = true // Default value
        //        };

        //        _context.Users.Add(newUser);
        //        await _context.SaveChangesAsync();

        //        // Get the generated UserId
        //        int userId = newUser.UserID;


        //        // Create instance of the Staffs table
        //        var newTenant = new Tenant
        //        {
        //            UserId = userId,
        //            IsActualTenant = true,
        //            //Email = model.Email,
        //            //ManagerID = ManagerID,

        //        };

        //        _context.Tenants.Add(newTenant);
        //        await _context.SaveChangesAsync();

        //        TempData["ShowPopup"] = true; // Indicate that the popup should be shown
        //        TempData["PopupMessage"] = "Tenant added successfully!";
        //        TempData["PopupTitle"] = "Success!";  // Set the custom title
        //        TempData["PopupIcon"] = "success";  // Set the icon dynamically (can be success, error, info, warning)

        //        return RedirectToAction("PMTenants");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}

        [HttpPost]
        public IActionResult DeleteTenant(int id)
        {
            // Find the PropertyManager record using the provided Manager ID
            var tenant = _context.Tenants.FirstOrDefault(t => t.TenantID == id);
            if (tenant == null)
            {
                return NotFound("Tenant not found.");
            }

            // Find the associated user using the UserId from the PropertyManager record
            var user = _context.Users.FirstOrDefault(u => u.UserID == tenant.UserId);
            if (user == null)
            {
                return NotFound("Associated user not found.");
            }

            // Set the IsActive field to false (soft delete)
            user.IsActive = false;

            // Save changes to the database
            _context.SaveChanges();

            // Set TempData for the popup
            TempData["ShowPopup"] = true;
            TempData["PopupMessage"] = "Tenant deleted successfully!";
            TempData["PopupTitle"] = "Success!";
            TempData["PopupIcon"] = "success"; // Set the icon dynamically (success, error, info, warning)

            return RedirectToAction("PMTenants");
        }


        public async Task<IActionResult> PMManageUsers()
        {
            try
            {
                // Fetch the list of property managers with email
                var managerList = await _context.PropertyManagers
                    .Where(manager => manager.UserId != null && manager.User.IsActive) // Filter by active users
                    .Select(manager => new ManagerViewModel
                    {
                        ManagerID = manager.ManagerID,
                        ManagerName = manager.User.FirstName + " " + manager.User.LastName, // Combine FirstName and LastName
                        Email = manager.User.Email, // Get the Email of the User related to the Manager
                    })
                    .ToListAsync();

                // Pass the list of managers to the view
                return View(managerList);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading managers: {ex.Message}";
                return View(new List<ManagerViewModel>()); // Return an empty list in case of error
            }
        }



        [HttpPost]
        public async Task<IActionResult> AddManager(AddManagerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("All fields are required.");
            }

            try
            {
                // Hash the password using BCrypt
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                // Create instance of the Users table
                var newUser = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = hashedPassword, // hashed password
                    Role = "Property Manager", // Default role for all property manager
                    TermsAndConditions = true // Default value
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // Get the generated UserId
                int userId = newUser.UserID;

              
                // Create instance of the Staffs table
                var newManager = new PropertyManager
                {
                    UserId = userId,
                    //Email = model.Email,
                    //ManagerID = ManagerID,

                };

                _context.PropertyManagers.Add(newManager);
                await _context.SaveChangesAsync();

                TempData["ShowPopup"] = true; // Indicate that the popup should be shown
                TempData["PopupMessage"] = "Manager added successfully!";
                TempData["PopupTitle"] = "Success!";  // Set the custom title
                TempData["PopupIcon"] = "success";  // Set the icon dynamically (can be success, error, info, warning)

                return RedirectToAction("PMManageUsers");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult DeleteManager(int id)
        {
            // Find the PropertyManager record using the provided Manager ID
            var manager = _context.PropertyManagers.FirstOrDefault(m => m.ManagerID == id);
            if (manager == null)
            {
                return NotFound("Property Manager not found.");
            }

            // Find the associated user using the UserId from the PropertyManager record
            var user = _context.Users.FirstOrDefault(u => u.UserID == manager.UserId);
            if (user == null)
            {
                return NotFound("Associated user not found.");
            }

            // Set the IsActive field to false (soft delete)
            user.IsActive = false;

            // Save changes to the database
            _context.SaveChanges();

            // Set TempData for the popup
            TempData["ShowPopup"] = true;
            TempData["PopupMessage"] = "Property Manager deleted successfully!";
            TempData["PopupTitle"] = "Success!";
            TempData["PopupIcon"] = "success"; // Set the icon dynamically (success, error, info, warning)

            return RedirectToAction("PMManageUsers");
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

        public IActionResult PMStaff()
        {
            // Retrieve all staff from the database, including related User information
            var staffList = _context.Staffs
                .Where(staff => staff.UserId != null && staff.User.IsActive) // Filter by active users
                .Select(staff => new
                {
                    StaffID = staff.StaffID,
                    StaffName = staff.UserId != null ? staff.User.FirstName + " " + staff.User.LastName : "Unknown",
                    StaffRole = staff.StaffRole,
                    Shift = (staff.ShiftStartTime.HasValue && staff.ShiftEndTime.HasValue)
                        ? (staff.ShiftStartTime.Value.Hour == 6 && staff.ShiftEndTime.Value.Hour == 14 ? "First"
                        : staff.ShiftStartTime.Value.Hour == 14 && staff.ShiftEndTime.Value.Hour == 22 ? "Second"
                        : staff.ShiftStartTime.Value.Hour == 22 || staff.ShiftStartTime.Value.Hour == 0 ? "Third"
                        : "Unknown")
                        : "Unknown",
                    Availability = staff.IsVacant ? "Vacant" : "Occupied"
                })
                .ToList()
                .Select(staff => new StaffViewModel
                {
                    StaffID = staff.StaffID,
                    StaffName = staff.StaffName,
                    StaffRole = staff.StaffRole,
                    Shift = staff.Shift,
                    Availability = staff.Availability
                })
                .ToList();

            return View(staffList);
        }

        [HttpPost]
        public async Task<IActionResult> AddStaff(AddStaffViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("All fields are required.");
            }

            try
            {
                // Hash the password using BCrypt
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                // Create instance of the Users table
                var newUser = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = hashedPassword, // hashedpassword
                    Role = "Staff", // Default role for all staff
                    TermsAndConditions = true // Default value
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // Get the generated UserId
                int userId = newUser.UserID;

                // Set default shift times based on the selected shift
                TimeOnly? shiftStartTime = null;
                TimeOnly? shiftEndTime = null;

                switch (model.Shift)
                {
                    case "First":
                        shiftStartTime = TimeOnly.FromTimeSpan(new TimeSpan(6, 0, 0)); // 6 AM
                        shiftEndTime = TimeOnly.FromTimeSpan(new TimeSpan(14, 0, 0)); // 2 PM
                        break;
                    case "Second":
                        shiftStartTime = TimeOnly.FromTimeSpan(new TimeSpan(14, 0, 0)); // 2 PM
                        shiftEndTime = TimeOnly.FromTimeSpan(new TimeSpan(22, 0, 0)); // 10 PM
                        break;
                    case "Third":
                        shiftStartTime = TimeOnly.FromTimeSpan(new TimeSpan(22, 0, 0)); // 10 PM
                        shiftEndTime = TimeOnly.FromTimeSpan(new TimeSpan(6, 0, 0)); // 6 AM
                        break;
                    default:
                        return BadRequest("Invalid shift selected.");
                }

                // Create instance of the Staffs table
                var newStaff = new Staff
                {
                    UserId = userId,
                    StaffRole = model.Role,
                    ShiftStartTime = shiftStartTime,
                    ShiftEndTime = shiftEndTime,
                    IsVacant = true
                };

                _context.Staffs.Add(newStaff);
                await _context.SaveChangesAsync();

                TempData["ShowPopup"] = true; // Indicate that the popup should be shown
                TempData["PopupMessage"] = "Staff added successfully!";
                TempData["PopupTitle"] = "Success!";  // Set the custom title
                TempData["PopupIcon"] = "success";  // Set the icon dynamically (can be success, error, info, warning)

                return RedirectToAction("PMStaff");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //[HttpPost]
        public IActionResult DeleteStaff(int id)
        {
            // Find the staff record using the provided staff ID
            var staff = _context.Staffs.FirstOrDefault(s => s.StaffID == id);
            if (staff == null)
            {
                return NotFound("Staff not found.");
            }

            // Find the associated user using the UserId from the staff record
            var user = _context.Users.FirstOrDefault(u => u.UserID == staff.UserId);
            if (user == null)
            {
                return NotFound("Associated user not found.");
            }

            // Set the IsActive field to false (soft delete)
            user.IsActive = false;

            // Save changes to the database
            _context.SaveChanges();

            TempData["ShowPopup"] = true; // Indicate that the popup should be shown
            TempData["PopupMessage"] = "Staff deleted successfully!";
            TempData["PopupTitle"] = "Success!";  // Set the custom title
            TempData["PopupIcon"] = "success";  // Set the icon dynamically (can be success, error, info, warning)

            return RedirectToAction("PMStaff");
        }

        //Profile
        public IActionResult PMProfilePage()
        {
            //var model = new Profile(); // Ensure it's initialized
            //return View(model);

            // Retrieve the logged-in user's ID from session (as string) and convert to int
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                // If the session doesn't contain a valid UserId, redirect to login page
                //TempData["ErrorMessage"] = "User profile not found.";
                return RedirectToAction("Login", "Login");
            }

            // Log the UserId to the console (this will output to the console/logs)
            _logger.LogInformation($"Logged-in UserId: {userId}");


            // Fetch user profile and user details
            var profile = _context.UserProfiles.FirstOrDefault(p => p.Id == userId);
            var user = _context.Users.FirstOrDefault(u => u.UserID == userId);

            if (profile == null)
            {
                // Create a default profile for new users
                profile = new Profile
                {
                    FirstName = "",
                    LastName = "",

                    Email = "",
                    PhoneNumber = "",
                    Address = ""
                };
                _context.UserProfiles.Add(profile);
                _context.SaveChanges();
            }
            return View(profile);
        }

        public IActionResult PMEditProfile2()
        {
            // Retrieve the logged-in user's ID from session (as string) and convert to int
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                // If the session doesn't contain a valid UserId, redirect to login page
                return RedirectToAction("Login", "Login");
            }
            var profile = _context.UserProfiles.FirstOrDefault(p => p.Id == userId);
            return View(profile);
        }


        // Update profile
        [HttpPost]
        public IActionResult UpdateProfile2(Profile model)
        {
            if (!ModelState.IsValid)
            {
                // Return the view with the current model and validation errors
                TempData["ErrorMessage"] = "Please fill out all required fields.";
                return View("PMEditProfile2", model);
            }

            // Retrieve the logged-in user's ID from session (as string) and convert to int
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                // If the session doesn't contain a valid UserId, redirect to login page
                TempData["ErrorMessage"] = "User not found. Please log in again.";
                return RedirectToAction("Login", "Login");
            }

            var profile = _context.UserProfiles.FirstOrDefault(p => p.Id == userId);
            var user = _context.Users.FirstOrDefault(u => u.UserID == userId);

                     
            // Proceed with the update if everything is valid
            if (profile != null && user != null)
            {
                // Only update the fields if the user has provided new values
                if (!string.IsNullOrEmpty(model.FirstName))
                {
                    profile.FirstName = model.FirstName;
                    user.FirstName = model.FirstName;
                }
                if (!string.IsNullOrEmpty(model.LastName))
                {
                    profile.LastName = model.LastName;
                    user.LastName = model.LastName;
                }
                if (!string.IsNullOrEmpty(model.Email))
                {
                    profile.Email = model.Email;
                    user.Email = model.Email;
                }
                if (!string.IsNullOrEmpty(model.PhoneNumber))
                {
                    profile.PhoneNumber = model.PhoneNumber;
                }
                if (!string.IsNullOrEmpty(model.Address))
                {
                    profile.Address = model.Address;
                }

                // Save changes to the database
                _context.SaveChanges();

                // Set success message
                TempData["SuccessMessage"] = "Profile updated successfully!";
            }

            // Redirect to the profile page
            return RedirectToAction("PMProfilePage");
        }




        public IActionResult PMChangePassword()
        {
            return View();
        }

        // Change or Update the password
        [HttpPost]
        public IActionResult PMChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            // Retrieve the logged-in user's ID from session (as string) and convert to int
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserID == userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("PMChangePassword");
            }

            // Check if the current password matches the hashed password (using BCrypt)
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))  // Compare hashed password
            {
                TempData["ErrorMessage"] = "Current password is incorrect.";
                return RedirectToAction("PMChangePassword");
            }

            // Check if the new password and confirm password match
            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "New password and confirm password do not match.";
                return RedirectToAction("PMChangePassword");
            }

            // Validate that the new password is between 15 and 64 characters
            if (newPassword.Length < 15 || newPassword.Length > 64)
            {
                TempData["ErrorMessage"] = "Password must be between 15 and 64 characters.";
                return RedirectToAction("PMChangePassword");
            }

            // Hash the new password before saving it to the database
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);  // Hash the new password

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToAction("PMProfilePage");
        }

        //// Display the profile page
        //public IActionResult ProfilePage()
        //{
        //    // Retrieve the logged-in user's ID from session (as string) and convert to int
        //    var userIdString = HttpContext.Session.GetString("UserId");
        //    if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        //    {
        //        // If the session doesn't contain a valid UserId, redirect to login page
        //        //TempData["ErrorMessage"] = "User profile not found.";
        //        return RedirectToAction("Login", "Login");
        //    }

        //    // Log the UserId to the console (this will output to the console/logs)
        //    _logger.LogInformation($"Logged-in UserId: {userId}");


        //    // Fetch user profile and user details
        //    var profile = _context.UserProfiles.FirstOrDefault(p => p.Id == userId);
        //    var user = _context.Users.FirstOrDefault(u => u.UserID == userId);

        //    if (profile == null)
        //    {
        //        // Create a default profile for new users
        //        profile = new Profile
        //        {
        //            Id = user.UserID,      // Link the Profile to the newly created User
        //            FirstName = "",
        //            LastName = "",

        //            Email = "",
        //            PhoneNumber = "",
        //            Address = ""
        //        };
        //        _context.UserProfiles.Add(profile);
        //        _context.SaveChanges();
        //    }
        //    return View(profile);
        //}


    }
}
