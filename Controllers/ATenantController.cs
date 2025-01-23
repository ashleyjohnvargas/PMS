﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Models;

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
        public IActionResult ATenantLease()
        {
            return View();
        }
        public IActionResult ATenantPayment()
        {
            return View();
        }
        public IActionResult ATenantMaintenance()
        {
            return View();
        }
        public IActionResult ATenantEditProfile()
        {
            return View();
        }
        public IActionResult ATenantProfilePage()
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
                return View("ATenantEditProfile2", model);
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

        // Change or Update the password
        [HttpPost]
        public IActionResult ATenantChangePassword(string currentPassword, string newPassword, string confirmPassword)
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
