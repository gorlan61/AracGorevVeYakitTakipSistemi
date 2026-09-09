using AracGorevVeYakitTakipSistemi.Application.DTOs;

namespace AracGorevVeYakitTakipSistemi.Application.Interfaces;

public interface IYakitService
{
    Task<IReadOnlyList<YakitKaydiDto>> TumunuGetirAsync();
    Task<IReadOnlyList<YakitKaydiDto>> AracGoreGetirAsync(int aracId);
    Task<YakitKaydiDto?> GetirAsync(int yakitId);
    Task EkleAsync(YakitKaydiDto dto);
    Task GuncelleAsync(YakitKaydiDto dto);
    Task SilAsync(int yakitId);
    Task<IReadOnlyList<YakitTuketimAnaliziDto>> TumAraclarTuketimAnaliziGetirAsync();
}
