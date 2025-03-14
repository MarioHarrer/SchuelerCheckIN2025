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

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
     

            List<Schuelerdaten> schueler = _context.Schuelerdatenset.ToList();

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
