using AracGorevVeYakitTakipSistemi.Domain.Entities;
using AracGorevVeYakitTakipSistemi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AracGorevVeYakitTakipSistemi.Infrastructure.Data;

public class UygulamaDbContext(DbContextOptions<UygulamaDbContext> options)
    : IdentityDbContext<Kullanici>(options)
{
    public DbSet<Arac> Araclar => Set<Arac>();
    public DbSet<Surucu> Suruculer => Set<Surucu>();
    public DbSet<Gorev> Gorevler => Set<Gorev>();
    public DbSet<YakitKaydi> YakitKayitlari => Set<YakitKaydi>();
    public DbSet<Bakim> Bakimlar => Set<Bakim>();
    public DbSet<Hasar> Hasarlar => Set<Hasar>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Kullanici>(entity =>
        {
            entity.ToTable("Kullanicilar");
            entity.Property(x => x.UserName).HasColumnName("KullaniciAdi").HasMaxLength(256);
            entity.Property(x => x.PasswordHash).HasColumnName("SifreHash");
        });
        modelBuilder.Entity<IdentityRole>().ToTable("Roller");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("KullaniciRolleri");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("KullaniciClaimleri");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("KullaniciGirisleri");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RolClaimleri");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("KullaniciTokenlari");

        modelBuilder.Entity<Arac>(entity =>
        {
            entity.ToTable("Araclar");
            entity.HasKey(x => x.AracId);
            entity.HasIndex(x => x.Plaka).IsUnique();
            entity.Property(x => x.Plaka).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Marka).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Model).HasMaxLength(100).IsRequired();
            entity.Property(x => x.YakitTuru).HasConversion<string>().HasMaxLength(20);
            entity.Property(x => x.RuhsatNo).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Durum).HasConversion<string>().HasMaxLength(20);
        });

        modelBuilder.Entity<Surucu>(entity =>
        {
            entity.ToTable("Suruculer");
            entity.HasKey(x => x.SurucuId);
            entity.Property(x => x.AdSoyad).HasMaxLength(150).IsRequired();
            entity.Property(x => x.EhliyetSinifi).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Telefon).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(256).IsRequired();
        });

        modelBuilder.Entity<Gorev>(entity =>
        {
            entity.ToTable("Gorevler");
            entity.HasKey(x => x.GorevId);
            entity.Property(x => x.GidilecekYer).HasMaxLength(200).IsRequired();
            entity.Property(x => x.GorevAmaci).HasMaxLength(500).IsRequired();
            entity.Property(x => x.Durum).HasConversion<string>().HasMaxLength(20);
            entity.HasOne(x => x.Arac).WithMany(x => x.Gorevler).HasForeignKey(x => x.AracId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Surucu).WithMany(x => x.Gorevler).HasForeignKey(x => x.SurucuId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<YakitKaydi>(entity =>
        {
            entity.ToTable("YakitKayitlari");
            entity.HasKey(x => x.YakitId);
            entity.Property(x => x.Litre).HasPrecision(18, 2);
            entity.Property(x => x.ToplamTutar).HasPrecision(18, 2);
            entity.Property(x => x.FisNo).HasMaxLength(50).IsRequired();
            entity.HasOne(x => x.Arac).WithMany(x => x.YakitKayitlari).HasForeignKey(x => x.AracId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Bakim>(entity =>
        {
            entity.ToTable("Bakimlar");
            entity.HasKey(x => x.BakimId);
            entity.Property(x => x.BakimTuru).HasConversion<string>().HasMaxLength(30);
            entity.Property(x => x.Aciklama).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.Maliyet).HasPrecision(18, 2);
            entity.HasOne(x => x.Arac).WithMany(x => x.Bakimlar).HasForeignKey(x => x.AracId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Hasar>(entity =>
        {
            entity.ToTable("Hasarlar");
            entity.HasKey(x => x.HasarId);
            entity.Property(x => x.Aciklama).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.FotografYolu).HasMaxLength(500).IsRequired();
            entity.Property(x => x.TahminiMaliyet).HasPrecision(18, 2);
            entity.Property(x => x.OnarimDurumu).HasConversion<string>().HasMaxLength(20);
            entity.HasOne(x => x.Arac).WithMany(x => x.Hasarlar).HasForeignKey(x => x.AracId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
