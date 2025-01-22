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

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
           
            //if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            //{
            //    ModelState.AddModelError("", "Email and Password are required.");
            //    return RedirectToAction("Login"); // Redirect back to the Login page
            //}

            // Find the user with the given email
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return RedirectToAction("Login"); // Redirect back to the Login page
            }

            //// Check if the password matches
            //if (user.Password != password)
            //{
            //    ModelState.AddModelError("", "Invalid email or password.");
            //    return RedirectToAction("Login"); // Redirect back to the Login page
            //}

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))  // Compare the entered password with the hashed password
            {
                //HttpContext.Session.SetString("UserId", user.UserID.ToString());
                //HttpContext.Session.SetString("UserFirstName", user.FirstName);
                //HttpContext.Session.SetString("UserLastName", user.LastName);
                //HttpContext.Session.SetString("UserEmail", user.Email);

                // If the credentials are correct, store user information in session (or claims)
                HttpContext.Session.SetString("UserId", user.UserID.ToString());

                //HttpContext.Session.SetInt32("UserId", user.UserID);
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
               
            }
            ViewBag.ErrorMessage = "Invalid email or password";
            // Fallback redirect
            return RedirectToAction("Login");
        }

            
          

        //[HttpPost]
        //public IActionResult LoginUser(string email, string password)
        //{
        //    //var user = _context.Users.FirstOrDefault(u => u.Email == email);
        //    //if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))  // Compare the entered password with the hashed password

        //    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        //    {
        //        ModelState.AddModelError("", "Email and Password are required.");
        //        return RedirectToAction("Login"); // Redirect back to the Login page
        //    }

        //    // Find the user with the given email
        //    var user = _context.Users.FirstOrDefault(u => u.Email == email);

        //    if (user == null)
        //    {
        //        ModelState.AddModelError("", "User not found.");
        //        return RedirectToAction("Login"); // Redirect back to the Login page
        //    }

        //    // Check if the password matches
        //    if (user.Password != password)
        //    {
        //        ModelState.AddModelError("", "Invalid email or password.");
        //        return RedirectToAction("Login"); // Redirect back to the Login page
        //    }

        //    // If the credentials are correct, store user information in session (or claims)
        //    HttpContext.Session.SetInt32("UserId", user.UserID);
        //    HttpContext.Session.SetString("UserEmail", user.Email);
        //    HttpContext.Session.SetString("FirstName", user.FirstName);
        //    HttpContext.Session.SetString("LastName", user.LastName);
        //    HttpContext.Session.SetString("UserRole", user.Role);

        //    // Redirect to the appropriate dashboard based on the user's role
        //    if (user.Role == "Property Manager")
        //    {
        //        return RedirectToAction("PMDashboard", "PropertyManager");
        //    }
        //    else if (user.Role == "Staff")
        //    {
        //        return RedirectToAction("SHomePage", "Staff");
        //    }
        //    else if (user.Role == "Tenant")
        //    {
        //        return RedirectToAction("ATenantHome", "ATenant");
        //    }

        //    // Fallback redirect
        //    return RedirectToAction("Login");
        //}


        public IActionResult ForgotPass()
        {
            return View();
        }

        public IActionResult Register()
        {
            var user = new User();
            //var profile = new Profile();
            //profile.Id = user.UserID;
            //user.DateCreated = DateTime.Now;

            //// You may save the user and profile to the database
            //_context.Users.Add(user);
            //_context.UserProfiles.Add(profile);
            //_context.SaveChanges();

            return View(user);
            //var user = new User
            //{
            //    DateCreated = DateTime.Now  // Set the creation date to the current date and time
            //};

            //var profile = new Profile();  // Create a new profile (you can add default values if needed)

            //return View(new RegisterViewModel
            //{
            //    User = user,
            //    Profile = profile
            //});
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
                Password = hashedPassword, // Store hashed password
                Role = user.Role,
                TermsAndConditions = user.TermsAndConditions,
                DateCreated = DateTime.Now,
            };
            _context.Users.Add(newUser);

            // Before adding the profile, check if one already exists for the user
            var existingProfile = _context.UserProfiles.FirstOrDefault(p => p.Id == newUser.UserID);
            if (existingProfile == null)
            {
                var newProfile = new Profile
                {
                    Id = newUser.UserID,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                };
                _context.UserProfiles.Add(newProfile);
                _context.SaveChanges();  // Save the profile only if it's a new user
            }

            // Store user information in the session
            HttpContext.Session.SetString("UserId", user.UserID.ToString());

           // HttpContext.Session.SetInt32("UserId", newUser.UserID);
            HttpContext.Session.SetString("FirstName", newUser.FirstName);
            HttpContext.Session.SetString("LastName", newUser.LastName);
            HttpContext.Session.SetString("UserEmail", newUser.Email);
            HttpContext.Session.SetString("UserRole", newUser.Role);

            // Redirect based on the user's role
            if (newUser.Role == "Property Manager")
            {
                return RedirectToAction("PMDashboard", "PropertyManager");
            }
            else if (newUser.Role == "Staff")
            {
                return RedirectToAction("SHomePage", "Staff");
            }
            else if (newUser.Role == "Tenant")
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
