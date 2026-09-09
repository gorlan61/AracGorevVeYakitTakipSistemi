using AracGorevVeYakitTakipSistemi.Application.Interfaces;
using AracGorevVeYakitTakipSistemi.Infrastructure.Data;
using AracGorevVeYakitTakipSistemi.Infrastructure.Identity;
using AracGorevVeYakitTakipSistemi.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<UygulamaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VarsayilanBaglanti")));
builder.Services.AddDefaultIdentity<Kullanici>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddErrorDescriber<TurkceKimlikHataTanimlayici>()
    .AddEntityFrameworkStores<UygulamaDbContext>();

// Uygulama servisleri
builder.Services.AddScoped<IAracService, AracService>();
builder.Services.AddScoped<ISurucuService, SurucuService>();
builder.Services.AddScoped<IGorevService, GorevService>();
builder.Services.AddScoped<IYakitService, YakitService>();
builder.Services.AddScoped<IBakimService, BakimService>();
builder.Services.AddScoped<IHasarService, HasarService>();
builder.Services.AddScoped<IRaporService, RaporService>();

var app = builder.Build();

// Veritabanı Otomatik Migration ve Örnek Veri Tohumlama
await VeriTohumlayici.VerileriTohumlaAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Türkçe Kültür / Format Desteği
var defaultCulture = new System.Globalization.CultureInfo("tr-TR");
defaultCulture.NumberFormat.NumberDecimalSeparator = ",";
defaultCulture.NumberFormat.CurrencyDecimalSeparator = ",";
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(defaultCulture),
    SupportedCultures = new[] { defaultCulture },
    SupportedUICultures = new[] { defaultCulture }
};
app.UseRequestLocalization(localizationOptions);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


app.Run();
