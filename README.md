
# DynamicConfiguration

## 🧪 Örnek Kullanım

### Servis kaydı:

```csharp
services.AddSingleton<IConfigurationReader>(sp =>
{
    const string applicationName = "SERVICE-A";
    const int refreshIntervalMs = 100000;

    return new ConfigurationReader(
        applicationName,
        Configuration.GetConnectionString("MongoConnection"),
        refreshIntervalMs
    );
});
```

### Okuma işlemi:

```csharp
public class SampleService
{
    private readonly IConfigurationReader _config;

    public SampleService(IConfigurationReader config)
    {
        _config = config;
    }

    public void PrintSettings()
    {
        var siteName = _config.GetValue<string>("SiteName");
        var maxItems = _config.GetValue<int>("MaxItemCount");
        Console.WriteLine($"Site: {siteName}, MaxItems: {maxItems}");
    }
}
```

---

## 🚀 Kurulum

1. `docker-compose.yml` dosyasını kullanarak hızlıca başlatabilirsiniz:

```bash
docker-compose up -d
```

---

## 📂 Proje Yapısı

- **DynamicConfiguration.Library** → Kütüphane kodları
- **DynamicConfiguration.TestServiceA-API / B-API** → API kullanım örnekleri
- **DynamicConfiguration.Common** → Event sınıfları
- **DynamicConfigurationWeb** →  Konfigürasyonları yönetmek için Web uygulaması
