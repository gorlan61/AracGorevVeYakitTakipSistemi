using AracGorevVeYakitTakipSistemi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AracGorevVeYakitTakipSistemi.Web.Areas.Identity.Pages.Account;

public class LogoutModel(SignInManager<Kullanici> signInManager) : PageModel
{
    public async Task<IActionResult> OnPostAsync()
    {
        await signInManager.SignOutAsync();
        return LocalRedirect("~/");
    }
}
