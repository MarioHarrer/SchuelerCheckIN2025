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
using Microsoft.AspNetCore.Mvc.Rendering;

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

        //public async Task<IActionResult> Index()
        //{
        //    bool IsAuth = (User.Identity is not null) ? User.Identity.IsAuthenticated : false;

        //    if (IsAuth)
        //    {
        //        var userName = User.Identity?.Name;
        //        var user = await _userManager.FindByNameAsync(userName);

        //        if (user != null)
        //        {
        //            // Prüfen, ob bereits ein QR-Code existiert
        //            var letzterEintrag = _context.Schuelerdatenset
        //                .Where(s => s.email == user.Email)
        //                .OrderByDescending(s => s.Id) // Annahme: Id ist aufsteigend
        //                .FirstOrDefault();

        //            string uuid;

        //            if (letzterEintrag == null)
        //            {
        //                // Neuen Eintrag erstellen
        //                uuid = Guid.NewGuid().ToString();

        //                var schuelerdaten = new Schuelerdaten
        //                {
        //                    email = user.Email,
        //                    schluessel = uuid,
        //                    klasse = "3AHINF"
        //                };

        //                _context.Schuelerdatenset.Add(schuelerdaten);
        //                _context.SaveChanges();
        //            }
        //            else
        //            {
        //                // Falls es bereits einen QR-Code gibt, nutze ihn
        //                uuid = letzterEintrag.schluessel;
        //            }

        //            // QR-Code generieren
        //            int width = 300;
        //            int height = 300;
        //            var writer = new BarcodeWriter<Bitmap>()
        //            {
        //                Format = BarcodeFormat.QR_CODE,
        //                Renderer = new ZXing.Windows.Compatibility.BitmapRenderer(),
        //                Options = new EncodingOptions
        //                {
        //                    Height = height,
        //                    Width = width,
        //                    Margin = 1
        //                }
        //            };

        //            using (var qrCodeImage = writer.Write(uuid))
        //            {
        //                using (MemoryStream ms = new MemoryStream())
        //                {
        //                    qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        //                    byte[] byteImage = ms.ToArray();
        //                    string base64String = Convert.ToBase64String(byteImage);

        //                    // Base64-String in ViewData speichern
        //                    ViewData["QRCode"] = "data:image/png;base64," + base64String;
        //                }
        //            }
        //        }
        //    }

        //    return View()

        //

        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await GetCurrentUserAsync();
                if (user != null)
                {
                    string uuid = GetOrCreateUuidd(user);

                    List<Schuelerdaten> datalist = _context.Schuelerdatenset.ToList();
                    Schuelerdaten schuelerdaten1 = new Schuelerdaten();

                    foreach(Schuelerdaten s in datalist)
                    {
                        if(s.schluessel == uuid)
                        {
                            schuelerdaten1 = s;
                        }
                    }
                    if(schuelerdaten1.admin == false)
                    {
                        GenerateQrCodeBase64(uuid);
                        
                    }
                    ViewBag.isAdmin = _context.Schuelerdatenset.Where(u => u.email == user.Email).First().admin;
                }
            }


            return View();
        }

        private async Task<IdentityUser?> GetCurrentUserAsync()
        {
            var userName = User.Identity?.Name;
            return await _userManager.FindByNameAsync(userName);
        }

        private string GetOrCreateUuidd(IdentityUser user)
        {
            var letzterEintrag = _context.Schuelerdatenset
                .Where(s => s.email == user.Email)
                .OrderByDescending(s => s.Id)
                .FirstOrDefault();

            if (letzterEintrag == null)
            {
                Schuelerdaten daten = createDatenFromUser(user, _context, "UNBEKANNT");

                return daten.schluessel;
            }

            return letzterEintrag.schluessel;
        }

        public static Schuelerdaten createDatenFromUser(IdentityUser user, ApplicationDbContext context, string klasse)
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

            {

            }

            };
            if(klasse == "Teacher")
            {
                schuelerdaten.admin = true;
            }

            context.Schuelerdatenset.Add(schuelerdaten);
            context.SaveChanges();

            return schuelerdaten;
        }

        private void GenerateQrCodeBase64(string uuid)
        {
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

            var svg = writer.Write(uuid);
            string svgBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(svg.Content));
            ViewData["QRCode"] = "data:image/svg+xml;base64," + svgBase64;
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



        public IActionResult Anwesenheit(AnwesenheitsViewModel anwesenheitsview)
        {
            var model = new AnwesenheitsViewModel
            {
                SelectedClass = anwesenheitsview.SelectedClass == null ? " " : anwesenheitsview.SelectedClass,
                ClassList = new List<SelectListItem>
        {
            new SelectListItem { Value = "5AHINF", Text = "5AHINF" },
            new SelectListItem { Value = "4AHINF", Text = "4AHINF" },
            new SelectListItem { Value = "3AHINF", Text = "3AHINF" },
            new SelectListItem { Value = "2AHINF", Text = "2AHINF" },
            new SelectListItem { Value = "1AHINF", Text = "1AHINF" },
        },
                Students = _context.Schuelerdatenset.Where(s => !s.anwesend && !s.admin).ToList() // erstmal leer
            };

            return View(model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
