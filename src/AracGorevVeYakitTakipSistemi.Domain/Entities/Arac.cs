using AracGorevVeYakitTakipSistemi.Domain.Enums;

namespace AracGorevVeYakitTakipSistemi.Domain.Entities;

public class Arac
{
    public int AracId { get; set; }
    public string Plaka { get; set; } = string.Empty;
    public string Marka { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int ModelYili { get; set; }
    public YakitTuru YakitTuru { get; set; }
    public string RuhsatNo { get; set; } = string.Empty;
    public int GuncelKm { get; set; }
    public AracDurumu Durum { get; set; } = AracDurumu.Aktif;
    public DateTime KayitTarihi { get; set; } = DateTime.UtcNow;

    public ICollection<Gorev> Gorevler { get; set; } = new List<Gorev>();
    public ICollection<YakitKaydi> YakitKayitlari { get; set; } = new List<YakitKaydi>();
    public ICollection<Bakim> Bakimlar { get; set; } = new List<Bakim>();
    public ICollection<Hasar> Hasarlar { get; set; } = new List<Hasar>();
}
