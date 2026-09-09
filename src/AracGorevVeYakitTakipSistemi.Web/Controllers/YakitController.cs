using AracGorevVeYakitTakipSistemi.Application.DTOs;
using AracGorevVeYakitTakipSistemi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AracGorevVeYakitTakipSistemi.Web.Controllers;

[Authorize]
public class YakitController(
    IYakitService yakitService,
    IAracService aracService) : Controller
{
    // GET: /Yakit
    public async Task<IActionResult> Index(int? aracId)
    {
        IReadOnlyList<YakitKaydiDto> liste;
        if (aracId.HasValue)
        {
            liste = await yakitService.AracGoreGetirAsync(aracId.Value);
            ViewBag.SeciliAracId = aracId.Value;
        }
        else
        {
            liste = await yakitService.TumunuGetirAsync();
        }

        var araclar = await aracService.TumunuGetirAsync();
        ViewBag.AracListesi = araclar
            .Select(a => new SelectListItem($"{a.Plaka} – {a.Marka} {a.Model}", a.AracId.ToString()))
            .ToList();

        ViewBag.Analizler = await yakitService.TumAraclarTuketimAnaliziGetirAsync();

        return View(liste);
    }

    // GET: /Yakit/Detay/5
    public async Task<IActionResult> Detay(int id)
    {
        var dto = await yakitService.GetirAsync(id);
        if (dto is null)
        {
            TempData["Hata"] = "Yakıt kaydı bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
        return View(dto);
    }

    // GET: /Yakit/Yeni
    public async Task<IActionResult> Yeni(int? aracId)
    {
        await DropdownDoldurAsync();
        return View("YeniDuzenle", new YakitKaydiDto
        {
            AracId = aracId ?? 0,
            Tarih = DateTime.Today
        });
    }

    // POST: /Yakit/Yeni
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Yeni(YakitKaydiDto dto)
    {
        if (!ModelState.IsValid)
        {
            await DropdownDoldurAsync();
            return View("YeniDuzenle", dto);
        }

        try
        {
            await yakitService.EkleAsync(dto);
            TempData["Basari"] = "Yakıt kaydı başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await DropdownDoldurAsync();
            return View("YeniDuzenle", dto);
        }
    }

    // GET: /Yakit/Duzenle/5
    public async Task<IActionResult> Duzenle(int id)
    {
        var dto = await yakitService.GetirAsync(id);
        if (dto is null)
        {
            TempData["Hata"] = "Yakıt kaydı bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
        await DropdownDoldurAsync();
        return View("YeniDuzenle", dto);
    }

    // POST: /Yakit/Duzenle/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(int id, YakitKaydiDto dto)
    {
        if (id != dto.YakitId)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            await DropdownDoldurAsync();
            return View("YeniDuzenle", dto);
        }

        try
        {
            await yakitService.GuncelleAsync(dto);
            TempData["Basari"] = "Yakıt kaydı güncellendi.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await DropdownDoldurAsync();
            return View("YeniDuzenle", dto);
        }
    }

    // GET: /Yakit/Sil/5
    public async Task<IActionResult> Sil(int id)
    {
        var dto = await yakitService.GetirAsync(id);
        if (dto is null)
        {
            TempData["Hata"] = "Yakıt kaydı bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
        return View(dto);
    }

    // POST: /Yakit/Sil/5
    [HttpPost, ActionName("Sil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SilOnayla(int id)
    {
        try
        {
            await yakitService.SilAsync(id);
            TempData["Basari"] = "Yakıt kaydı silindi.";
        }
        catch (Exception ex)
        {
            TempData["Hata"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task DropdownDoldurAsync()
    {
        var araclar = await aracService.TumunuGetirAsync();
        ViewBag.AracListesi = araclar
            .Select(a => new SelectListItem($"{a.Plaka} – {a.Marka} {a.Model}", a.AracId.ToString()))
            .ToList();
    }
}
