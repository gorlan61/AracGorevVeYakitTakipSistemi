using System.ComponentModel.DataAnnotations;

namespace AracGorevVeYakitTakipSistemi.Application.DTOs;

public class SurucuDto
{
    public int SurucuId { get; set; }

    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    [StringLength(150)]
    public string AdSoyad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ehliyet sınıfı zorunludur.")]
    [StringLength(50)]
    public string EhliyetSinifi { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
    [StringLength(30)]
    public string Telefon { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    public bool AktifMi { get; set; } = true;
    public DateTime KayitTarihi { get; set; }
}
