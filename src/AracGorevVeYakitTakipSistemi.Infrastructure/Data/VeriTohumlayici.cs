using AracGorevVeYakitTakipSistemi.Domain.Entities;
using AracGorevVeYakitTakipSistemi.Domain.Enums;
using AracGorevVeYakitTakipSistemi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AracGorevVeYakitTakipSistemi.Infrastructure.Data;

public static class VeriTohumlayici
{
    public static async Task VerileriTohumlaAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UygulamaDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Kullanici>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // 1. Veritabanı ve Migration'ları Otomatik Uygula
        await context.Database.MigrateAsync();

        // 2. Rolleri Oluştur
        string[] roller = ["Admin", "Operatör"];
        foreach (var rol in roller)
        {
            if (!await roleManager.RoleExistsAsync(rol))
            {
                await roleManager.CreateAsync(new IdentityRole(rol));
            }
        }

        // 3. Varsayılan Kullanıcıları Oluştur
        var adminEmail = "admin@aractakip.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new Kullanici
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        var operatorEmail = "operator@aractakip.com";
        var operatorUser = await userManager.FindByEmailAsync(operatorEmail);
        if (operatorUser == null)
        {
            operatorUser = new Kullanici
            {
                UserName = "operator",
                Email = operatorEmail,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(operatorUser, "Operator123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(operatorUser, "Operatör");
            }
        }

        // 4. Örnek Araçlar (Veritabanı boşsa)
        if (!await context.Araclar.AnyAsync())
        {
            var arac1 = new Arac
            {
                Plaka = "34ABC123",
                Marka = "Ford",
                Model = "Transit",
                ModelYili = 2022,
                YakitTuru = YakitTuru.Dizel,
                RuhsatNo = "RH-100234",
                GuncelKm = 45200,
                Durum = AracDurumu.Aktif,
                KayitTarihi = DateTime.UtcNow.AddMonths(-6)
            };
            var arac2 = new Arac
            {
                Plaka = "06DEF456",
                Marka = "Renault",
                Model = "Megane",
                ModelYili = 2023,
                YakitTuru = YakitTuru.Benzin,
                RuhsatNo = "RH-200567",
                GuncelKm = 28100,
                Durum = AracDurumu.Aktif,
                KayitTarihi = DateTime.UtcNow.AddMonths(-4)
            };
            var arac3 = new Arac
            {
                Plaka = "35GHI789",
                Marka = "Fiat",
                Model = "Egea",
                ModelYili = 2021,
                YakitTuru = YakitTuru.LPG,
                RuhsatNo = "RH-300890",
                GuncelKm = 68400,
                Durum = AracDurumu.Bakimda,
                KayitTarihi = DateTime.UtcNow.AddMonths(-8)
            };

            await context.Araclar.AddRangeAsync(arac1, arac2, arac3);
            await context.SaveChangesAsync();

            // 5. Örnek Sürücüler
            var surucu1 = new Surucu
            {
                AdSoyad = "Ahmet Yılmaz",
                EhliyetSinifi = "C",
                Telefon = "0532 111 22 33",
                Email = "ahmet.yilmaz@kurum.gov.tr",
                AktifMi = true,
                KayitTarihi = DateTime.UtcNow.AddMonths(-6)
            };
            var surucu2 = new Surucu
            {
                AdSoyad = "Mehmet Demir",
                EhliyetSinifi = "B",
                Telefon = "0533 444 55 66",
                Email = "mehmet.demir@kurum.gov.tr",
                AktifMi = true,
                KayitTarihi = DateTime.UtcNow.AddMonths(-5)
            };

            await context.Suruculer.AddRangeAsync(surucu1, surucu2);
            await context.SaveChangesAsync();

            // 6. Örnek Görevler
            var gorev1 = new Gorev
            {
                AracId = arac1.AracId,
                SurucuId = surucu1.SurucuId,
                GorevTarihi = DateTime.Today.AddDays(-10),
                GidilecekYer = "Ankara Bölge Müdürlüğü",
                GorevAmaci = "Evrak ve teçhizat teslimatı",
                BaslangicKm = 44800,
                BitisKm = 45200,
                ToplamMesafe = 400,
                Durum = GorevDurumu.Tamamlandi
            };
            var gorev2 = new Gorev
            {
                AracId = arac2.AracId,
                SurucuId = surucu2.SurucuId,
                GorevTarihi = DateTime.Today.AddDays(2),
                GidilecekYer = "İstanbul Saha İncelemesi",
                GorevAmaci = "Saha denetim görevi",
                Durum = GorevDurumu.Planlandi
            };

            await context.Gorevler.AddRangeAsync(gorev1, gorev2);

            // 7. Örnek Yakıt Kayıtları
            var yakit1 = new YakitKaydi
            {
                AracId = arac1.AracId,
                Tarih = DateTime.Today.AddDays(-12),
                Litre = 45.5m,
                ToplamTutar = 1950.00m,
                FisNo = "FIS-2026-101",
                Km = 44800
            };
            var yakit2 = new YakitKaydi
            {
                AracId = arac2.AracId,
                Tarih = DateTime.Today.AddDays(-5),
                Litre = 38.0m,
                ToplamTutar = 1710.00m,
                FisNo = "FIS-2026-102",
                Km = 28100
            };

            await context.YakitKayitlari.AddRangeAsync(yakit1, yakit2);

            // 8. Örnek Bakım Kayıtları (Biri yaklaşan bakım olarak!)
            var bakim1 = new Bakim
            {
                AracId = arac3.AracId,
                BakimTuru = BakimTuru.PeriyodikBakim,
                YapilanTarih = DateTime.Today.AddDays(-90),
                SonrakiTarih = DateTime.Today.AddDays(10), // 10 gün kaldı!
                Aciklama = "Periyodik yağ ve filtre değişimi",
                Maliyet = 3500.00m
            };

            await context.Bakimlar.AddAsync(bakim1);

            // 9. Örnek Hasar Kaydı
            var hasar1 = new Hasar
            {
                AracId = arac1.AracId,
                Tarih = DateTime.Today.AddDays(-20),
                Aciklama = "Park halindeyken sağ dikiz aynası sürtündü",
                FotografYolu = "",
                TahminiMaliyet = 1200.00m,
                OnarimDurumu = OnarimDurumu.Tamamlandi
            };

            await context.Hasarlar.AddAsync(hasar1);

            await context.SaveChangesAsync();
        }
    }
}
