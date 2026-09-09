namespace AracGorevVeYakitTakipSistemi.Application.DTOs;

public class AracMaliyetOzetDto
{
    public int AracId { get; set; }
    public string Plaka { get; set; } = string.Empty;
    public string MarkaModel { get; set; } = string.Empty;
    public int GorevSayisi { get; set; }
    public int ToplamKm { get; set; }
    public decimal YakitMaliyeti { get; set; }
    public decimal BakimMaliyeti { get; set; }
    public decimal HasarMaliyeti { get; set; }
    public decimal ToplamMaliyet => YakitMaliyeti + BakimMaliyeti + HasarMaliyeti;
}

public class AylikYakitOzetDto
{
    public int Yil { get; set; }
    public int Ay { get; set; }
    public string AyAdi => new DateTime(Yil, Ay, 1).ToString("MMMM yyyy", new System.Globalization.CultureInfo("tr-TR"));
    public decimal ToplamLitre { get; set; }
    public decimal ToplamTutar { get; set; }
}

public class RaporDataDto
{
    public decimal FiloToplamMaliyet { get; set; }
    public decimal FiloToplamYakitMaliyeti { get; set; }
    public decimal FiloToplamBakimMaliyeti { get; set; }
    public decimal FiloToplamHasarMaliyeti { get; set; }

    public IReadOnlyList<AracMaliyetOzetDto> AracMaliyetleri { get; set; } = new List<AracMaliyetOzetDto>();
    public IReadOnlyList<AracMaliyetOzetDto> EnCokKullanilanAraclar { get; set; } = new List<AracMaliyetOzetDto>();
    public IReadOnlyList<AracMaliyetOzetDto> EnMaliyetliAraclar { get; set; } = new List<AracMaliyetOzetDto>();
    public IReadOnlyList<AylikYakitOzetDto> AylikYakitTuketimi { get; set; } = new List<AylikYakitOzetDto>();
    public IReadOnlyList<BakimDto> YaklasanBakimlar { get; set; } = new List<BakimDto>();
}
