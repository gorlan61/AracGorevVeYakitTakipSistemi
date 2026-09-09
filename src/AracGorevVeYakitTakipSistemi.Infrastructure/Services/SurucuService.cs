using AracGorevVeYakitTakipSistemi.Application.DTOs;
using AracGorevVeYakitTakipSistemi.Application.Interfaces;
using AracGorevVeYakitTakipSistemi.Domain.Entities;
using AracGorevVeYakitTakipSistemi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AracGorevVeYakitTakipSistemi.Infrastructure.Services;

public class SurucuService(UygulamaDbContext dbContext) : ISurucuService
{
    public async Task<IReadOnlyList<SurucuDto>> TumunuGetirAsync() =>
        await dbContext.Suruculer.AsNoTracking().OrderBy(x => x.AdSoyad).Select(x => DtoyaDonustur(x)).ToListAsync();

    public async Task<SurucuDto?> GetirAsync(int SurucuId)
    {
        var surucu = await dbContext.Suruculer.AsNoTracking().FirstOrDefaultAsync(x => x.SurucuId == SurucuId);
        return surucu is null ? null : DtoyaDonustur(surucu);
    }

    public async Task EkleAsync(SurucuDto surucuDto)
    {
        var surucu = new Surucu();
        DtoyuUygula(surucuDto, surucu);
        surucu.KayitTarihi = DateTime.UtcNow;
        dbContext.Suruculer.Add(surucu);
        await dbContext.SaveChangesAsync();
    }

    public async Task GuncelleAsync(SurucuDto surucuDto)
    {
        var surucu = await dbContext.Suruculer.FindAsync(surucuDto.SurucuId)
            ?? throw new KeyNotFoundException("Sürücü bulunamadı.");
        DtoyuUygula(surucuDto, surucu);
        await dbContext.SaveChangesAsync();
    }

    public async Task SilAsync(int SurucuId)
    {
        var surucu = await dbContext.Suruculer.FindAsync(SurucuId)
            ?? throw new KeyNotFoundException("Sürücü bulunamadı.");
        if (await dbContext.Gorevler.AnyAsync(x => x.SurucuId == SurucuId))
        {
            throw new InvalidOperationException("Bu sürücü görev kayıtlarında kullanıldığı için silinemez.");
        }

        dbContext.Suruculer.Remove(surucu);
        await dbContext.SaveChangesAsync();
    }

    private static SurucuDto DtoyaDonustur(Surucu surucu) => new()
    {
        SurucuId = surucu.SurucuId, AdSoyad = surucu.AdSoyad, EhliyetSinifi = surucu.EhliyetSinifi,
        Telefon = surucu.Telefon, Email = surucu.Email, AktifMi = surucu.AktifMi, KayitTarihi = surucu.KayitTarihi
    };

    private static void DtoyuUygula(SurucuDto surucuDto, Surucu surucu)
    {
        surucu.AdSoyad = surucuDto.AdSoyad.Trim();
        surucu.EhliyetSinifi = surucuDto.EhliyetSinifi.Trim();
        surucu.Telefon = surucuDto.Telefon.Trim();
        surucu.Email = surucuDto.Email.Trim();
        surucu.AktifMi = surucuDto.AktifMi;
    }
}
