using System.ComponentModel.DataAnnotations;
using AracGorevVeYakitTakipSistemi.Domain.Enums;

namespace AracGorevVeYakitTakipSistemi.Application.DTOs;

public class AracDto
{
    public int AracId { get; set; }

    [Required(ErrorMessage = "Plaka zorunludur.")]
    [StringLength(20)]
    public string Plaka { get; set; } = string.Empty;

    [Required(ErrorMessage = "Marka zorunludur.")]
    [StringLength(100)]
    public string Marka { get; set; } = string.Empty;

    [Required(ErrorMessage = "Model zorunludur.")]
    [StringLength(100)]
    public string Model { get; set; } = string.Empty;

    [Range(1900, 2100, ErrorMessage = "Geçerli bir model yılı girin.")]
    public int ModelYili { get; set; }

    [Required(ErrorMessage = "Yakıt türü zorunludur.")]
    public YakitTuru YakitTuru { get; set; }

    [Required(ErrorMessage = "Ruhsat numarası zorunludur.")]
    [StringLength(50)]
    public string RuhsatNo { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "Güncel kilometre negatif olamaz.")]
    public int GuncelKm { get; set; }

    [Required(ErrorMessage = "Durum zorunludur.")]
    public AracDurumu Durum { get; set; }

    public DateTime KayitTarihi { get; set; }
}
