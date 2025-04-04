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

                    ViewData["QRCode"] = "data:image/png;base64,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";

                    /*using (var qrCodeImage = writer.Write(uuid))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            byte[] byteImage = ms.ToArray();
                            string base64String = Convert.ToBase64String(byteImage);

                            // Base64-String in ViewData speichern
                            ViewData["QRCode"] = "data:image/png;base64," + base64String;
                        }
                    }*/
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
