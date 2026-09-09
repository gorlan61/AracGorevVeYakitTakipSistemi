namespace AracGorevVeYakitTakipSistemi.Domain.Entities;

public class Surucu
{
    public int SurucuId { get; set; }
    public string AdSoyad { get; set; } = string.Empty;
    public string EhliyetSinifi { get; set; } = string.Empty;
    public string Telefon { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool AktifMi { get; set; } = true;
    public DateTime KayitTarihi { get; set; } = DateTime.UtcNow;

    public ICollection<Gorev> Gorevler { get; set; } = new List<Gorev>();
}
