using Microsoft.AspNetCore.Mvc;

namespace PMS.Controllers
{
    public class StaffController : Controller
    {
        public IActionResult SHomePage()
        {
            return View();
        }
        public IActionResult SMaintenanceAssignment()
        {
            return View();
        }
        public IActionResult SMaintenanceHistory()
        {
            return View();
        }

        public IActionResult SEditProfile()
        {
            return View();
        }

    }
}
