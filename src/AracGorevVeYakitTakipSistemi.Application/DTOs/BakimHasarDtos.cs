using System.ComponentModel.DataAnnotations;
using AracGorevVeYakitTakipSistemi.Domain.Enums;

namespace AracGorevVeYakitTakipSistemi.Application.DTOs;

public class BakimDto
{
    public int BakimId { get; set; }

    [Required(ErrorMessage = "Araç seçimi zorunludur.")]
    [Range(1, int.MaxValue, ErrorMessage = "Araç seçimi zorunludur.")]
    public int AracId { get; set; }

    [Required(ErrorMessage = "Bakım türü zorunludur.")]
    public BakimTuru BakimTuru { get; set; }

    [Required(ErrorMessage = "Yapılan tarih zorunludur.")]
    public DateTime YapilanTarih { get; set; } = DateTime.Today;

    public DateTime? SonrakiTarih { get; set; }

    [Required(ErrorMessage = "Açıklama zorunludur.")]
    [StringLength(1000)]
    public string Aciklama { get; set; } = string.Empty;

    [Required(ErrorMessage = "Maliyet zorunludur.")]
    [Range(0, 10000000, ErrorMessage = "Maliyet negatif olamaz.")]
    public decimal Maliyet { get; set; }

    // Görüntüleme için
    public string? AracPlaka { get; set; }
    public bool YaklastiMi { get; set; }
    public int? KalanGun { get; set; }
}

public class HasarDto
{
    public int HasarId { get; set; }

    [Required(ErrorMessage = "Araç seçimi zorunludur.")]
    [Range(1, int.MaxValue, ErrorMessage = "Araç seçimi zorunludur.")]
    public int AracId { get; set; }

    [Required(ErrorMessage = "Tarih zorunludur.")]
    public DateTime Tarih { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Açıklama zorunludur.")]
    [StringLength(1000)]
    public string Aciklama { get; set; } = string.Empty;

    public string? FotografYolu { get; set; }

    [Required(ErrorMessage = "Tahmini maliyet zorunludur.")]
    [Range(0, 10000000, ErrorMessage = "Tahmini maliyet negatif olamaz.")]
    public decimal TahminiMaliyet { get; set; }

    public OnarimDurumu OnarimDurumu { get; set; } = OnarimDurumu.Beklemede;

    // Görüntüleme için
    public string? AracPlaka { get; set; }
}
