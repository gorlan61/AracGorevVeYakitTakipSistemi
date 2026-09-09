using AracGorevVeYakitTakipSistemi.Application.DTOs;

namespace AracGorevVeYakitTakipSistemi.Web.Models;

public class DashboardViewModel
{
    public int ToplamArac { get; set; }
    public int AktifArac { get; set; }
    public int ToplamSurucu { get; set; }
    public int AktifSurucu { get; set; }
    public int PlanlananGorev { get; set; }
    public int DevamEdenGorev { get; set; }
    public decimal BuAyYakitTutar { get; set; }

    public IReadOnlyList<GorevDto> SonGorevler { get; set; } = new List<GorevDto>();
    public IReadOnlyList<BakimDto> YaklasanBakimlar { get; set; } = new List<BakimDto>();
}
