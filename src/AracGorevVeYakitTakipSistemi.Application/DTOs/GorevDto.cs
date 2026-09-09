using System.ComponentModel.DataAnnotations;
using AracGorevVeYakitTakipSistemi.Domain.Enums;

namespace AracGorevVeYakitTakipSistemi.Application.DTOs;

public class GorevDto
{
    public int GorevId { get; set; }

    [Required(ErrorMessage = "Araç seçimi zorunludur.")]
    [Range(1, int.MaxValue, ErrorMessage = "Araç seçimi zorunludur.")]
    public int AracId { get; set; }

    [Required(ErrorMessage = "Sürücü seçimi zorunludur.")]
    [Range(1, int.MaxValue, ErrorMessage = "Sürücü seçimi zorunludur.")]
    public int SurucuId { get; set; }

    [Required(ErrorMessage = "Görev tarihi zorunludur.")]
    public DateTime GorevTarihi { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Gidilecek yer zorunludur.")]
    [StringLength(200)]
    public string GidilecekYer { get; set; } = string.Empty;

    [Required(ErrorMessage = "Görev amacı zorunludur.")]
    [StringLength(500)]
    public string GorevAmaci { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "Başlangıç km negatif olamaz.")]
    public int? BaslangicKm { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Bitiş km negatif olamaz.")]
    public int? BitisKm { get; set; }

    public int? ToplamMesafe { get; set; }

    public GorevDurumu Durum { get; set; } = GorevDurumu.Planlandi;

    // Görüntülemek için
    public string? AracPlaka { get; set; }
    public string? SurucuAdSoyad { get; set; }
}
