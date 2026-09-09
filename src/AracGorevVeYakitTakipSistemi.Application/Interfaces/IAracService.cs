using AracGorevVeYakitTakipSistemi.Application.DTOs;

namespace AracGorevVeYakitTakipSistemi.Application.Interfaces;

public interface IAracService
{
    Task<IReadOnlyList<AracDto>> TumunuGetirAsync();
    Task<AracDto?> GetirAsync(int AracId);
    Task EkleAsync(AracDto aracDto);
    Task GuncelleAsync(AracDto aracDto);
    Task SilAsync(int AracId);
}
