using AracGorevVeYakitTakipSistemi.Domain.Enums;

namespace AracGorevVeYakitTakipSistemi.Domain.Entities;

public class Hasar
{
    public int HasarId { get; set; }
    public int AracId { get; set; }
    public DateTime Tarih { get; set; }
    public string Aciklama { get; set; } = string.Empty;
    public string FotografYolu { get; set; } = string.Empty;
    public decimal TahminiMaliyet { get; set; }
    public OnarimDurumu OnarimDurumu { get; set; } = OnarimDurumu.Beklemede;

    public Arac Arac { get; set; } = null!;
}
