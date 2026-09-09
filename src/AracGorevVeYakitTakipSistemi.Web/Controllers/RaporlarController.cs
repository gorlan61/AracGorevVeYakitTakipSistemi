using AracGorevVeYakitTakipSistemi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AracGorevVeYakitTakipSistemi.Web.Controllers;

[Authorize]
public class RaporlarController(IRaporService raporService) : Controller
{
    // GET: /Raporlar
    public async Task<IActionResult> Index()
    {
        var data = await raporService.RaporVerileriniGetirAsync();
        return View(data);
    }
}
