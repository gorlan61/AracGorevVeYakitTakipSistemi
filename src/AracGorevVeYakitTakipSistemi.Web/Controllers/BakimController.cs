using AracGorevVeYakitTakipSistemi.Application.DTOs;
using AracGorevVeYakitTakipSistemi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AracGorevVeYakitTakipSistemi.Web.Controllers;

[Authorize]
public class BakimController(
    IBakimService bakimService,
    IAracService aracService) : Controller
{
    // GET: /Bakim
    public async Task<IActionResult> Index()
    {
        var bakimlar = await bakimService.TumunuGetirAsync();
        ViewBag.YaklasanBakimlar = await bakimService.YaklasanBakimlariGetirAsync();
        return View(bakimlar);
    }

    // GET: /Bakim/Detay/5
    public async Task<IActionResult> Detay(int id)
    {
        var dto = await bakimService.GetirAsync(id);
        if (dto is null)
        {
            TempData["Hata"] = "Bakım kaydı bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
        return View(dto);
    }

    // GET: /Bakim/Yeni
    public async Task<IActionResult> Yeni(int? aracId)
    {
        await DropdownDoldurAsync();
        return View("YeniDuzenle", new BakimDto
        {
            AracId = aracId ?? 0,
            YapilanTarih = DateTime.Today
        });
    }

    // POST: /Bakim/Yeni
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Yeni(BakimDto dto)
    {
        if (!ModelState.IsValid)
        {
            await DropdownDoldurAsync();
            return View("YeniDuzenle", dto);
        }

        try
        {
            await bakimService.EkleAsync(dto);
            TempData["Basari"] = "Bakım kaydı oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await DropdownDoldurAsync();
            return View("YeniDuzenle", dto);
        }
    }

    // GET: /Bakim/Duzenle/5
    public async Task<IActionResult> Duzenle(int id)
    {
        var dto = await bakimService.GetirAsync(id);
        if (dto is null)
        {
            TempData["Hata"] = "Bakım kaydı bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
        await DropdownDoldurAsync();
        return View("YeniDuzenle", dto);
    }

    // POST: /Bakim/Duzenle/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(int id, BakimDto dto)
    {
        if (id != dto.BakimId)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            await DropdownDoldurAsync();
            return View("YeniDuzenle", dto);
        }

        try
        {
            await bakimService.GuncelleAsync(dto);
            TempData["Basari"] = "Bakım kaydı güncellendi.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await DropdownDoldurAsync();
            return View("YeniDuzenle", dto);
        }
    }

    // GET: /Bakim/Sil/5
    public async Task<IActionResult> Sil(int id)
    {
        var dto = await bakimService.GetirAsync(id);
        if (dto is null)
        {
            TempData["Hata"] = "Bakım kaydı bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
        return View(dto);
    }

    // POST: /Bakim/Sil/5
    [HttpPost, ActionName("Sil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SilOnayla(int id)
    {
        try
        {
            await bakimService.SilAsync(id);
            TempData["Basari"] = "Bakım kaydı silindi.";
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
