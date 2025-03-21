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

            bool IsAuth = (User.Identity is not null) ? User.Identity.IsAuthenticated : false;

            if (IsAuth)
            {
                var userName = User.Identity?.Name;  // Name des aktuell angemeldeten Benutzers
                Console.WriteLine($"Aktuell angemeldeter Benutzer: {userName}");

                // Hole den Benutzer anhand des Benutzernamens
                var user = await _userManager.FindByNameAsync(userName);

                if (user != null)
                {
                    // Wenn der Benutzer gefunden wurde, kannst du z.B. weitere Daten abfragen und auf der Konsole ausgeben
                    Console.WriteLine($"Benutzer ID: {user.Id}");
                    Console.WriteLine($"Benutzer E-Mail: {user.Email}");
                }
                else
                {
                    Console.WriteLine("Benutzer nicht gefunden!");
                }
            }

            return View(schueler);
        }


        public IActionResult Tues()
        {
            Schuelerdaten schuelerdaten = new Schuelerdaten();
            schuelerdaten.email = "dhfsfs";
            schuelerdaten.schluessel = "df";
            schuelerdaten.klasse = "3ahinf";

            _context.Schuelerdatenset.Add(schuelerdaten);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
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
