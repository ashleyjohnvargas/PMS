using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Models;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace PMS.Controllers
{
    public class ATenantController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ATenantController> _logger;
        public ATenantController(ApplicationDbContext context, ILogger<ATenantController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult ATenantHome()
        {
            return View();
        }

        public IActionResult ATenantContact()
        {
            return View();
        }

        public async Task<IActionResult> ATenantUnits()
        {
            var units = await _context.Units
            .Where(u => u.UnitStatus == "Active" && u.AvailabilityStatus == "Available") // Filter for Active units
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
        public IActionResult ATenantDetails(int id)
        {
            Console.WriteLine("Unit ID of the selected unit: " + id); // For debugging
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
            Console.WriteLine("Unit ID to be passed to Book This Unit Now: " + model.UnitId); // For debugging

            return View(model); // Render the PTenantDetails view with the model
        }

        public IActionResult ATenantApply(int unitId)
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
                return RedirectToAction("ATenantApply", new { unitId = leaseApplication.UnitId });
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

            //// After the active tenant makes a lease, set the AvailabilityStatus of the selected unit into "Unavailable"
            //// Update the unit's AvailabilityStatus to "Unavailable"
            //var unit = _context.Units.FirstOrDefault(u => u.UnitID == leaseApplication.UnitId);
            //if (unit != null)
            //{
            //    unit.AvailabilityStatus = "Occupied";
            //    _context.SaveChanges(); // Save changes to update the unit status
            //}
            //else
            //{
            //    ModelState.AddModelError("", "The specified unit does not exist.");
            //    return RedirectToAction("ATenantApply", new { unitId = leaseApplication.UnitId });
            //}
 

            try
            {
                TempData["ShowPopup"] = true; // Indicate that the popup should be shown
                TempData["PopupMessage"] = "Your application has been sent successfully!";
                TempData["PopupTitle"] = "Success!";  // Set the custom title
                TempData["PopupIcon"] = "success";  // Set the icon dynamically (can be success, error, info, warning)
                return RedirectToAction("ATenantHome"); // Redirect to a success page or confirmation
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the lease application: " + ex.Message);
                return RedirectToAction("ATenantApply", new { unitId = leaseApplication.UnitId });
            }
        }



        public IActionResult ATenantLease()
        {
            // Fetch the logged-in user's ID from the session
            var userId = HttpContext.Session.GetInt32("UserId");
            //if (string.IsNullOrEmpty(userId))
            //{
            //    return RedirectToAction("Login", "Login"); // Redirect to login if no user is logged in
            //}

            // Get the tenant's details based on the UserId
            var tenant = _context.Tenants.FirstOrDefault(t => t.UserId == userId);
            if (tenant == null)
            {
                return NotFound("Tenant not found.");
            }

            // Get the lease details for the tenant
            var lease = _context.Leases
                .Include(l => l.Unit) // Include Unit details for property information
                .Include(l => l.LeaseDetails) // Include LeaseDetails for tenant info
                .FirstOrDefault(l => l.TenantID == tenant.TenantID);

            if (lease == null)
            {
                return NotFound("Lease not found.");
            }

            // Get the current date
            var currentDate = DateTime.Now;

            // Calculate the first day of the next month
            var firstDayNextMonth = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(1);

            // Construct the model for the view
            var model = new TenantLeaseViewModel
            {
                LeaseID = lease.LeaseID,
                PropertyName = lease.Unit.UnitName, // Assuming UnitName contains the property name
                Address = $"{lease.Unit.Location} {lease.Unit.City} {lease.Unit.Town} {lease.Unit.State},{lease.Unit.ZipCode} {lease.Unit.Country}",
                StartDate = lease.LeaseStartDate?.ToString("MMMM d, yyyy") ?? "N/A",
                EndDate = lease.LeaseEndDate?.ToString("MMMM d, yyyy") ?? "N/A",
                MonthlyRent = lease.Unit.PricePerMonth, // Keep as numeric value (decimal, double)
                SecurityDeposit = string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-PH"), "{0:C}", lease.Unit.SecurityDeposit),
                Status = lease.LeaseStatus,
                TenantName = lease.LeaseDetails.FullName,
                Contact = lease.LeaseDetails.Email,
                Phone = lease.LeaseDetails.ContactNumber,
                DueDate = firstDayNextMonth.ToString("MMMM d, yyyy") 
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult PayLease(PaymentViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(model);  // Return the same view with validation errors
            //}

            if (model.Amount != model.MonthlyRent)
            {
                TempData["ShowPopup"] = true; // Indicate that the popup should be shown
                TempData["PopupMessage"] = "Please pay the exact amount due.";
                TempData["PopupTitle"] = "Invalid Amount!";
                TempData["PopupIcon"] = "warning";  // Set the icon dynamically (can be success, error, info, warning)
                return RedirectToAction("ATenantLease");
            }

            // Fetch the lease details based on LeaseId
            var lease = _context.Leases.FirstOrDefault(l => l.LeaseID == model.LeaseId);
            if (lease == null)
            {
                return NotFound("Lease not found.");
            }

            // Create a new instance of the Payments table
            var payment = new Payment
            {
                LeaseID = lease.LeaseID,                // LeaseId from the form
                PaymentDate = DateTime.Now,              // Current date and time for PaymentDate
                Amount = model.Amount,                   // Amount from the form
                PaymentMethod = model.PaymentMethod,     // Selected PaymentMethod from the form
                PaymentStatus = "Paid"                   // Default PaymentStatus as "Paid"
            };

            // Save the new payment record to the database
            _context.Payments.Add(payment);
            _context.SaveChanges();

            TempData["ShowPopup"] = true; // Indicate that the popup should be shown
            TempData["PopupMessage"] = "Your payment has been sent successfully!";
            TempData["PopupTitle"] = "Success!";
            TempData["PopupIcon"] = "success";  // Set the icon dynamically (can be success, error, info, warning)

            // Redirect to the appropriate page after successful payment
            return RedirectToAction("ATenantLease");
        }



        public IActionResult ATenantPayment()
        {
            // Get the UserId from the session
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // Handle the case where the user is not logged in or session is missing
                TempData["ErrorMessage"] = "You must be logged in to view your payments.";  
                return RedirectToAction("Login", "Login"); // Redirect to the login page or appropriate action
            }

            // Retrieve the Tenant associated with the UserId
            var tenant = _context.Tenants.FirstOrDefault(t => t.UserId == userId);

            if (tenant == null)
            {
                // Handle the case where no tenant is found for the UserId
                TempData["ErrorMessage"] = "No tenant found for this user.";
                return RedirectToAction("Index", "Home"); // Redirect to a general home page or appropriate action
            }

            // Retrieve all the LeaseIDs associated with the tenant
            var leaseIds = _context.Leases
                .Where(l => l.TenantID == tenant.TenantID)
                .Select(l => l.LeaseID)
                .ToList();

            // Retrieve all the payments associated with the LeaseIDs for the current user
            var payments = _context.Payments
                .Where(p => leaseIds.Contains((int)p.LeaseID))
                .Select(p => new PaymentsDisplayModel
                {
                    PaymentID = p.PaymentID,
                    PaymentDate = p.PaymentDate,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    PaymentStatus = p.PaymentStatus
                })
                .ToList();

            return View(payments);
        }


        public IActionResult PreviewInvoice(int id)
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


        public IActionResult MakeRequest()
        {
            return View();
        }


        [HttpPost]
        public IActionResult SendRequest(RequestFormModel form)
        {
            try
            {
                // Step 1: Retrieve the TenantID from the session
                int? userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return BadRequest("User is not logged in.");
                }

                var tenant = _context.Tenants.FirstOrDefault(t => t.UserId == userId);
                if (tenant == null)
                {
                    return NotFound("Tenant not found.");
                }

                // Step 2: Fetch Staffs based on RequestType
                var staffRole = form.RequestType switch
                {
                    "Electrical" => "Electrician",
                    "Plumbing" => "Plumber",
                    "Structural" => "Carpenter",
                    "Appliance" => "Technician",
                    "Internet" => "IT Technician",
                    _ => throw new Exception("Invalid Request Type")
                };

                // Get the current date and time
                DateTime currentDateTime = DateTime.Now;

                // Fetch staff with matching role and current shift
                var availableStaff = _context.Staffs
                    .Where(staff => staff.StaffRole == staffRole && staff.IsVacant)
                    .AsEnumerable() // Switch to in-memory evaluation
                    .Where(staff => staff.ShiftStartTime.HasValue &&
                                    staff.ShiftEndTime.HasValue &&
                                    currentDateTime.TimeOfDay >= staff.ShiftStartTime.Value.ToTimeSpan() &&
                                    currentDateTime.TimeOfDay <= staff.ShiftEndTime.Value.ToTimeSpan())
                    .OrderBy(staff => staff.StaffID) // Ensure consistent assignment
                    .FirstOrDefault();



                if (availableStaff == null)
                {
                    TempData["ShowPopup"] = true; // Indicate that the popup should be shown
                    TempData["PopupMessage"] = "Cannot make a request now. No staff is available as of the moment.";
                    TempData["PopupTitle"] = "Sorry";
                    TempData["PopupIcon"] = "info";  // Set the icon dynamically (can be success, error, info, warning)
                    return RedirectToAction("MakeRequest");
                }

                // Step 3: Create a new Request instance
                var newRequest = new Request
                {
                    TenantID = tenant.TenantID,
                    StaffID = availableStaff.StaffID,
                    RequestType = form.RequestType,
                    RequestDescription = form.RequestDescription,
                    RequestDateTime = currentDateTime,
                    RequestStatus = "Pending", // Default status
                };

                // Step 4: Update the staff's IsVacant field
                availableStaff.IsVacant = false;

                // Step 4: Save the new request to the database
                _context.Requests.Add(newRequest);
                _context.Staffs.Update(availableStaff);
                _context.SaveChanges();

                TempData["ShowPopup"] = true; // Indicate that the popup should be shown
                TempData["PopupMessage"] = "Request has been successfully sent!";
                TempData["PopupTitle"] = "Success!";
                TempData["PopupIcon"] = "success";  // Set the icon dynamically (can be success, error, info, warning)
                return RedirectToAction("ATenantMaintenance");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        public async Task<IActionResult> ATenantMaintenance()
        {
            // Get the logged-in user's ID from the session
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Login"); // Redirect to login if not logged in
            }

            // Find the Tenant based on the UserId
            var tenant = await _context.Tenants
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (tenant == null)
            {
                return View(new List<RequestViewModel>()); // No tenant found, pass an empty list
            }

            // Fetch all requests for the Tenant
            var requests = await _context.Requests
                .Include(r => r.Tenant)
                .Include(r => r.Staff)
                .ThenInclude(s => s.User)
                .Where(r => r.TenantID == tenant.TenantID)
                .ToListAsync();

            // Map the requests to a ViewModel
            var requestViewModels = new List<RequestViewModel>();

            foreach (var request in requests)
            {
                // Fetch the unit details via Lease and Unit
                var lease = await _context.Leases
                    .Include(l => l.Unit)
                    .FirstOrDefaultAsync(l => l.TenantID == tenant.TenantID);

                var unitName = lease?.Unit?.UnitName ?? "N/A";

                // Prepare the ViewModel
                var requestViewModel = new RequestViewModel
                {
                    Tenant = $"{tenant.User?.FirstName} {tenant.User?.LastName}",
                    Unit = unitName,
                    RequestDate = request.RequestDateTime?.ToString("MM/dd/yyyy hh:mm tt") ?? "N/A",
                    Issue = request.RequestType ?? "N/A",
                    AssignedStaff = request.Staff?.User != null
                        ? $"{request.Staff.User.FirstName} {request.Staff.User.LastName}"
                        : "Unassigned",
                    Status = request.RequestStatus ?? "N/A"
                };

                requestViewModels.Add(requestViewModel);
            }

            // Pass the ViewModel list to the view
            return View(requestViewModels);
        }

        public IActionResult ATenantProfilePage()
        {
            //var model = new Profile(); // Ensure it's initialized
            //return View(model);

            // Retrieve the logged-in user's ID from session (as string) and convert to int
            var userId = HttpContext.Session.GetInt32("UserId"); // Retrieve as an integer
            if (!userId.HasValue)
            {
                // Redirect to login if UserId is not in the session
                TempData["ErrorMessage"] = "You need to log in to view your profile.";
                return RedirectToAction("Login", "Login");
            }

            // Log the UserId to the console (this will output to the console/logs)
            _logger.LogInformation($"Logged-in UserId: {userId}");


            // Fetch user profile and user details
            var profile = _context.UserProfiles.FirstOrDefault(p => p.Id == userId);
            var user = _context.Users.FirstOrDefault(u => u.UserID == userId);

            //if (profile == null)
            //{
            //    // Create a default profile for new users
            //    profile = new Profile
            //    {
            //        FirstName = "",
            //        LastName = "",

            //        Email = "",
            //        PhoneNumber = "",
            //        Address = ""
            //    };
            //    _context.UserProfiles.Add(profile);
            //    _context.SaveChanges();
            //}
            return View(profile);
        }

        public IActionResult ATenantEditProfile2()
        {

            // Retrieve the logged-in user's ID from session (as string) and convert to int
            var userId = HttpContext.Session.GetInt32("UserId"); // Retrieve as an integer
            if (!userId.HasValue)
            {
                // Redirect to login if UserId is not in the session
                TempData["ErrorMessage"] = "You need to log in to view your profile.";
                return RedirectToAction("Login", "Login");
            }
            var profile = _context.UserProfiles.FirstOrDefault(p => p.Id == userId);
            return View(profile);
        }

        // Update profile
        [HttpPost]
        public IActionResult UpdateProfile2(Profile model)
        {
            // Ensure TempData["ErrorMessage"] is initialized
            TempData["ErrorMessage"] = TempData["ErrorMessage"] ?? string.Empty;
            TempData["SuccessMessage"] = TempData["SuccessMessage"] ?? string.Empty;


            if (!ModelState.IsValid)
            {
                // Return the view with the current model and validation errors
                TempData["ErrorMessage"] = "Please fill out all required fields.";
                return View("ATenantEditProfile2", model);
            }

            // Retrieve the logged-in user's ID from session (as string) and convert to int
            var userId = HttpContext.Session.GetInt32("UserId"); // Retrieve as an integer
            if (!userId.HasValue)
            {
                // If the session doesn't contain a valid UserId, redirect to login page
                TempData["ErrorMessage"] = "User not found. Please log in again.";
                return RedirectToAction("Login", "Login");
            }

            var profile = _context.UserProfiles.FirstOrDefault(p => p.Id == userId);
            var user = _context.Users.FirstOrDefault(u => u.UserID == userId);

            //// Check if any required fields are null or empty
            //if (string.IsNullOrEmpty(profile.FirstName))
            //{
            //    ModelState.AddModelError("FirstName", "First Name cannot be null.");
            //}
            //if (string.IsNullOrEmpty(profile.LastName))
            //{
            //    ModelState.AddModelError("LastName", "Last Name cannot be null.");
            //}
            //if (string.IsNullOrEmpty(profile.Email))
            //{
            //    ModelState.AddModelError("Email", "Email cannot be null.");
            //}

            // If ModelState is invalid, show errors and return to the form
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors in the form.";
                return View("ATenantEditProfile2", model);
            }

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
            return RedirectToAction("ATenantProfilePage");
        }


        //Display ChangePaassword
        public IActionResult ATenantChangePassword()
        {
            return View();
        }

        // Change or Update the password
        [HttpPost]
        public IActionResult ATenantChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            // Retrieve the logged-in user's ID from session (as string) and convert to int
            var userId = HttpContext.Session.GetInt32("UserId"); // Retrieve as an integer
            if (!userId.HasValue)
            {
                // Redirect to login if UserId is not in the session
                TempData["ErrorMessage"] = "You need to log in.";
                return RedirectToAction("Login", "Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserID == userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("ATenantChangePassword");
            }

            // Check if the current password matches the hashed password (using BCrypt)
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))  // Compare hashed password
            {
                TempData["ErrorMessage"] = "Current password is incorrect.";
                return RedirectToAction("ATenantChangePassword");
            }

            // Check if the new password and confirm password match
            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "New password and confirm password do not match.";
                return RedirectToAction("ATenantChangePassword");
            }

            // Validate that the new password is between 15 and 64 characters
            if (newPassword.Length < 15 || newPassword.Length > 64)
            {
                TempData["ErrorMessage"] = "Password must be between 15 and 64 characters.";
                return RedirectToAction("ATenantChangePassword");
            }

            // Hash the new password before saving it to the database
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);  // Hash the new password

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToAction("ATenantProfilePage");
        }


    }
}
