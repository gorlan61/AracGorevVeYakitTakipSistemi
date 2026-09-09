using AracGorevVeYakitTakipSistemi.Application.DTOs;
using AracGorevVeYakitTakipSistemi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AracGorevVeYakitTakipSistemi.Web.Controllers;

[Authorize]
public class SuruculerController(ISurucuService surucuService) : Controller
{
    // GET: /Suruculer
    public async Task<IActionResult> Index()
    {
        var liste = await surucuService.TumunuGetirAsync();
        return View(liste);
    }

    // GET: /Suruculer/Detay/5
    public async Task<IActionResult> Detay(int id)
    {
        var dto = await surucuService.GetirAsync(id);
        if (dto is null)
        {
            TempData["Hata"] = "Sürücü bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
        return View(dto);
    }

    // GET: /Suruculer/Yeni
    public IActionResult Yeni() => View("YeniDuzenle", new SurucuDto());

    // POST: /Suruculer/Yeni
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Yeni(SurucuDto dto)
    {
        if (!ModelState.IsValid)
            return View("YeniDuzenle", dto);

        await surucuService.EkleAsync(dto);
        TempData["Basari"] = $"'{dto.AdSoyad}' adlı sürücü başarıyla eklendi.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Suruculer/Duzenle/5
    public async Task<IActionResult> Duzenle(int id)
    {
        var dto = await surucuService.GetirAsync(id);
        if (dto is null)
        {
            TempData["Hata"] = "Sürücü bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
        return View("YeniDuzenle", dto);
    }

    // POST: /Suruculer/Duzenle/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(int id, SurucuDto dto)
    {
        if (id != dto.SurucuId)
            return BadRequest();

        if (!ModelState.IsValid)
            return View("YeniDuzenle", dto);

        try
        {
            await surucuService.GuncelleAsync(dto);
            TempData["Basari"] = $"'{dto.AdSoyad}' adlı sürücü güncellendi.";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            TempData["Hata"] = "Sürücü bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: /Suruculer/Sil/5
    public async Task<IActionResult> Sil(int id)
    {
        var dto = await surucuService.GetirAsync(id);
        if (dto is null)
        {
            TempData["Hata"] = "Sürücü bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
        return View(dto);
    }

    // POST: /Suruculer/Sil/5
    [HttpPost, ActionName("Sil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SilOnayla(int id)
    {
        try
        {
            await surucuService.SilAsync(id);
            TempData["Basari"] = "Sürücü başarıyla silindi.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Hata"] = ex.Message;
        }
        catch (KeyNotFoundException)
        {
            TempData["Hata"] = "Sürücü bulunamadı.";
        }
        return RedirectToAction(nameof(Index));
    }
}
