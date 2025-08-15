Gereksinimler

* **SQL Server** (Express veya Developer sürümü)
* **.NET 8 SDK**
* **Entity Framework Core Tools**

#### EF Core Tools Kurulumu

**Global Kurulum:**

```bash
dotnet tool install --global dotnet-ef
```

**Proje Klasörüne Özel Kurulum:**

```bash
dotnet tool install --local dotnet-ef
```

---


1. **Proje dizinine geçin:**

   ```bash
   cd EtkinlikYonetim
   ```

2. **`appsettings.json` içinde bağlantı ayarını yapın:**

   * `DefaultConnection` değerini kendi SQL Server bağlantınıza göre düzenleyin.

3. **Veritabanını Migration ile oluşturun/güncelleyin:**

   ```bash
   dotnet ef database update
   ```

   > Bu adım veritabanını, migration dosyalarına göre otomatik olarak oluşturur.

4. **Uygulamayı başlatın:**

   ```bash
   dotnet run
   ```

   veya otomatik yenileme için:

   ```bash
   dotnet watch
   ```


