using AracGorevVeYakitTakipSistemi.Domain.Enums;

namespace AracGorevVeYakitTakipSistemi.Domain.Entities;

public class Bakim
{
    public int BakimId { get; set; }
    public int AracId { get; set; }
    public BakimTuru BakimTuru { get; set; }
    public DateTime YapilanTarih { get; set; }
    public DateTime? SonrakiTarih { get; set; }
    public string Aciklama { get; set; } = string.Empty;
    public decimal Maliyet { get; set; }

    public Arac Arac { get; set; } = null!;
}
