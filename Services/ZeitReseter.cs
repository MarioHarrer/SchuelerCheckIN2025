using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using SchuelerCheckIN2025.Data; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchuelerCheckIN2025.Data;

public class ZeitResetWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<ZeitResetWorker> _logger;

    public ZeitResetWorker(IServiceProvider services, ILogger<ZeitResetWorker> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ZeitResetWorker gestartet.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextRun = DateTime.Today.AddDays(now >= DateTime.Today.AddHours(5) ? 1 : 0).AddHours(5);



            var delay = nextRun - now;
            _logger.LogInformation("Warte bis zum nächsten Lauf um {NextRun}", nextRun);

            await Task.Delay(delay, stoppingToken);

            // Prüfen, ob heute ein Wochentag ist (Montag bis Freitag)
            var heute = DateTime.Today.DayOfWeek;
            if (heute >= DayOfWeek.Monday && heute <= DayOfWeek.Friday)
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        var schuelerListe = await context.Schuelerdatenset.ToListAsync(stoppingToken);
                        foreach (var schueler in schuelerListe)
                        {
                            schueler.zeit = new TimeOnly(7, 40); 
                            schueler.anwesend = false; 
                        }

                        await context.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("Zeit für alle Schüler auf 07:40 gesetzt ({Date})", DateTime.Now);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fehler beim Zurücksetzen der Zeit.");
                }
            }
            else
            {
                _logger.LogInformation("Heute ist Wochenende ({Day}), kein Reset durchgeführt.", heute);
            }
        }
    }
}
