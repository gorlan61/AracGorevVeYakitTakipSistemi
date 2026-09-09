using AracGorevVeYakitTakipSistemi.Application.DTOs;
using AracGorevVeYakitTakipSistemi.Application.Interfaces;
using AracGorevVeYakitTakipSistemi.Domain.Entities;
using AracGorevVeYakitTakipSistemi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AracGorevVeYakitTakipSistemi.Infrastructure.Services;

public class AracService(UygulamaDbContext dbContext) : IAracService
{
    public async Task<IReadOnlyList<AracDto>> TumunuGetirAsync() =>
        await dbContext.Araclar.AsNoTracking().OrderBy(x => x.Plaka).Select(x => DtoyaDonustur(x)).ToListAsync();

    public async Task<AracDto?> GetirAsync(int AracId)
    {
        var arac = await dbContext.Araclar.AsNoTracking().FirstOrDefaultAsync(x => x.AracId == AracId);
        return arac is null ? null : DtoyaDonustur(arac);
    }

    public async Task EkleAsync(AracDto aracDto)
    {
        await PlakaBenzersizMiKontrolEtAsync(aracDto.Plaka);
        var arac = new Arac();
        DtoyuUygula(aracDto, arac);
        arac.KayitTarihi = DateTime.UtcNow;
        dbContext.Araclar.Add(arac);
        await dbContext.SaveChangesAsync();
    }

    public async Task GuncelleAsync(AracDto aracDto)
    {
        var arac = await dbContext.Araclar.FindAsync(aracDto.AracId)
            ?? throw new KeyNotFoundException("Araç bulunamadı.");
        await PlakaBenzersizMiKontrolEtAsync(aracDto.Plaka, aracDto.AracId);
        DtoyuUygula(aracDto, arac);
        await dbContext.SaveChangesAsync();
    }

    public async Task SilAsync(int AracId)
    {
        var arac = await dbContext.Araclar.FindAsync(AracId)
            ?? throw new KeyNotFoundException("Araç bulunamadı.");

        var kullaniliyor = await dbContext.Gorevler.AnyAsync(x => x.AracId == AracId)
            || await dbContext.YakitKayitlari.AnyAsync(x => x.AracId == AracId)
            || await dbContext.Bakimlar.AnyAsync(x => x.AracId == AracId)
            || await dbContext.Hasarlar.AnyAsync(x => x.AracId == AracId);
        if (kullaniliyor)
        {
            throw new InvalidOperationException("Bu araç başka kayıtlarda kullanıldığı için silinemez.");
        }

        dbContext.Araclar.Remove(arac);
        await dbContext.SaveChangesAsync();
    }

    private async Task PlakaBenzersizMiKontrolEtAsync(string Plaka, int? AracId = null)
    {
        var plakaVar = await dbContext.Araclar.AnyAsync(x =>
            x.Plaka == Plaka && (!AracId.HasValue || x.AracId != AracId.Value));
        if (plakaVar)
        {
            throw new InvalidOperationException("Bu plaka ile kayıtlı bir araç zaten var.");
        }
    }

    private static AracDto DtoyaDonustur(Arac arac) => new()
    {
        AracId = arac.AracId, Plaka = arac.Plaka, Marka = arac.Marka, Model = arac.Model,
        ModelYili = arac.ModelYili, YakitTuru = arac.YakitTuru, RuhsatNo = arac.RuhsatNo,
        GuncelKm = arac.GuncelKm, Durum = arac.Durum, KayitTarihi = arac.KayitTarihi
    };

    private static void DtoyuUygula(AracDto aracDto, Arac arac)
    {
        arac.Plaka = aracDto.Plaka.Trim().ToUpperInvariant();
        arac.Marka = aracDto.Marka.Trim();
        arac.Model = aracDto.Model.Trim();
        arac.ModelYili = aracDto.ModelYili;
        arac.YakitTuru = aracDto.YakitTuru;
        arac.RuhsatNo = aracDto.RuhsatNo.Trim();
        arac.GuncelKm = aracDto.GuncelKm;
        arac.Durum = aracDto.Durum;
    }
}
