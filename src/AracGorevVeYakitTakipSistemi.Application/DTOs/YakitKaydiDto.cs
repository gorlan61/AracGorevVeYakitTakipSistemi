using System.ComponentModel.DataAnnotations;

namespace AracGorevVeYakitTakipSistemi.Application.DTOs;

public class YakitKaydiDto
{
    public int YakitId { get; set; }

    [Required(ErrorMessage = "Araç seçimi zorunludur.")]
    [Range(1, int.MaxValue, ErrorMessage = "Araç seçimi zorunludur.")]
    public int AracId { get; set; }

    [Required(ErrorMessage = "Tarih zorunludur.")]
    public DateTime Tarih { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Litre zorunludur.")]
    [Range(0.01, 10000, ErrorMessage = "Litre 0'dan büyük olmalıdır.")]
    public decimal Litre { get; set; }

    [Required(ErrorMessage = "Toplam tutar zorunludur.")]
    [Range(0.01, 1000000, ErrorMessage = "Toplam tutar 0'dan büyük olmalıdır.")]
    public decimal ToplamTutar { get; set; }

    [Required(ErrorMessage = "Fiş no zorunludur.")]
    [StringLength(50)]
    public string FisNo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kilometre zorunludur.")]
    [Range(0, int.MaxValue, ErrorMessage = "Kilometre negatif olamaz.")]
    public int Km { get; set; }

    // Görüntüleme için
    public string? AracPlaka { get; set; }
    public decimal BirimFiyat => Litre > 0 ? Math.Round(ToplamTutar / Litre, 2) : 0;
}

public class YakitTuketimAnaliziDto
{
    public int AracId { get; set; }
    public string Plaka { get; set; } = string.Empty;
    public string MarkaModel { get; set; } = string.Empty;
    public decimal ToplamLitre { get; set; }
    public decimal ToplamTutar { get; set; }
    public int EnKucukKm { get; set; }
    public int EnBuyukKm { get; set; }
    public int ToplamMesafe => EnBuyukKm > EnKucukKm ? EnBuyukKm - EnKucukKm : 0;
    
    // 100 km'deki ortalama litre tüketimi
    public decimal OrtalamaTuketim100Km => ToplamMesafe > 0 ? Math.Round((ToplamLitre / ToplamMesafe) * 100, 2) : 0;

    // Km başına ortalama maliyet (TL)
    public decimal KmBasiMaliyet => ToplamMesafe > 0 ? Math.Round(ToplamTutar / ToplamMesafe, 2) : 0;
}
