using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchuelerCheckIN2025.Data;
using SchuelerCheckIN2025.Models;
using System.Diagnostics;

namespace SchuelerCheckIN2025.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            


        }

        public async Task<IActionResult> Index()
        {
            List<Schuelerdaten> schueler = _context.Schuelerdatenset.ToList();
            bool UserExists = false;

            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (user != null)
                {
                    var existingSchueler = _context.Schuelerdatenset.FirstOrDefault(s => s.email == user.Email);

                    if (existingSchueler == null) // Nur hinzufügen, wenn kein Eintrag existiert
                    {
                        _context.Schuelerdatenset.Add(new Schuelerdaten
                        {
                            email = user.Email,
                            schluessel = Guid.NewGuid().ToString(),
                            klasse = "3AHINF"
                        });

                        _context.SaveChanges();
                    }
                }
            }

            return View(schueler);
        }

        public IActionResult Privacy()
        {

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
