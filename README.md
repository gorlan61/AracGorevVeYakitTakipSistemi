# Araç, Görev ve Yakıt Takip Sistemi

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?logo=dotnet&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/Entity%20Framework%20Core-8.0-512BD4?logo=dotnet&logoColor=white)
![C%23](https://img.shields.io/badge/C%23-.NET%208-239120?logo=csharp&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?logo=bootstrap&logoColor=white)
![GitHub Durumu](https://img.shields.io/badge/GitHub-Repository%20Ready-181717?logo=github&logoColor=white)
![Lisans](https://img.shields.io/badge/Lisans-Belirtilmemi%C5%9F-lightgrey)

> *Bu proje, **Trabzon Büyükşehir Belediyesi Bilgi İşlem Dairesi Başkanlığı Yazılım Şube Müdürlüğü** bünyesinde gerçekleştirilen staj döneminde geliştirilmiş web tabanlı bir Araç, Görev ve Yakıt Takip Yönetim Sistemidir.*

Kurum filosundaki araçların operasyonel süreçlerini tek merkezden yönetmek amacıyla geliştirilen bu uygulama; araç, sürücü, görev, yakıt, bakım ve hasar verilerini bütünleşik biçimde izlemeyi sağlar. Sistem, yöneticilerin maliyetleri görünür kılmasına, görev süreçlerini kontrol etmesine ve planlı bakım kararlarını veriye dayalı olarak almasına yardımcı olur.

## Proje Hakkında

Filo operasyonlarında farklı kaynaklarda tutulan kayıtlar, araç kullanımının, yakıt giderlerinin ve bakım yükümlülüklerinin izlenmesini zorlaştırır. Araç, Görev ve Yakıt Takip Sistemi bu ihtiyaca cevap olarak operasyonel kayıtları merkezi bir yapıda toplar.

Uygulama, yetkilendirilmiş kullanıcıların araç ve sürücü envanterini yönetmesine; araçlara görev atamasına; yakıt, bakım ve hasar hareketlerini kaydetmesine; görev kilometrelerini takip etmesine ve maliyet/performance raporlarına erişmesine imkân tanır.

## Öne Çıkan Özellikler

- **Araç filo yönetimi:** Plaka, marka-model, ruhsat, yakıt türü, güncel kilometre ve araç durumu takibi.
- **Sürücü yönetimi:** Sürücü iletişim/ehliyet bilgilerinin ve aktiflik durumunun yönetimi.
- **Görev planlama ve atama:** Aktif araç ve sürücü seçimiyle görev oluşturma, görev durumunu güncelleme ve görev ayrıntılarını görüntüleme.
- **Kilometre kontrolü:** Başlangıç/bitiş kilometresi kaydı, kat edilen mesafenin otomatik hesaplanması ve olağan dışı mesafe uyarısı.
- **Yakıt kayıtları:** Fiş numarası, litre, tutar ve kilometre bilgileriyle yakıt tüketimi ile maliyetin izlenmesi.
- **Bakım ve hasar yönetimi:** Bakım türü, maliyet, sonraki bakım tarihi, hasar açıklaması ve onarım durumu kaydı.
- **Raporlama ve analiz:** Filo toplam maliyeti, yakıt/bakım/hasar kırılımı, aylık yakıt grafiği, en çok kullanılan ve en maliyetli araçlar.
- **Yaklaşan bakım bildirimi:** Tanımlı tarih aralığındaki bakım ihtiyaçlarının rapor ekranında gösterilmesi.
- **Kimlik doğrulama ve rol yapısı:** ASP.NET Core Identity ile kullanıcı girişi, rol desteği ve Türkçe doğrulama hata mesajları.

## Kullanılan Teknolojiler

| Alan | Teknolojiler |
| --- | --- |
| Backend | ASP.NET Core MVC 8, C#, Razor Pages |
| Veri erişimi | Entity Framework Core 8, Code First, LINQ |
| Veritabanı | Microsoft SQL Server / LocalDB |
| Kimlik ve yetki | ASP.NET Core Identity, roller |
| Arayüz | Razor Views, Bootstrap 5, HTML5, CSS3, JavaScript |
| Görselleştirme | Chart.js |
| Mimari | Katmanlı mimari, servis arayüzleri, DTO yaklaşımı |

## Veritabanı Yapısı ve Mimari

Çözüm, sorumlulukları ayrıştıran dört katmandan oluşur:

```text
src/
├── AracGorevVeYakitTakipSistemi.Domain/          # Varlıklar ve enum tanımları
├── AracGorevVeYakitTakipSistemi.Application/     # DTO'lar ve servis sözleşmeleri
├── AracGorevVeYakitTakipSistemi.Infrastructure/  # EF Core, Identity, migration ve servis uygulamaları
└── AracGorevVeYakitTakipSistemi.Web/             # MVC denetleyicileri, Razor görünümleri ve statik içerikler
```

Temel ilişki modeli aşağıdaki gibidir:

```text
Sürücü (1) ───< Görev >─── (1) Araç
                               ├───< Yakıt Kaydı
                               ├───< Bakım
                               └───< Hasar
```

- Her **görev**, bir araç ve bir sürücü ile ilişkilidir.
- Her **araç** için birden çok yakıt, bakım ve hasar kaydı tutulabilir.
- Araç plakası benzersizdir; ilişkisel kayıtlarda silme işlemleri kısıtlanarak veri bütünlüğü korunur.
- ASP.NET Core Identity tabloları, uygulama kullanıcıları ve rollerini yönetir.

## Kurulum ve Çalıştırma

### Ön koşullar

- .NET SDK 8.0 veya üzeri
- SQL Server ya da SQL Server LocalDB
- Entity Framework Core komut satırı aracı (gerekirse): `dotnet tool install --global dotnet-ef`

### Adımlar

1. Depoyu klonlayın ve çözüm klasörüne geçin.

   ```bash
   git clone <repo-url>
   cd <repo-klasörü>
   ```

2. Bağımlılıkları geri yükleyin.

   ```bash
   dotnet restore
   ```

3. Veritabanı bağlantısını yapılandırın.

   `src/AracGorevVeYakitTakipSistemi.Web/appsettings.Development.json` dosyasını yerel ortamınızda oluşturun ve `VarsayilanBaglanti` adlı bağlantı dizesini SQL Server kurulumunuza göre tanımlayın. Bu dosya sürüm kontrolü dışında tutulur.

   Örnek:

   ```json
   {
     "ConnectionStrings": {
       "VarsayilanBaglanti": "Server=(localdb)\\MSSQLLocalDB;Database=AracGorevVeYakitTakipSistemiDb;Trusted_Connection=True;TrustServerCertificate=True"
     }
   }
   ```

4. Migration'ları uygulayın.

   ```bash
   dotnet ef database update --project src/AracGorevVeYakitTakipSistemi.Infrastructure --startup-project src/AracGorevVeYakitTakipSistemi.Web
   ```

5. Uygulamayı çalıştırın.

   ```bash
   dotnet run --project src/AracGorevVeYakitTakipSistemi.Web
   ```

Uygulama başlangıcında örnek veri tohumlama işlemi çalışır. Tarayıcıda terminalde belirtilen HTTPS adresini açın.

## İletişim ve Katkıda Bulunanlar

- **Geliştiren:** Tolga Gürsoy
- **Kurum:** Trabzon Büyükşehir Belediyesi Bilgi İşlem Dairesi Başkanlığı, Yazılım Şube Müdürlüğü

Projenin geliştirilme sürecine sağladıkları kurum içi destek ve rehberlik için Trabzon Büyükşehir Belediyesi Yazılım Şube Müdürlüğüne teşekkür edilir.

## Lisans

Bu depo için henüz bir lisans tanımlanmamıştır. Yeniden kullanım, dağıtım veya açık kaynak lisanslama kararları proje sahibinin onayına tabidir.
