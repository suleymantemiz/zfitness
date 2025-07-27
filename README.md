# ZFitness - Fitness Salonu Yönetim Sistemi

## 📋 Proje Hakkında

ZFitness, fitness salonları için geliştirilmiş kapsamlı bir yönetim sistemidir. Bu sistem, üye kartlarının cihazlara gönderilmesi, silinmesi, cihazlardan veri alınması ve gerçek zamanlı izleme işlemlerini otomatik olarak gerçekleştirir.

## 🚀 Özellikler

### 🔐 Üye Yönetimi

- **Kart Gönderimi**: Üye kartlarının cihazlara otomatik gönderimi
- **Kart Silme**: Üye kartlarının cihazlardan otomatik silinmesi
- **Limit Yönetimi**: Üye kredi limitlerinin cihazlara gönderilmesi
- **Son Kullanma Tarihi**: Üye kartlarının son kullanma tarihlerinin yönetimi

### 📊 Veri İşleme

- **Gerçek Zamanlı Veri Alma**: Cihazlardan UDP ile gerçek zamanlı veri alma
- **Hareket Kayıtları**: Üye giriş/çıkış hareketlerinin kaydedilmesi
- **Log Yönetimi**: Detaylı log dosyaları ile işlem takibi
- **Hata Yönetimi**: Kapsamlı hata yakalama ve raporlama

### 🔧 Teknik Özellikler

- **TCP/UDP İletişimi**: Cihazlarla güvenli iletişim
- **MySQL Veritabanı**: Merkezi veri yönetimi
- **Çoklu Cihaz Desteği**: Birden fazla cihazın aynı anda yönetimi
- **Otomatik Yeniden Deneme**: Bağlantı kopması durumunda otomatik yeniden deneme

## 🛠️ Sistem Gereksinimleri

### Yazılım Gereksinimleri

- **.NET Framework 4.7.2** veya üzeri
- **MySQL Server 5.7** veya üzeri
- **Windows 10/11** (64-bit)

### Donanım Gereksinimleri

- **RAM**: Minimum 4GB, Önerilen 8GB
- **İşlemci**: Intel i3 veya AMD eşdeğeri
- **Depolama**: Minimum 10GB boş alan
- **Ağ**: Ethernet bağlantısı (WiFi desteklenir)

## 📦 Kurulum

### 1. Veritabanı Kurulumu

```sql
-- MySQL veritabanını oluşturun
CREATE DATABASE zfitness_fitness_db;

-- Gerekli tabloları oluşturun
-- (Tablolar otomatik olarak oluşturulacaktır)
```

### 2. Uygulama Kurulumu

1. **ZIP dosyasını indirin** ve uygun klasöre çıkartın
2. **App.config** dosyasını düzenleyin:

   ```xml
   <connectionStrings>
     <add name="ZFitnessDB"
          connectionString="Database=zfitness_fitness_db;
                           Data Source=localhost;
                           Port=3306;
                           User ID=root;
                           password=your_password;
                           Connection Timeout=30;
                           Command Timeout=30;" />
   </connectionStrings>
   ```

3. **Uygulamayı çalıştırın**:
   ```bash
   ZFitness.exe
   ```

## 🔧 Konfigürasyon

### Veritabanı Ayarları

- **Host**: Veritabanı sunucusu IP adresi
- **Port**: MySQL port (varsayılan: 3306)
- **Database**: Veritabanı adı
- **Username**: Veritabanı kullanıcı adı
- **Password**: Veritabanı şifresi

### Cihaz Ayarları

- **TCP Port**: Cihaz iletişim portu (varsayılan: 9780)
- **UDP Port**: Gerçek zamanlı veri alma portu (varsayılan: 9781)
- **Timeout**: Bağlantı zaman aşımı süresi (varsayılan: 10 saniye)

### Log Ayarları

- **Log Seviyesi**: INFO, WARNING, ERROR
- **Log Dosyası**: Günlük log dosyaları
- **Log Klasörü**: `bin/Debug/log/`

## 📊 Kullanım

### Program Başlatma

1. **ZFitness.exe** dosyasını çalıştırın
2. Program otomatik olarak başlayacak ve "ZFitness Salonuna Hoş Geldiniz!" mesajı görünecek
3. Konsol ekranında işlem durumları takip edilebilir

### Temel İşlemler

- **Kişi Gönderimi**: Veritabanından üye bilgileri alınır ve cihazlara gönderilir
- **Kişi Silme**: Silinecek üye kartları cihazlardan kaldırılır
- **Veri Alma**: Cihazlardan hareket verileri alınır ve veritabanına kaydedilir
- **Log Temizleme**: İşlenen veriler cihazlardan temizlenir

### Konsol Çıktıları

```
[INFO] KART GÖNDERİLDİ:
[INFO] KİŞİ ADI: John Doe
[INFO] KART NO: 12345678
[INFO] LİMİT: 100
[INFO] SON TARİH: 2025-12-31
[INFO] GRUP: UYE
[INFO] ----------------------------------------
```

## 🗂️ Dosya Yapısı

```
ZFitness/
├── Program.cs              # Ana program dosyası
├── mika.cs                 # Cihaz iletişim sınıfı
├── App.config              # Konfigürasyon dosyası
├── ZFitness.csproj         # Proje dosyası
├── bin/Debug/
│   ├── ZFitness.exe        # Çalıştırılabilir dosya
│   ├── log/                # Log dosyaları klasörü
│   │   ├── 20250727.txt    # Günlük log dosyaları
│   │   └── loglar.txt      # Genel log dosyası
│   └── MySql.Data.dll      # MySQL bağlantı kütüphanesi
└── README.md               # Bu dosya
```

## 🔍 Veritabanı Tabloları

### Ana Tablolar

- **clients**: Üye bilgileri
- **cards**: Kart bilgileri
- **device**: Cihaz bilgileri
- **device_records**: Hareket kayıtları
- **device_add_card**: Gönderilecek kartlar
- **device_del_card**: Silinecek kartlar

## 🚨 Hata Yönetimi

### Yaygın Hatalar ve Çözümler

#### Bağlantı Hatası

```
[ERROR] VERİTABANI BAĞLANTI HATASI: Connection refused
```

**Çözüm**: MySQL servisinin çalıştığından emin olun

#### Cihaz Bağlantı Hatası

```
[ERROR] BAĞLANTI YOKTUR
```

**Çözüm**: Cihaz IP adresini ve port numarasını kontrol edin

#### Veri Gönderme Hatası

```
[ERROR] KİŞİ CİHAZA GÖNDERİLEMEDİ
```

**Çözüm**: Kart numarası ve üye ID'sinin doğru olduğunu kontrol edin

## 📈 Performans Optimizasyonu

### Öneriler

- **Düzenli Log Temizleme**: Eski log dosyalarını periyodik olarak temizleyin
- **Veritabanı İndeksleme**: Sık kullanılan sorgular için indeks oluşturun
- **Ağ Optimizasyonu**: Cihazlarla aynı ağ segmentinde çalıştırın
- **Bellek Yönetimi**: Büyük veri setleri için bellek ayarlarını optimize edin

## 🔒 Güvenlik

### Güvenlik Önlemleri

- **Şifreli Bağlantı**: MySQL SSL bağlantısı kullanın
- **Firewall**: Gerekli portları açın (9780, 9781)
- **Kullanıcı Yetkileri**: Veritabanı kullanıcısına minimum yetki verin
- **Log Güvenliği**: Hassas bilgileri log dosyalarından çıkarın

## 📞 Destek

### Teknik Destek

- **E-posta**: supermansuleyman@gmail.com
- **Telefon**: +994 55 603 5908
- **Çalışma Saatleri**: Pazartesi - Cuma, 09:00 - 18:00

### Dokümantasyon

- **Kullanım Kılavuzu**: [Link]
- **API Dokümantasyonu**: [Link]
- **Video Eğitimler**: [Link]

## 🔄 Güncellemeler

### Versiyon Geçmişi

- **v4.70** (2025-07-27): Etiketli log sistemi, hata düzeltmeleri
- **v4.60** (2025-07-26): Performans iyileştirmeleri
- **v4.50** (2025-07-25): Yeni özellikler eklendi

## 📄 Lisans

Bu proje özel lisans altında geliştirilmiştir. Tüm hakları saklıdır.

## 👥 Geliştirici

**TozCRM Development Team**

- **Proje Yöneticisi**: [Süleyman]
- **Baş Geliştirici**: [Süleyman]
- **Test Uzmanı**: [İsim]

---

**© 2025 ZFitness. Tüm hakları saklıdır.**
