using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PMS.Controllers
{
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StaffController> _logger;
        public StaffController(ApplicationDbContext context, ILogger<StaffController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult SHomePage()
        {
            return View();
        }
        public IActionResult SMaintenanceAssignment()
        {
            // Get the logged-in user's ID from the session
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Handle the case where the session does not have a UserId (e.g., redirect to login)
                return RedirectToAction("Login", "Login");
            }

            // Retrieve the staff member associated with the logged-in user
            var staff = _context.Staffs
                .Include(s => s.Requests) // Include requests assigned to the staff
                    .ThenInclude(r => r.Tenant) // Include Tenant for each request
                        .ThenInclude(t => t.User) // Include User for each tenant
                .FirstOrDefault(s => s.UserId == userId);

            if (staff == null)
            {
                // Handle the case where no staff is found (e.g., unauthorized access)
                return Unauthorized();
            }

            // Get all requests assigned to this staff with RequestStatus = "Pending"
            var requests = staff.Requests
                .Where(r => r.RequestStatus != "Completed")
                .ToList();


            // Build the view model to pass to the view
            var model = requests.Select(request =>
            {
                // Get the unit name by matching TenantID in Lease and UnitId in Units
                var lease = _context.Leases
                    .Include(l => l.Unit)
                    .FirstOrDefault(l => l.TenantID == request.TenantID);

                var unitName = lease?.Unit?.UnitName ?? "N/A"; // Default to "N/A" if no unit is found

                // Get tenant's name from the request
                var tenantName = request.Tenant?.User != null
                    ? $"{request.Tenant.User.FirstName} {request.Tenant.User.LastName}"
                    : "N/A"; // Default to "N/A" if tenant or user is null

                return new MaintenanceAssignmentViewModel
                {
                    RequestID = request.RequestID,
                    UnitName = unitName,
                    DateRequested = request.RequestDateTime?.ToString("yyyy-MM-dd hh:mm tt") ?? "N/A",
                    RequestTask = request.RequestDescription ?? "N/A",
                    Category = request.RequestType ?? "N/A",
                    Status = request.RequestStatus ?? "N/A",
                    TenantName = tenantName // Add tenant's name
                };
            }).ToList();

            return View(model);
        }


        public IActionResult SMaintenanceHistory()
        {
            // Get the logged-in user's ID from the session
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Handle the case where the session does not have a UserId (e.g., redirect to login)
                return RedirectToAction("Login", "Login");
            }

            // Retrieve the staff member associated with the logged-in user
            var staff = _context.Staffs
                .Include(s => s.Requests) // Include requests assigned to the staff
                    .ThenInclude(r => r.Tenant) // Include Tenant for each request
                .FirstOrDefault(s => s.UserId == userId);

            if (staff == null)
            {
                // Handle the case where no staff is found (e.g., unauthorized access)
                return Unauthorized();
            }

            // Get all requests assigned to this staff where RequestStatus is not "Pending"
            var requests = staff.Requests
                .Where(r => r.RequestStatus == "Completed")
                .ToList();

            // Build the view model to pass to the view
            var model = requests.Select(request =>
            {
                // Get the unit name by matching TenantID in Lease and UnitId in Units
                var lease = _context.Leases
                    .Include(l => l.Unit)
                    .FirstOrDefault(l => l.TenantID == request.TenantID);

                var unitName = lease?.Unit?.UnitName ?? "N/A"; // Default to "N/A" if no unit is found

                return new MaintenanceHistoryViewModel
                {
                    RequestID = request.RequestID,
                    UnitName = unitName,
                    DateRequested = request.RequestDateTime?.ToString("yyyy-MM-dd hh:mm tt") ?? "N/A",
                    RequestTask = request.RequestDescription ?? "N/A",
                    Category = request.RequestType ?? "N/A",
                    Status = request.RequestStatus ?? "N/A",
                    DateStarted = request.RequestStartDateTime?.ToString("yyyy-MM-dd hh:mm tt") ?? "N/A",
                    DateFinished = request.CompletedDateTime?.ToString("yyyy-MM-dd hh:mm tt") ?? "N/A"
                };
            }).ToList();

            return View(model);
        }


        //[HttpPost]
        public IActionResult StartRequest(int id)
        {
            // Retrieve the request by its ID
            var request = _context.Requests.FirstOrDefault(r => r.RequestID == id);

            if (request == null)
            {
                // Handle the case where the request does not exist
                return NotFound();
            }

            // Update the RequestStartDateTime to the current date and time
            request.RequestStartDateTime = DateTime.Now;

            // Update the RequestStatus to "Ongoing"
            request.RequestStatus = "Ongoing";

            // Save the changes to the database
            _context.SaveChanges();

            TempData["ShowPopup"] = true; // Indicate that the popup should be shown
            TempData["PopupMessage"] = "Request has been successfully started!";
            TempData["PopupTitle"] = "Request Started!";  // Set the custom title
            TempData["PopupIcon"] = "success";  // Set the icon dynamically (can be success, error, info, warning)

            // Redirect to the SMaintenanceAssignment view or wherever is appropriate
            return RedirectToAction("SMaintenanceAssignment");
        }


        [HttpPost]
        public IActionResult CompleteRequest(CompleteRequestViewModel model)
        {
            // Validate that all required fields are provided
            if (model.Date == default || model.Time == null || model.Cost <= 0)
            {
                TempData["ShowPopup"] = true; // Indicate that the popup should be shown
                TempData["PopupMessage"] = "Date, Time, and Cost are required fields.";
                TempData["PopupTitle"] = "Missing required fields!";  // Set the custom title
                TempData["PopupIcon"] = "error";  // Set the icon dynamically (can be success, error, info, warning)
                return RedirectToAction("SMaintenanceAssignment");
            }

            // Retrieve the request from the database using the RequestId
            var request = _context.Requests.FirstOrDefault(r => r.RequestID == model.RequestId);

            if (request == null)
            {
                TempData["Error"] = "The specified request does not exist.";
                return RedirectToAction("SMaintenanceAssignment");
            }

            // Update the CompletedDateTime with the combined Date and Time from the form
            request.CompletedDateTime = model.Date.Add(model.Time);

            // Update the RequestStatus to "Completed"
            request.RequestStatus = "Completed";

            // Update the Cost field of the request
            request.Cost = model.Cost;

            // Retrieve the associated staff using the StaffID from the request
            var staff = _context.Staffs.FirstOrDefault(s => s.StaffID == request.StaffID);

            if (staff != null)
            {
                // Set the IsVacant field of the staff to true
                staff.IsVacant = true;
            }

            // Save the changes to the database
            _context.SaveChanges();

            // Set TempData for success message
            TempData["ShowPopup"] = true; // Indicate that the popup should be shown
            TempData["PopupMessage"] = "Request has been successfully completed.";
            TempData["PopupTitle"] = "Request Completed!";  // Set the custom title
            TempData["PopupIcon"] = "success";  // Set the icon dynamically (can be success, error, info, warning)

            return RedirectToAction("SMaintenanceAssignment");
        }

        // Staff Profile

        //Profile
        public IActionResult SProfilePage()
        {
            // Retrieve the logged-in user's ID from the session
            //var userIdString = HttpContext.Session.GetString("UserId");
            var userId = HttpContext.Session.GetInt32("UserId"); // Retrieve as an integer
            if (!userId.HasValue)
            {
                // Redirect to login if UserId is not in the session
                TempData["ErrorMessage"] = "You need to log in to view your profile.";
                return RedirectToAction("Login", "Login");
            }

            // Log the UserId for debugging purposes
            _logger.LogInformation($"Logged-in UserId: {userId}");

            // Fetch the user profile from the database
            var profile = _context.UserProfiles.FirstOrDefault(p => p.UserID == userId);

            if (profile == null)
            {
                // Create a default profile for the user if none exists
                profile = new Profile
                {
                    UserID = (int)userId,
                    FirstName = "First name not provided",
                    LastName = "Last name not provided",
                    Email = "Email not provided",
                    PhoneNumber = "Phone number not provided",
                    Address = "Address not provided"
                };

                // Save the new profile to the database
                _context.UserProfiles.Add(profile);
                _context.SaveChanges();

                // Log the creation of a new profile
                _logger.LogInformation($"Created a default profile for UserId: {userId}");
            }

            // Pass the profile to the view
            return View(profile);
        }


        public IActionResult SEditProfile()
        {
            // Example: Set default values for TempData
            TempData["ErrorMessage"] = TempData["ErrorMessage"] ?? string.Empty;
            TempData["SuccessMessage"] = TempData["SuccessMessage"] ?? string.Empty;
            // Retrieve the logged-in user's ID from session (as string) and convert to int
            var userId = HttpContext.Session.GetInt32("UserId"); // Retrieve as an integer
            if (!userId.HasValue)
            {
                // Redirect to login if UserId is not in the session
                TempData["ErrorMessage"] = "You need to log in to view your profile.";
                return RedirectToAction("Login", "Login");
            }
            //var userIdString = HttpContext.Session.GetString("UserId");
            //if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            //{
            //    // If the session doesn't contain a valid UserId, redirect to login page
            //    return RedirectToAction("Login", "Login");
            //}
            var profile = _context.UserProfiles.FirstOrDefault(p => p.Id == userId);
            return View(profile);
        }

        [HttpPost]
        public IActionResult UpdateProfile(Profile model)
        {
            // Retrieve the logged-in user's ID from session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                TempData["ErrorMessage"] = "You need to log in to view your profile.";
                return RedirectToAction("Login", "Login");
            }

            // Retrieve the user profile from the database
            var profile = _context.UserProfiles.FirstOrDefault(p => p.Id == userId);
            var user = _context.Users.FirstOrDefault(u => u.UserID == userId);

            // Ensure the profile and user exist before updating
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
            else
            {
                // Handle case where profile or user doesn't exist
                TempData["ErrorMessage"] = "Profile not found.";
                return RedirectToAction("SProfilePage");
            }

            // Redirect to the profile page after update
            return RedirectToAction("SProfilePage");
        }



        // Display the EditPasswordPage
        public IActionResult ChangePassword()
        {
            return View();
        }

        // Change or Update the password
        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            // Retrieve the logged-in user's ID from session (as string) and convert to int
            //var userIdString = HttpContext.Session.GetString("UserId");
            //if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            //{
            //    return RedirectToAction("Login", "Login");
            //}
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
                return RedirectToAction("ChangePassword");
            }

            // Check if the current password matches the hashed password (using BCrypt)
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))  // Compare hashed password
            {
                TempData["ErrorMessage"] = "Current password is incorrect.";
                return RedirectToAction("ChangePassword");
            }

            // Check if the new password and confirm password match
            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "New password and confirm password do not match.";
                return RedirectToAction("ChangePassword");
            }

            // Validate that the new password is between 15 and 64 characters
            if (newPassword.Length < 15 || newPassword.Length > 64)
            {
                TempData["ErrorMessage"] = "Password must be between 15 and 64 characters.";
                return RedirectToAction("ChangePassword");
            }

            // Hash the new password before saving it to the database
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);  // Hash the new password

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToAction("SProfilePage");
        }


    }
}
