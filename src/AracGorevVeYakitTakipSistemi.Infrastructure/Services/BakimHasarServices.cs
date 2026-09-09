using AracGorevVeYakitTakipSistemi.Application.DTOs;
using AracGorevVeYakitTakipSistemi.Application.Interfaces;
using AracGorevVeYakitTakipSistemi.Domain.Entities;
using AracGorevVeYakitTakipSistemi.Domain.Enums;
using AracGorevVeYakitTakipSistemi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AracGorevVeYakitTakipSistemi.Infrastructure.Services;

public class BakimService(UygulamaDbContext db, IConfiguration config) : IBakimService
{
    private int YaklasanBakimGunSayisi => int.TryParse(config["UygulamaAyarlari:YaklasanBakimGunSayisi"], out var val) ? val : 30;

    public async Task<IReadOnlyList<BakimDto>> TumunuGetirAsync()
    {
        var bakimlar = await db.Bakimlar
            .AsNoTracking()
            .Include(b => b.Arac)
            .OrderByDescending(b => b.YapilanTarih)
            .ToListAsync();

        return bakimlar.Select(b => DtoyaDonustur(b)).ToList();
    }

    public async Task<BakimDto?> GetirAsync(int bakimId)
    {
        var b = await db.Bakimlar
            .AsNoTracking()
            .Include(b => b.Arac)
            .FirstOrDefaultAsync(b => b.BakimId == bakimId);
        return b is null ? null : DtoyaDonustur(b);
    }

    public async Task EkleAsync(BakimDto dto)
    {
        var b = new Bakim
        {
            AracId = dto.AracId,
            BakimTuru = dto.BakimTuru,
            YapilanTarih = dto.YapilanTarih,
            SonrakiTarih = dto.SonrakiTarih,
            Aciklama = dto.Aciklama.Trim(),
            Maliyet = dto.Maliyet
        };

        db.Bakimlar.Add(b);
        await db.SaveChangesAsync();
    }

    public async Task GuncelleAsync(BakimDto dto)
    {
        var b = await db.Bakimlar.FindAsync(dto.BakimId)
            ?? throw new KeyNotFoundException("Bakım kaydı bulunamadı.");

        b.AracId = dto.AracId;
        b.BakimTuru = dto.BakimTuru;
        b.YapilanTarih = dto.YapilanTarih;
        b.SonrakiTarih = dto.SonrakiTarih;
        b.Aciklama = dto.Aciklama.Trim();
        b.Maliyet = dto.Maliyet;

        await db.SaveChangesAsync();
    }

    public async Task SilAsync(int bakimId)
    {
        var b = await db.Bakimlar.FindAsync(bakimId)
            ?? throw new KeyNotFoundException("Bakım kaydı bulunamadı.");
        db.Bakimlar.Remove(b);
        await db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<BakimDto>> YaklasanBakimlariGetirAsync()
    {
        var bugün = DateTime.Today;
        var hedefTarih = bugün.AddDays(YaklasanBakimGunSayisi);

        var bakimlar = await db.Bakimlar
            .AsNoTracking()
            .Include(b => b.Arac)
            .Where(b => b.SonrakiTarih.HasValue && b.SonrakiTarih.Value >= bugün && b.SonrakiTarih.Value <= hedefTarih)
            .OrderBy(b => b.SonrakiTarih)
            .ToListAsync();

        return bakimlar.Select(b => DtoyaDonustur(b)).ToList();
    }

    private BakimDto DtoyaDonustur(Bakim b)
    {
        var bugün = DateTime.Today;
        int? kalanGun = b.SonrakiTarih.HasValue
            ? (int)(b.SonrakiTarih.Value.Date - bugün).TotalDays
            : null;

        bool yaklasti = kalanGun.HasValue && kalanGun.Value >= 0 && kalanGun.Value <= YaklasanBakimGunSayisi;

        return new BakimDto
        {
            BakimId = b.BakimId,
            AracId = b.AracId,
            BakimTuru = b.BakimTuru,
            YapilanTarih = b.YapilanTarih,
            SonrakiTarih = b.SonrakiTarih,
            Aciklama = b.Aciklama,
            Maliyet = b.Maliyet,
            AracPlaka = b.Arac?.Plaka ?? string.Empty,
            YaklastiMi = yaklasti,
            KalanGun = kalanGun
        };
    }
}

public class HasarService(UygulamaDbContext db) : IHasarService
{
    public async Task<IReadOnlyList<HasarDto>> TumunuGetirAsync() =>
        await db.Hasarlar
            .AsNoTracking()
            .Include(h => h.Arac)
            .OrderByDescending(h => h.Tarih)
            .Select(h => DtoyaDonustur(h))
            .ToListAsync();

    public async Task<HasarDto?> GetirAsync(int hasarId)
    {
        var h = await db.Hasarlar
            .AsNoTracking()
            .Include(h => h.Arac)
            .FirstOrDefaultAsync(h => h.HasarId == hasarId);
        return h is null ? null : DtoyaDonustur(h);
    }

    public async Task EkleAsync(HasarDto dto)
    {
        var h = new Hasar
        {
            AracId = dto.AracId,
            Tarih = dto.Tarih,
            Aciklama = dto.Aciklama.Trim(),
            FotografYolu = dto.FotografYolu ?? string.Empty,
            TahminiMaliyet = dto.TahminiMaliyet,
            OnarimDurumu = dto.OnarimDurumu
        };

        db.Hasarlar.Add(h);
        await db.SaveChangesAsync();
    }

    public async Task GuncelleAsync(HasarDto dto)
    {
        var h = await db.Hasarlar.FindAsync(dto.HasarId)
            ?? throw new KeyNotFoundException("Hasar kaydı bulunamadı.");

        h.AracId = dto.AracId;
        h.Tarih = dto.Tarih;
        h.Aciklama = dto.Aciklama.Trim();
        if (!string.IsNullOrEmpty(dto.FotografYolu))
            h.FotografYolu = dto.FotografYolu;
        h.TahminiMaliyet = dto.TahminiMaliyet;
        h.OnarimDurumu = dto.OnarimDurumu;

        await db.SaveChangesAsync();
    }

    public async Task DurumGuncelleAsync(int hasarId, OnarimDurumu yeniDurum)
    {
        var h = await db.Hasarlar.FindAsync(hasarId)
            ?? throw new KeyNotFoundException("Hasar kaydı bulunamadı.");

        h.OnarimDurumu = yeniDurum;
        await db.SaveChangesAsync();
    }

    public async Task SilAsync(int hasarId)
    {
        var h = await db.Hasarlar.FindAsync(hasarId)
            ?? throw new KeyNotFoundException("Hasar kaydı bulunamadı.");

        db.Hasarlar.Remove(h);
        await db.SaveChangesAsync();
    }

    private static HasarDto DtoyaDonustur(Hasar h) => new()
    {
        HasarId = h.HasarId,
        AracId = h.AracId,
        Tarih = h.Tarih,
        Aciklama = h.Aciklama,
        FotografYolu = h.FotografYolu,
        TahminiMaliyet = h.TahminiMaliyet,
        OnarimDurumu = h.OnarimDurumu,
        AracPlaka = h.Arac?.Plaka ?? string.Empty
    };
}
