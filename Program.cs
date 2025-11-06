using System;
using System.Windows.Forms;
using SARAI.Modules;

namespace SARAI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Ayarlar & Lisans & DB
                _ = AppConfig.Current;           // AppData\SARAI\appconfig.json
                LicenseManager.Initialize();     // Demo sayaç
                DbConnection.EnsureDatabase();   // SQLite dosyası + tablolar

                if (!LicenseManager.IsLicensed)
                {
                    MessageBox.Show($"Demo sürüm: {LicenseManager.DemoDaysLeft} gün kaldı.",
                        "SARAI", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                Application.Run(new MainDashboard());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Başlatma hatası: " + ex.Message, "SARAI",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
