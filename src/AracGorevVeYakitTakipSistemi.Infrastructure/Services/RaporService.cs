using AracGorevVeYakitTakipSistemi.Application.DTOs;
using AracGorevVeYakitTakipSistemi.Application.Interfaces;
using AracGorevVeYakitTakipSistemi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AracGorevVeYakitTakipSistemi.Infrastructure.Services;

public class RaporService(UygulamaDbContext db, IBakimService bakimService) : IRaporService
{
    public async Task<RaporDataDto> RaporVerileriniGetirAsync()
    {
        var araclar = await db.Araclar.AsNoTracking().ToListAsync();
        var gorevler = await db.Gorevler.AsNoTracking().ToListAsync();
        var yakitlar = await db.YakitKayitlari.AsNoTracking().ToListAsync();
        var bakimlar = await db.Bakimlar.AsNoTracking().ToListAsync();
        var hasarlar = await db.Hasarlar.AsNoTracking().ToListAsync();

        var aracMaliyetListesi = new List<AracMaliyetOzetDto>();

        foreach (var a in araclar)
        {
            var aGorevler = gorevler.Where(g => g.AracId == a.AracId).ToList();
            var aYakitlar = yakitlar.Where(y => y.AracId == a.AracId).ToList();
            var aBakimlar = bakimlar.Where(b => b.AracId == a.AracId).ToList();
            var aHasarlar = hasarlar.Where(h => h.AracId == a.AracId).ToList();

            var yakitMaliyeti = aYakitlar.Sum(y => y.ToplamTutar);
            var bakimMaliyeti = aBakimlar.Sum(b => b.Maliyet);
            var hasarMaliyeti = aHasarlar.Sum(h => h.TahminiMaliyet);
            var toplamKm = aGorevler.Sum(g => g.ToplamMesafe ?? 0);

            aracMaliyetListesi.Add(new AracMaliyetOzetDto
            {
                AracId = a.AracId,
                Plaka = a.Plaka,
                MarkaModel = $"{a.Marka} {a.Model}",
                GorevSayisi = aGorevler.Count,
                ToplamKm = toplamKm,
                YakitMaliyeti = yakitMaliyeti,
                BakimMaliyeti = bakimMaliyeti,
                HasarMaliyeti = hasarMaliyeti
            });
        }

        // Aylık yakıt tüketim grubu
        var aylikYakit = yakitlar
            .GroupBy(y => new { y.Tarih.Year, y.Tarih.Month })
            .OrderByDescending(g => g.Key.Year).ThenByDescending(g => g.Key.Month)
            .Select(g => new AylikYakitOzetDto
            {
                Yil = g.Key.Year,
                Ay = g.Key.Month,
                ToplamLitre = g.Sum(y => y.Litre),
                ToplamTutar = g.Sum(y => y.ToplamTutar)
            })
            .ToList();

        var yaklasanBakimlar = await bakimService.YaklasanBakimlariGetirAsync();

        return new RaporDataDto
        {
            FiloToplamYakitMaliyeti = yakitlar.Sum(y => y.ToplamTutar),
            FiloToplamBakimMaliyeti = bakimlar.Sum(b => b.Maliyet),
            FiloToplamHasarMaliyeti = hasarlar.Sum(h => h.TahminiMaliyet),
            FiloToplamMaliyet = yakitlar.Sum(y => y.ToplamTutar) + bakimlar.Sum(b => b.Maliyet) + hasarlar.Sum(h => h.TahminiMaliyet),
            AracMaliyetleri = aracMaliyetListesi.OrderByDescending(m => m.ToplamMaliyet).ToList(),
            EnCokKullanilanAraclar = aracMaliyetListesi.OrderByDescending(m => m.GorevSayisi).ThenByDescending(m => m.ToplamKm).Take(5).ToList(),
            EnMaliyetliAraclar = aracMaliyetListesi.OrderByDescending(m => m.ToplamMaliyet).Take(5).ToList(),
            AylikYakitTuketimi = aylikYakit,
            YaklasanBakimlar = yaklasanBakimlar
        };
    }
}
