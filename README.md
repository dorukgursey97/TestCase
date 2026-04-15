# AppTemplate Backend

Bu proje, .NET 8.0 kullanılarak geliştirilmiş, Clean Architecture yapısına sahip ve Dockerize edilmiş bir backend uygulamasıdır. İçerisinde API servisinin yanı sıra yapılandırılmış bir SQL Server veritabanı barındırır.

## 🚀 Başlangıç

Projeyi yerel ortamınızda hızlıca çalıştırmak için aşağıdaki adımları izleyebilirsiniz.

### 📋 Ön Gereksinimler

Sisteminizde şunların yüklü olması gerekir:
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)
* [Git](https://git-scm.com/)

### 🛠️ Kurulum ve Çalıştırma

Aşağıdaki komutları sırasıyla terminalinizde çalıştırın:

```bash
# 1. Projeyi Klonlayın
git clone [https://github.com/kullanici-adin/app-template.git](https://github.com/kullanici-adin/app-template.git)
cd app-template

# 2. Docker Compose ile Ayağa Kaldırın
docker-compose up -d

# 3. Konteyner Durumunu Kontrol Edin
docker-compose ps

Servis,Adres,Bilgi
API Endpoint,http://localhost:5000,Backend ana adresi
Swagger UI,http://localhost:5000/swagger,API Dokümantasyonu
SQL Server,"localhost,1433",User: sa / Pass: YourPassword123!

⚙️ Çevresel Değişkenler (Environment)
Proje, docker-compose.yml dosyasında tanımlı şu varsayılanlarla çalışmaktadır:

Environment: Development

JWT Key: TodoApp-SuperSecretKey-MustBe32CharsMin!!

Database: TodoDb

🛠️ Temel Komutlar
Servisleri Durdurma: docker-compose down

Logları Takip Etme: docker-compose logs -f app

Yeniden Başlatma: docker-compose down -v && docker-compose up -d
