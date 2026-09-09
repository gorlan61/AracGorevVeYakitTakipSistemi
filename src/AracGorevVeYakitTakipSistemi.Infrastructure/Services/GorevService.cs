using AracGorevVeYakitTakipSistemi.Application.DTOs;
using AracGorevVeYakitTakipSistemi.Application.Interfaces;
using AracGorevVeYakitTakipSistemi.Domain.Entities;
using AracGorevVeYakitTakipSistemi.Domain.Enums;
using AracGorevVeYakitTakipSistemi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AracGorevVeYakitTakipSistemi.Infrastructure.Services;

public class GorevService(UygulamaDbContext db, IConfiguration config) : IGorevService
{
    private int AnomaliEsigi => int.TryParse(config["UygulamaAyarlari:AnomaliEsigi"], out var val) ? val : 500;

    public async Task<IReadOnlyList<GorevDto>> TumunuGetirAsync() =>
        await db.Gorevler
            .AsNoTracking()
            .Include(g => g.Arac)
            .Include(g => g.Surucu)
            .OrderByDescending(g => g.GorevTarihi)
            .Select(g => DtoyaDonustur(g))
            .ToListAsync();

    public async Task<GorevDto?> GetirAsync(int gorevId)
    {
        var gorev = await db.Gorevler
            .AsNoTracking()
            .Include(g => g.Arac)
            .Include(g => g.Surucu)
            .FirstOrDefaultAsync(g => g.GorevId == gorevId);
        return gorev is null ? null : DtoyaDonustur(gorev);
    }

    public async Task EkleAsync(GorevDto dto)
    {
        // İş Kuralı 1: Çakışma kontrolü (aynı araç veya aynı sürücü aynı tarihte)
        await CakismaKontrolEtAsync(dto.AracId, dto.SurucuId, dto.GorevTarihi);

        var gorev = new Gorev
        {
            AracId = dto.AracId,
            SurucuId = dto.SurucuId,
            GorevTarihi = dto.GorevTarihi.Date,
            GidilecekYer = dto.GidilecekYer.Trim(),
            GorevAmaci = dto.GorevAmaci.Trim(),
            Durum = GorevDurumu.Planlandi
        };
        db.Gorevler.Add(gorev);
        await db.SaveChangesAsync();
    }

    public async Task<(bool AnomaliVar, int ToplamMesafe)> KmGuncelleAsync(
        int gorevId, int baslangicKm, int bitisKm)
    {
        if (bitisKm < baslangicKm)
            throw new InvalidOperationException("Bitiş km başlangıç km'den küçük olamaz.");

        var gorev = await db.Gorevler.Include(g => g.Arac).FirstOrDefaultAsync(g => g.GorevId == gorevId)
            ?? throw new KeyNotFoundException("Görev bulunamadı.");

        int toplamMesafe = bitisKm - baslangicKm;
        bool anomaliVar = toplamMesafe > AnomaliEsigi;

        gorev.BaslangicKm = baslangicKm;
        gorev.BitisKm = bitisKm;
        gorev.ToplamMesafe = toplamMesafe;
        gorev.Durum = GorevDurumu.Tamamlandi;

        // İş Kuralı 3: Görev tamamlanınca aracın güncel km'si güncelle
        if (bitisKm > gorev.Arac.GuncelKm)
            gorev.Arac.GuncelKm = bitisKm;

        await db.SaveChangesAsync();
        return (anomaliVar, toplamMesafe);
    }

    public async Task DurumGuncelleAsync(int gorevId, GorevDurumu yeniDurum)
    {
        var gorev = await db.Gorevler.FindAsync(gorevId)
            ?? throw new KeyNotFoundException("Görev bulunamadı.");
        gorev.Durum = yeniDurum;
        await db.SaveChangesAsync();
    }

    public async Task SilAsync(int gorevId)
    {
        var gorev = await db.Gorevler.FindAsync(gorevId)
            ?? throw new KeyNotFoundException("Görev bulunamadı.");

        if (gorev.Durum == GorevDurumu.DevamEdiyor)
            throw new InvalidOperationException("Devam eden görev silinemez.");

        db.Gorevler.Remove(gorev);
        await db.SaveChangesAsync();
    }

    // ─── Yardımcı metotlar ───────────────────────────────────────────────────

    private async Task CakismaKontrolEtAsync(int aracId, int surucuId, DateTime tarih, int? haricGorevId = null)
    {
        // Aktif (Planlandı veya Devam Ediyor) görevlerde aynı tarih kontrolü
        var cakisanGorevVar = await db.Gorevler.AnyAsync(g =>
            g.GorevId != (haricGorevId ?? 0) &&
            g.GorevTarihi.Date == tarih.Date &&
            (g.Durum == GorevDurumu.Planlandi || g.Durum == GorevDurumu.DevamEdiyor) &&
            (g.AracId == aracId || g.SurucuId == surucuId));

        if (cakisanGorevVar)
            throw new InvalidOperationException(
                "Seçilen araç veya sürücü bu tarihte başka bir göreve atanmış. Lütfen farklı bir tarih, araç veya sürücü seçin.");
    }

    private static GorevDto DtoyaDonustur(Gorev g) => new()
    {
        GorevId = g.GorevId,
        AracId = g.AracId,
        SurucuId = g.SurucuId,
        GorevTarihi = g.GorevTarihi,
        GidilecekYer = g.GidilecekYer,
        GorevAmaci = g.GorevAmaci,
        BaslangicKm = g.BaslangicKm,
        BitisKm = g.BitisKm,
        ToplamMesafe = g.ToplamMesafe,
        Durum = g.Durum,
        AracPlaka = g.Arac?.Plaka ?? string.Empty,
        SurucuAdSoyad = g.Surucu?.AdSoyad ?? string.Empty
    };
}
