using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Models;

namespace PMS.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginUser(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email and Password are required.");
                return RedirectToAction("Login"); // Redirect back to the Login page
            }

            // Find the user with the given email
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return RedirectToAction("Login"); // Redirect back to the Login page
            }

            // Check if the password matches
            if (user.Password != password)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return RedirectToAction("Login"); // Redirect back to the Login page
            }

            // If the credentials are correct, store user information in session (or claims)
            HttpContext.Session.SetInt32("UserId", user.UserID);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("FirstName", user.FirstName);
            HttpContext.Session.SetString("LastName", user.LastName);
            HttpContext.Session.SetString("UserRole", user.Role);

            // Redirect to the appropriate dashboard based on the user's role
            if (user.Role == "Property Manager")
            {
                return RedirectToAction("PMDashboard", "PropertyManager");
            }
            else if (user.Role == "Staff")
            {
                return RedirectToAction("SHomePage", "Staff");
            }
            else if (user.Role == "Tenant")
            {
                return RedirectToAction("ATenantHome", "ATenant");
            }

            // Fallback redirect
            return RedirectToAction("Login");
        }


        public IActionResult ForgotPass()
        {
            return View();
        }

        public IActionResult Register()
        {
            var user = new User();
            return View(user);
        }

        // RegisterUser action to handle form submission and save user data
       // [HttpPost]
        // RegisterUser action to handle form submission and save user data
        [HttpPost]
        public IActionResult RegisterUser(User user)
        {
            // Check if email already exists
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "An account with this email already exists.");
                return View("Register", user); // Return to the Register page
            }

            // Check if passwords match
            if (user.Password != user.ConfirmPassword)
            {
                ModelState.AddModelError("", "Password and Confirm Password do not match.");
                return RedirectToAction("Register"); // Redirect to the Register action
            }

            // Check if Terms and Conditions are accepted
            if (user.TermsAndConditions.HasValue && !user.TermsAndConditions.Value)
            {
                ModelState.AddModelError("", "You must agree to the terms and conditions.");
                return RedirectToAction("Register"); // Redirect to the Register action
            }

            // Set the Role based on the email domain
            if (user.Email.Contains("@manager"))
            {
                user.Role = "Property Manager";
            }
            else if (user.Email.Contains("@staff"))
            {
                user.Role = "Staff";
            }
            else
            {
                user.Role = "Tenant"; // Default role if the email doesn't match above criteria
            }


            // Store user information in the session
            HttpContext.Session.SetInt32("UserId", user.UserID);
            HttpContext.Session.SetString("FirstName", user.FirstName);
            HttpContext.Session.SetString("LastName", user.LastName);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role);


            // Create a new User object and populate it with data
            var newUser = new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                Role = user.Role, // Default or set role based on your requirement
                TermsAndConditions = user.TermsAndConditions,
            };

            // Save the new user to the database
            _context.Users.Add(newUser);
            _context.SaveChanges();

            // Redirect based on the user's role
            if (user.Role == "Property Manager")
            {
                return RedirectToAction("PMDashboard", "PropertyManager");
            }
            else if (user.Role == "Staff")
            {
                return RedirectToAction("SHomePage", "Staff");
            }
            else if (user.Role == "Tenant")
            {
                return RedirectToAction("ATenantHome", "ATenant");
            }

            // If none of the conditions match, return to the Register action (or another appropriate action)
            return RedirectToAction("Register"); // This is the fallback action, should never be reached

        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("PTenantHomePage", "PTenant");
        }
    }
}
