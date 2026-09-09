using System.ComponentModel.DataAnnotations;
using AracGorevVeYakitTakipSistemi.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AracGorevVeYakitTakipSistemi.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class RegisterModel(
    UserManager<Kullanici> userManager,
    SignInManager<Kullanici> signInManager,
    RoleManager<IdentityRole> roleManager,
    ILogger<RegisterModel> logger) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [StringLength(100, ErrorMessage = "{0} en az {2} karakter olmalıdır.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Sifre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Sifre), ErrorMessage = "Şifre ve şifre tekrarı eşleşmiyor.")]
        public string SifreTekrar { get; set; } = string.Empty;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var kullanici = new Kullanici
        {
            UserName = Input.KullaniciAdi,
            Email = Input.Email
        };

        var result = await userManager.CreateAsync(kullanici, Input.Sifre);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        if (!await roleManager.RoleExistsAsync("Operatör"))
        {
            await roleManager.CreateAsync(new IdentityRole("Operatör"));
        }

        await userManager.AddToRoleAsync(kullanici, "Operatör");
        await signInManager.SignInAsync(kullanici, isPersistent: false);
        logger.LogInformation("Yeni kullanıcı oluşturuldu.");

        return LocalRedirect("~/");
    }
}
