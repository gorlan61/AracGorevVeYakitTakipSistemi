using AracGorevVeYakitTakipSistemi.Application.DTOs;
using AracGorevVeYakitTakipSistemi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AracGorevVeYakitTakipSistemi.Web.Controllers;

[Authorize]
public class AraclarController(IAracService aracService) : Controller
{
    // GET: /Araclar
    public async Task<IActionResult> Index()
    {
        var liste = await aracService.TumunuGetirAsync();
        return View(liste);
    }

    // GET: /Araclar/Detay/5
    public async Task<IActionResult> Detay(int id)
    {
        var dto = await aracService.GetirAsync(id);
        if (dto is null)
        {
            TempData["Hata"] = "Araç bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
        return View(dto);
    }

    // GET: /Araclar/Yeni
    public IActionResult Yeni() => View("YeniDuzenle", new AracDto { ModelYili = DateTime.Now.Year });

    // POST: /Araclar/Yeni
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Yeni(AracDto dto)
    {
        if (!ModelState.IsValid)
            return View("YeniDuzenle", dto);

        try
        {
            await aracService.EkleAsync(dto);
            TempData["Basari"] = $"'{dto.Plaka}' plakalı araç başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("YeniDuzenle", dto);
        }
    }

    // GET: /Araclar/Duzenle/5
    public async Task<IActionResult> Duzenle(int id)
    {
        var dto = await aracService.GetirAsync(id);
        if (dto is null)
        {
            TempData["Hata"] = "Araç bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
        return View("YeniDuzenle", dto);
    }

    // POST: /Araclar/Duzenle/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(int id, AracDto dto)
    {
        if (id != dto.AracId)
            return BadRequest();

        if (!ModelState.IsValid)
            return View("YeniDuzenle", dto);

        try
        {
            await aracService.GuncelleAsync(dto);
            TempData["Basari"] = $"'{dto.Plaka}' plakalı araç güncellendi.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("YeniDuzenle", dto);
        }
        catch (KeyNotFoundException)
        {
            TempData["Hata"] = "Araç bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: /Araclar/Sil/5
    public async Task<IActionResult> Sil(int id)
    {
        var dto = await aracService.GetirAsync(id);
        if (dto is null)
        {
            TempData["Hata"] = "Araç bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
        return View(dto);
    }

    // POST: /Araclar/Sil/5
    [HttpPost, ActionName("Sil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SilOnayla(int id)
    {
        try
        {
            await aracService.SilAsync(id);
            TempData["Basari"] = "Araç başarıyla silindi.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Hata"] = ex.Message;
        }
        catch (KeyNotFoundException)
        {
            TempData["Hata"] = "Araç bulunamadı.";
        }
        return RedirectToAction(nameof(Index));
    }
}
