namespace AracGorevVeYakitTakipSistemi.Domain.Entities;

public class YakitKaydi
{
    public int YakitId { get; set; }
    public int AracId { get; set; }
    public DateTime Tarih { get; set; }
    public decimal Litre { get; set; }
    public decimal ToplamTutar { get; set; }
    public string FisNo { get; set; } = string.Empty;
    public int Km { get; set; }

    public Arac Arac { get; set; } = null!;
}
