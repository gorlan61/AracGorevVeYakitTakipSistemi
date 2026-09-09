using AracGorevVeYakitTakipSistemi.Application.DTOs;

namespace AracGorevVeYakitTakipSistemi.Application.Interfaces;

public interface IGorevService
{
    Task<IReadOnlyList<GorevDto>> TumunuGetirAsync();
    Task<GorevDto?> GetirAsync(int gorevId);
    Task EkleAsync(GorevDto dto);
    Task<(bool AnomaliVar, int ToplamMesafe)> KmGuncelleAsync(int gorevId, int baslangicKm, int bitisKm);
    Task DurumGuncelleAsync(int gorevId, Domain.Enums.GorevDurumu yeniDurum);
    Task SilAsync(int gorevId);
}
