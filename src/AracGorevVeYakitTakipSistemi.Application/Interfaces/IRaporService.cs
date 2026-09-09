using AracGorevVeYakitTakipSistemi.Application.DTOs;

namespace AracGorevVeYakitTakipSistemi.Application.Interfaces;

public interface IRaporService
{
    Task<RaporDataDto> RaporVerileriniGetirAsync();
}
