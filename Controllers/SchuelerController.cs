using Microsoft.AspNetCore.Mvc;

namespace SchuelerCheckIN2025.Controllers
{
    public class SchuelerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
