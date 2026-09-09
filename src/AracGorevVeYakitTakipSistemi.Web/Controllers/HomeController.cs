using System.Diagnostics;
using AracGorevVeYakitTakipSistemi.Application.Interfaces;
using AracGorevVeYakitTakipSistemi.Domain.Enums;
using AracGorevVeYakitTakipSistemi.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace AracGorevVeYakitTakipSistemi.Web.Controllers;

public class HomeController(
    IAracService aracService,
    ISurucuService surucuService,
    IGorevService gorevService,
    IBakimService bakimService,
    IYakitService yakitService) : Controller
{
    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return View(new DashboardViewModel());
        }

        var araclar = await aracService.TumunuGetirAsync();
        var suruculer = await surucuService.TumunuGetirAsync();
        var gorevler = await gorevService.TumunuGetirAsync();
        var yakitlar = await yakitService.TumunuGetirAsync();
        var yaklasanBakimlar = await bakimService.YaklasanBakimlariGetirAsync();

        var buAy = DateTime.Today.Month;
        var buYil = DateTime.Today.Year;

        var vm = new DashboardViewModel
        {
            ToplamArac = araclar.Count,
            AktifArac = araclar.Count(a => a.Durum == AracDurumu.Aktif),
            ToplamSurucu = suruculer.Count,
            AktifSurucu = suruculer.Count(s => s.AktifMi),
            PlanlananGorev = gorevler.Count(g => g.Durum == GorevDurumu.Planlandi),
            DevamEdenGorev = gorevler.Count(g => g.Durum == GorevDurumu.DevamEdiyor),
            BuAyYakitTutar = yakitlar.Where(y => y.Tarih.Month == buAy && y.Tarih.Year == buYil).Sum(y => y.ToplamTutar),
            SonGorevler = gorevler.Take(5).ToList(),
            YaklasanBakimlar = yaklasanBakimlar
        };

        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
