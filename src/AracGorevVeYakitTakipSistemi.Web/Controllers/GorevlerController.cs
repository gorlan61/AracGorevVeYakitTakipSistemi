using AracGorevVeYakitTakipSistemi.Application.DTOs;
using AracGorevVeYakitTakipSistemi.Application.Interfaces;
using AracGorevVeYakitTakipSistemi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AracGorevVeYakitTakipSistemi.Web.Controllers;

[Authorize]
public class GorevlerController(
    IGorevService gorevService,
    IAracService aracService,
    ISurucuService surucuService) : Controller
{
    // GET: /Gorevler
    public async Task<IActionResult> Index()
    {
        var liste = await gorevService.TumunuGetirAsync();
        return View(liste);
    }

    // GET: /Gorevler/Detay/5
    public async Task<IActionResult> Detay(int id)
    {
        var dto = await gorevService.GetirAsync(id);
        if (dto is null)
        {
            TempData["Hata"] = "Görev bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
        return View(dto);
    }

    // GET: /Gorevler/Yeni
    public async Task<IActionResult> Yeni()
    {
        await DropdownlariDoldurAsync();
        return View(new GorevDto { GorevTarihi = DateTime.Today });
    }

    // POST: /Gorevler/Yeni
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Yeni(GorevDto dto)
    {
        if (!ModelState.IsValid)
        {
            await DropdownlariDoldurAsync();
            return View(dto);
        }

        try
        {
            await gorevService.EkleAsync(dto);
            TempData["Basari"] = "Görev başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await DropdownlariDoldurAsync();
            return View(dto);
        }
    }

    // GET: /Gorevler/KmGirisi/5
    public async Task<IActionResult> KmGirisi(int id)
    {
        var dto = await gorevService.GetirAsync(id);
        if (dto is null)
        {
            TempData["Hata"] = "Görev bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
        if (dto.Durum == GorevDurumu.Tamamlandi || dto.Durum == GorevDurumu.Iptal)
        {
            TempData["Hata"] = "Bu görev için kilometre girişi yapılamaz.";
            return RedirectToAction(nameof(Detay), new { id });
        }
        return View(dto);
    }

    // POST: /Gorevler/KmGirisi/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KmGirisi(int id, int baslangicKm, int bitisKm)
    {
        if (bitisKm < baslangicKm)
        {
            TempData["Hata"] = "Bitiş km başlangıç km'den küçük olamaz.";
            var dto = await gorevService.GetirAsync(id);
            return View(dto);
        }

        try
        {
            var (anomaliVar, toplamMesafe) = await gorevService.KmGuncelleAsync(id, baslangicKm, bitisKm);

            if (anomaliVar)
                TempData["Uyari"] = $"Görev kaydedildi ancak kat edilen mesafe ({toplamMesafe:N0} km) olağan dışı yüksek. Lütfen kontrol edin.";
            else
                TempData["Basari"] = $"Kilometre girişi kaydedildi. Toplam mesafe: {toplamMesafe:N0} km.";

            return RedirectToAction(nameof(Detay), new { id });
        }
        catch (InvalidOperationException ex)
        {
            TempData["Hata"] = ex.Message;
            return RedirectToAction(nameof(KmGirisi), new { id });
        }
    }

    // POST: /Gorevler/DurumGuncelle
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DurumGuncelle(int id, GorevDurumu durum)
    {
        try
        {
            await gorevService.DurumGuncelleAsync(id, durum);
            TempData["Basari"] = "Görev durumu güncellendi.";
        }
        catch (KeyNotFoundException)
        {
            TempData["Hata"] = "Görev bulunamadı.";
        }
        return RedirectToAction(nameof(Detay), new { id });
    }

    // GET: /Gorevler/Sil/5
    public async Task<IActionResult> Sil(int id)
    {
        var dto = await gorevService.GetirAsync(id);
        if (dto is null)
        {
            TempData["Hata"] = "Görev bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
        return View(dto);
    }

    // POST: /Gorevler/Sil/5
    [HttpPost, ActionName("Sil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SilOnayla(int id)
    {
        try
        {
            await gorevService.SilAsync(id);
            TempData["Basari"] = "Görev silindi.";
        }
        catch (Exception ex)
        {
            TempData["Hata"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    // ─── Yardımcı ─────────────────────────────────────────────────────────────
    private async Task DropdownlariDoldurAsync()
    {
        var araclar = await aracService.TumunuGetirAsync();
        var suruculer = await surucuService.TumunuGetirAsync();

        ViewBag.AracListesi = araclar
            .Where(a => a.Durum == Domain.Enums.AracDurumu.Aktif)
            .Select(a => new SelectListItem($"{a.Plaka} – {a.Marka} {a.Model}", a.AracId.ToString()))
            .ToList();

        ViewBag.SurucuListesi = suruculer
            .Where(s => s.AktifMi)
            .Select(s => new SelectListItem(s.AdSoyad, s.SurucuId.ToString()))
            .ToList();
    }
}
