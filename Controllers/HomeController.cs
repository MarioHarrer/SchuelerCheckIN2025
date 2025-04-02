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
                    int width = 300;
                    int height = 300;
                    var writer = new BarcodeWriter<Bitmap>()
                    {
                        Format = BarcodeFormat.QR_CODE,
                        Renderer = new ZXing.Windows.Compatibility.BitmapRenderer(),
                        Options = new EncodingOptions
                        {
                            Height = height,
                            Width = width,
                            Margin = 1
                        }
                    };

                    using (var qrCodeImage = writer.Write(uuid))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            byte[] byteImage = ms.ToArray();
                            string base64String = Convert.ToBase64String(byteImage);

                            // Base64-String in ViewData speichern
                            ViewData["QRCode"] = "data:image/png;base64," + base64String;
                        }
                    }
                }
            }

            return View();

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
