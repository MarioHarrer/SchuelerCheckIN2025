using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchuelerCheckIN2025.Data;
using SchuelerCheckIN2025.Models;
using System.Diagnostics;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Drawing;
using ZXing.Common;
using ZXing;
using ZXing.Rendering;
using static System.Net.Mime.MediaTypeNames;
using System.Text;

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
            bool IsAuth = (User.Identity is not null) ? User.Identity.IsAuthenticated : false;

            if (IsAuth)
            {
                var userName = User.Identity?.Name;
                var user = await _userManager.FindByNameAsync(userName);

                if (user != null)
                {
                    // Prüfen, ob bereits ein QR-Code existiert
                    var letzterEintrag = _context.Schuelerdatenset
                        .Where(s => s.email == user.Email)
                        .OrderByDescending(s => s.Id) // Annahme: Id ist aufsteigend
                        .FirstOrDefault();

                    string uuid;

                    if (letzterEintrag == null)
                    {
                        // Neuen Eintrag erstellen
                        uuid = Guid.NewGuid().ToString();

                        var schuelerdaten = new Schuelerdaten
                        {
                            email = user.Email,
                            schluessel = uuid,
                            klasse = "3AHINF"
                        };

                        _context.Schuelerdatenset.Add(schuelerdaten);
                        _context.SaveChanges();
                    }
                    else
                    {
                        // Falls es bereits einen QR-Code gibt, nutze ihn
                        uuid = letzterEintrag.schluessel;
                    }

                    // QR-Code generieren
                    int width = 100;
                    int height = 100;
                    var writer = new BarcodeWriterSvg
                    {
                        Format = BarcodeFormat.QR_CODE,
                        Options = new EncodingOptions
                        {
                            Height = height,
                            Width = width,
                            Margin = 1
                        }
                    };

                    
                    var svg = writer.Write(uuid); // SVG-Code für den QR-Code generieren
                    string svgBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(svg.Content));
                    ViewData["QRCode"] = "data:image/svg+xml;base64," + svgBase64;
                }
            }

            return View();

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
