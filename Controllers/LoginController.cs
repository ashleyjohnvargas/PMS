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
                return View("Login"); // Render the Login view with errors // Redirect back to the Login page
            }

            // Find the user with the given email
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return RedirectToAction("Login"); // Redirect back to the Login page
            }

            // Verify the password using BCrypt
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return RedirectToAction("Login"); // Redirect back to the Login page
            }

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))  // Compare the entered password with the hashed password
            {

                // Store user information in session
                HttpContext.Session.SetString("UserId", user.UserID.ToString());
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("FirstName", user.FirstName);
                HttpContext.Session.SetString("LastName", user.LastName);
                HttpContext.Session.SetString("UserRole", user.Role);

                // Find the tenant through the UserID (if applicable)
                var tenant = _context.Tenants.FirstOrDefault(t => t.UserId == user.UserID);

                // Redirect to the appropriate dashboard based on the user's role
                switch (user.Role)
                {
                    case "Property Manager":
                        return RedirectToAction("PMDashboard", "PropertyManager");

                    case "Staff":
                        return RedirectToAction("SHomePage", "Staff");

                    case "Tenant":
                        if (tenant != null && tenant.IsActualTenant) // Check if tenant exists and is an actual tenant
                        {
                            return RedirectToAction("ATenantHome", "ATenant");
                        }
                        return RedirectToAction("PTenantHomePage", "PTenant");

                    default:
                        // Handle unexpected roles
                        ModelState.AddModelError("", "Invalid role. Please contact the administrator.");
                        return RedirectToAction("Login");
                }
            }
            // Fallback for any other cases (should not normally be reached)
            ModelState.AddModelError("", "An unknown error occurred. Please try again.");
            return View("Login"); // Render the Login view with errors
        }




        //public IActionResult ForgotPass()
        //{
        //    return View();
        //}

        public IActionResult Register()
        {
            return View();
        }

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

            // Validate password length
            if (user.Password.Length < 15 || user.Password.Length > 64)
            {
                ModelState.AddModelError("", "Password must be between 15 and 64 characters long.");
                return View("Register", user); // Return to the Register page
            }

            // Check if passwords match
            if (user.Password != user.ConfirmPassword)
            {
                ModelState.AddModelError("", "Password and Confirm Password do not match.");
                return View("Register", user); // Return to the Register page
            }

            // Check if Terms and Conditions are accepted
            if (user.TermsAndConditions.HasValue && !user.TermsAndConditions.Value)
            {
                ModelState.AddModelError("", "You must agree to the terms and conditions.");
                return View("Register", user); // Return to the Register page
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

            // Hash the password using bcrypt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

            // Create a new User object and populate it with data
            var newUser = new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = hashedPassword,
                Role = user.Role,
                TermsAndConditions = user.TermsAndConditions,
                DateCreated = DateTime.Now,
            };

            // Save the new user to the database
            _context.Users.Add(newUser);
            _context.SaveChanges();

            var newProfile = new Profile
            {
                UserID = newUser.UserID, // Link to the User
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                PhoneNumber = user.PhoneNumber,
                
            };

            _context.UserProfiles.Add(newProfile);
            _context.SaveChanges();

            // Store user information in the session
            HttpContext.Session.SetString("UserId", newUser.UserID.ToString());
            HttpContext.Session.SetString("FirstName", newUser.FirstName);
            HttpContext.Session.SetString("LastName", newUser.LastName);
            HttpContext.Session.SetString("UserEmail", newUser.Email);
            HttpContext.Session.SetString("UserRole", newUser.Role);

            // Redirect based on the user's role
            if (newUser.Role == "Property Manager")
            {
                var propertyManager = new PropertyManager
                {
                    UserId = newUser.UserID,
                };
                _context.PropertyManagers.Add(propertyManager);
                _context.SaveChanges();

                return RedirectToAction("PMDashboard", "PropertyManager");
            }
            else if (newUser.Role == "Staff")
            {
                var staff = new Staff
                {
                    UserId = newUser.UserID,
                };
                _context.Staffs.Add(staff);
                _context.SaveChanges();

                return RedirectToAction("SHomePage", "Staff");
            }
            else if (newUser.Role == "Tenant")
            {
                var tenant = new Tenant
                {
                    UserId = newUser.UserID,
                };
                _context.Tenants.Add(tenant);
                _context.SaveChanges();

                return RedirectToAction("PTenantHomePage", "PTenant");
            }

            // Fallback if none of the conditions match
            return RedirectToAction("Register");
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("PTenantHomePage", "PTenant");
            //return RedirectToAction("Login");
        }
    }
}
