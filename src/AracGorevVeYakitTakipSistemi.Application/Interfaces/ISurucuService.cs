using AracGorevVeYakitTakipSistemi.Application.DTOs;

namespace AracGorevVeYakitTakipSistemi.Application.Interfaces;

public interface ISurucuService
{
    Task<IReadOnlyList<SurucuDto>> TumunuGetirAsync();
    Task<SurucuDto?> GetirAsync(int SurucuId);
    Task EkleAsync(SurucuDto surucuDto);
    Task GuncelleAsync(SurucuDto surucuDto);
    Task SilAsync(int SurucuId);
}
