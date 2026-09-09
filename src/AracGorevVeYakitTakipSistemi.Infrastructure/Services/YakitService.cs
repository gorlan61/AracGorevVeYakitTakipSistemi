using AracGorevVeYakitTakipSistemi.Application.DTOs;
using AracGorevVeYakitTakipSistemi.Application.Interfaces;
using AracGorevVeYakitTakipSistemi.Domain.Entities;
using AracGorevVeYakitTakipSistemi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AracGorevVeYakitTakipSistemi.Infrastructure.Services;

public class YakitService(UygulamaDbContext db) : IYakitService
{
    public async Task<IReadOnlyList<YakitKaydiDto>> TumunuGetirAsync() =>
        await db.YakitKayitlari
            .AsNoTracking()
            .Include(y => y.Arac)
            .OrderByDescending(y => y.Tarih)
            .Select(y => DtoyaDonustur(y))
            .ToListAsync();

    public async Task<IReadOnlyList<YakitKaydiDto>> AracGoreGetirAsync(int aracId) =>
        await db.YakitKayitlari
            .AsNoTracking()
            .Include(y => y.Arac)
            .Where(y => y.AracId == aracId)
            .OrderByDescending(y => y.Tarih)
            .Select(y => DtoyaDonustur(y))
            .ToListAsync();

    public async Task<YakitKaydiDto?> GetirAsync(int yakitId)
    {
        var y = await db.YakitKayitlari
            .AsNoTracking()
            .Include(y => y.Arac)
            .FirstOrDefaultAsync(y => y.YakitId == yakitId);
        return y is null ? null : DtoyaDonustur(y);
    }

    public async Task EkleAsync(YakitKaydiDto dto)
    {
        var arac = await db.Araclar.FindAsync(dto.AracId)
            ?? throw new KeyNotFoundException("Araç bulunamadı.");

        var y = new YakitKaydi
        {
            AracId = dto.AracId,
            Tarih = dto.Tarih,
            Litre = dto.Litre,
            ToplamTutar = dto.ToplamTutar,
            FisNo = dto.FisNo.Trim(),
            Km = dto.Km
        };

        if (dto.Km > arac.GuncelKm)
        {
            arac.GuncelKm = dto.Km;
        }

        db.YakitKayitlari.Add(y);
        await db.SaveChangesAsync();
    }

    public async Task GuncelleAsync(YakitKaydiDto dto)
    {
        var y = await db.YakitKayitlari.FindAsync(dto.YakitId)
            ?? throw new KeyNotFoundException("Yakıt kaydı bulunamadı.");

        y.AracId = dto.AracId;
        y.Tarih = dto.Tarih;
        y.Litre = dto.Litre;
        y.ToplamTutar = dto.ToplamTutar;
        y.FisNo = dto.FisNo.Trim();
        y.Km = dto.Km;

        var arac = await db.Araclar.FindAsync(dto.AracId);
        if (arac != null && dto.Km > arac.GuncelKm)
        {
            arac.GuncelKm = dto.Km;
        }

        await db.SaveChangesAsync();
    }

    public async Task SilAsync(int yakitId)
    {
        var y = await db.YakitKayitlari.FindAsync(yakitId)
            ?? throw new KeyNotFoundException("Yakıt kaydı bulunamadı.");
        db.YakitKayitlari.Remove(y);
        await db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<YakitTuketimAnaliziDto>> TumAraclarTuketimAnaliziGetirAsync()
    {
        var araclar = await db.Araclar.AsNoTracking().ToListAsync();
        var yakitlar = await db.YakitKayitlari.AsNoTracking().ToListAsync();

        var liste = new List<YakitTuketimAnaliziDto>();

        foreach (var arac in araclar)
        {
            var aracYakitlari = yakitlar.Where(y => y.AracId == arac.AracId).ToList();
            if (!aracYakitlari.Any()) continue;

            liste.Add(new YakitTuketimAnaliziDto
            {
                AracId = arac.AracId,
                Plaka = arac.Plaka,
                MarkaModel = $"{arac.Marka} {arac.Model}",
                ToplamLitre = aracYakitlari.Sum(y => y.Litre),
                ToplamTutar = aracYakitlari.Sum(y => y.ToplamTutar),
                EnKucukKm = aracYakitlari.Min(y => y.Km),
                EnBuyukKm = aracYakitlari.Max(y => y.Km)
            });
        }

        return liste;
    }

    private static YakitKaydiDto DtoyaDonustur(YakitKaydi y) => new()
    {
        YakitId = y.YakitId,
        AracId = y.AracId,
        Tarih = y.Tarih,
        Litre = y.Litre,
        ToplamTutar = y.ToplamTutar,
        FisNo = y.FisNo,
        Km = y.Km,
        AracPlaka = y.Arac?.Plaka ?? string.Empty
    };
}
