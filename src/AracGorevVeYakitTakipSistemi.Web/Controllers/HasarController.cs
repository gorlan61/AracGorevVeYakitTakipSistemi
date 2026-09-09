using AracGorevVeYakitTakipSistemi.Application.DTOs;
using AracGorevVeYakitTakipSistemi.Application.Interfaces;
using AracGorevVeYakitTakipSistemi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AracGorevVeYakitTakipSistemi.Web.Controllers;

[Authorize]
public class HasarController(
    IHasarService hasarService,
    IAracService aracService,
    IWebHostEnvironment environment) : Controller
{
    // GET: /Hasar
    public async Task<IActionResult> Index()
    {
        var hasarlar = await hasarService.TumunuGetirAsync();
        return View(hasarlar);
    }

    // GET: /Hasar/Detay/5
    public async Task<IActionResult> Detay(int id)
    {
        var dto = await hasarService.GetirAsync(id);
        if (dto is null)
        {
            TempData["Hata"] = "Hasar kaydı bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
        return View(dto);
    }

    // GET: /Hasar/Yeni
    public async Task<IActionResult> Yeni(int? aracId)
    {
        await DropdownDoldurAsync();
        return View("YeniDuzenle", new HasarDto
        {
            AracId = aracId ?? 0,
            Tarih = DateTime.Today
        });
    }

    // POST: /Hasar/Yeni
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Yeni(HasarDto dto, IFormFile? fotograf)
    {
        if (!ModelState.IsValid)
        {
            await DropdownDoldurAsync();
            return View("YeniDuzenle", dto);
        }

        try
        {
            if (fotograf != null && fotograf.Length > 0)
            {
                dto.FotografYolu = await FotografKaydetAsync(fotograf);
            }

            await hasarService.EkleAsync(dto);
            TempData["Basari"] = "Hasar kaydı oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await DropdownDoldurAsync();
            return View("YeniDuzenle", dto);
        }
    }

    // GET: /Hasar/Duzenle/5
    public async Task<IActionResult> Duzenle(int id)
    {
        var dto = await hasarService.GetirAsync(id);
        if (dto is null)
        {
            TempData["Hata"] = "Hasar kaydı bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
        await DropdownDoldurAsync();
        return View("YeniDuzenle", dto);
    }

    // POST: /Hasar/Duzenle/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(int id, HasarDto dto, IFormFile? fotograf)
    {
        if (id != dto.HasarId)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            await DropdownDoldurAsync();
            return View("YeniDuzenle", dto);
        }

        try
        {
            if (fotograf != null && fotograf.Length > 0)
            {
                dto.FotografYolu = await FotografKaydetAsync(fotograf);
            }

            await hasarService.GuncelleAsync(dto);
            TempData["Basari"] = "Hasar kaydı güncellendi.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await DropdownDoldurAsync();
            return View("YeniDuzenle", dto);
        }
    }

    // POST: /Hasar/DurumGuncelle
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DurumGuncelle(int id, OnarimDurumu durum)
    {
        try
        {
            await hasarService.DurumGuncelleAsync(id, durum);
            TempData["Basari"] = "Onarım durumu güncellendi.";
        }
        catch (Exception ex)
        {
            TempData["Hata"] = ex.Message;
        }
        return RedirectToAction(nameof(Detay), new { id });
    }

    // GET: /Hasar/Sil/5
    public async Task<IActionResult> Sil(int id)
    {
        var dto = await hasarService.GetirAsync(id);
        if (dto is null)
        {
            TempData["Hata"] = "Hasar kaydı bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
        return View(dto);
    }

    // POST: /Hasar/Sil/5
    [HttpPost, ActionName("Sil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SilOnayla(int id)
    {
        try
        {
            await hasarService.SilAsync(id);
            TempData["Basari"] = "Hasar kaydı silindi.";
        }
        catch (Exception ex)
        {
            TempData["Hata"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<string> FotografKaydetAsync(IFormFile dosya)
    {
        var klasor = Path.Combine(environment.WebRootPath, "uploads", "hasarlar");
        if (!Directory.Exists(klasor))
            Directory.CreateDirectory(klasor);

        var dosyaAdi = $"{Guid.NewGuid()}{Path.GetExtension(dosya.FileName)}";
        var tamYol = Path.Combine(klasor, dosyaAdi);

        using var stream = new FileStream(tamYol, FileMode.Create);
        await dosya.CopyToAsync(stream);

        return $"/uploads/hasarlar/{dosyaAdi}";
    }

    private async Task DropdownDoldurAsync()
    {
        var araclar = await aracService.TumunuGetirAsync();
        ViewBag.AracListesi = araclar
            .Select(a => new SelectListItem($"{a.Plaka} – {a.Marka} {a.Model}", a.AracId.ToString()))
            .ToList();
    }
}
