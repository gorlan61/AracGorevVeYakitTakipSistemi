using AracGorevVeYakitTakipSistemi.Application.DTOs;
using AracGorevVeYakitTakipSistemi.Domain.Enums;

namespace AracGorevVeYakitTakipSistemi.Application.Interfaces;

public interface IBakimService
{
    Task<IReadOnlyList<BakimDto>> TumunuGetirAsync();
    Task<BakimDto?> GetirAsync(int bakimId);
    Task EkleAsync(BakimDto dto);
    Task GuncelleAsync(BakimDto dto);
    Task SilAsync(int bakimId);
    Task<IReadOnlyList<BakimDto>> YaklasanBakimlariGetirAsync();
}

public interface IHasarService
{
    Task<IReadOnlyList<HasarDto>> TumunuGetirAsync();
    Task<HasarDto?> GetirAsync(int hasarId);
    Task EkleAsync(HasarDto dto);
    Task GuncelleAsync(HasarDto dto);
    Task DurumGuncelleAsync(int hasarId, OnarimDurumu yeniDurum);
    Task SilAsync(int hasarId);
}
