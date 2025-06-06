using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SchuelerCheckIN2025.Data;
using SchuelerCheckIN2025.Models;
using System.Diagnostics;
using ZXing;
using ZXing.Common;
using ZXing.SkiaSharp;
using SkiaSharp;

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
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await GetCurrentUserAsync();
                if (user != null)
                {
                    string uuid = GetOrCreateUuid(user);
                    string qrCodeBase64 = GenerateQrCodeBase64(uuid);
                    ViewData["QRCode"] = "data:image/png;base64," + qrCodeBase64;

                    var eintrag = _context.Schuelerdatenset.FirstOrDefault(u => u.email == user.Email);
                    ViewBag.isAdmin = eintrag?.admin ?? false;
                }
            }

            return View();
        }

        private async Task<IdentityUser?> GetCurrentUserAsync()
        {
            var userName = User.Identity?.Name;
            return await _userManager.FindByNameAsync(userName);
        }

        private string GetOrCreateUuid(IdentityUser user)
        {
            var letzterEintrag = _context.Schuelerdatenset
                .Where(s => s.email == user.Email)
                .OrderByDescending(s => s.Id)
                .FirstOrDefault();

            if (letzterEintrag == null)
            {
                var daten = CreateDatenFromUser(user, _context, "UNBEKANNT");
                return daten.schluessel;
            }

            return letzterEintrag.schluessel;
        }

        public static Schuelerdaten CreateDatenFromUser(IdentityUser user, ApplicationDbContext context, string klasse)
        {
            string uuid = Guid.NewGuid().ToString();
            var schuelerdaten = new Schuelerdaten
            {
                email = user.Email,
                schluessel = uuid,
                klasse = klasse,
                anwesend = false,
                admin = false,
                zeit = new TimeOnly(7, 40)
            };

            context.Schuelerdatenset.Add(schuelerdaten);
            context.SaveChanges();

            return schuelerdaten;
        }

        private string GenerateQrCodeBase64(string uuid)
        {
            int width = 300;
            int height = 300;

            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = 1
                }
            };

            var bitmap = writer.Write(uuid);
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return Convert.ToBase64String(data.ToArray());
        }

        public IActionResult Anwesenheit(AnwesenheitsViewModel anwesenheitsview)
        {
            var model = new AnwesenheitsViewModel
            {
                SelectedClass = anwesenheitsview.SelectedClass ?? " ",
                ClassList = new List<SelectListItem>
                {
                    new SelectListItem { Value = "3AHINF", Text = "3AHINF" },
                    new SelectListItem { Value = "2AHINF", Text = "2AHINF" },
                    new SelectListItem { Value = "1AHINF", Text = "1AHINF" },
                },
                Students = _context.Schuelerdatenset
                    .Where(s => !s.anwesend && !s.admin)
                    .ToList()
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<IdentityUser?> CheckIfUuidExistsAsync(string scannedUuid)
        {
            var schuelerdaten = _context.Schuelerdatenset
                .FirstOrDefault(s => s.schluessel == scannedUuid);

            if (schuelerdaten != null)
            {
                return await _userManager.FindByEmailAsync(schuelerdaten.email);
            }

            return null;
        }
    }
}
