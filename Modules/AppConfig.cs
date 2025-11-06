using System;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;

namespace SARAI.Modules
{
    public sealed class AppConfig
    {
        private static readonly Lazy<AppConfig> _lazy = new(() => new AppConfig());
        public static AppConfig Current => _lazy.Value;

        private AppConfig() { Load(); }

        public string BrandName { get; set; } = "SARAI";
        public string ConnectionString { get; set; } = ""; // SQLite path otomatik dolacak
        public bool EnablePayGo { get; set; } = false;
        public bool EnableProPay { get; set; } = false;
        public string LicenseKey { get; set; } = "";

        private string JsonPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SARAI", "appconfig.json");

        private void Load()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(JsonPath)!);
                if (File.Exists(JsonPath))
                {
                    var json = File.ReadAllText(JsonPath, Encoding.UTF8);
                    var dto = new JavaScriptSerializer().Deserialize<AppConfig>(json);
                    if (dto != null)
                    {
                        BrandName = dto.BrandName ?? BrandName;
                        ConnectionString = dto.ConnectionString ?? "";
                        EnablePayGo = dto.EnablePayGo;
                        EnableProPay = dto.EnableProPay;
                        LicenseKey = dto.LicenseKey ?? "";
                    }
                }
            }
            catch { }

            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                var db = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SARAI.db");
                ConnectionString = $"Data Source={db};Cache=Shared";
                Save();
            }
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(JsonPath)!);
                var json = new JavaScriptSerializer().Serialize(this);
                File.WriteAllText(JsonPath, json, Encoding.UTF8);
            }
            catch { }
        }
    }
}
