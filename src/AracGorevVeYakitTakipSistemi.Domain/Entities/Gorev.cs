using AracGorevVeYakitTakipSistemi.Domain.Enums;

namespace AracGorevVeYakitTakipSistemi.Domain.Entities;

public class Gorev
{
    public int GorevId { get; set; }
    public int AracId { get; set; }
    public int SurucuId { get; set; }
    public DateTime GorevTarihi { get; set; }
    public string GidilecekYer { get; set; } = string.Empty;
    public string GorevAmaci { get; set; } = string.Empty;
    public int? BaslangicKm { get; set; }
    public int? BitisKm { get; set; }
    public int? ToplamMesafe { get; set; }
    public GorevDurumu Durum { get; set; } = GorevDurumu.Planlandi;

    public Arac Arac { get; set; } = null!;
    public Surucu Surucu { get; set; } = null!;
}
