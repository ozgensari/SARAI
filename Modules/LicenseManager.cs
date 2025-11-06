using System;
using System.IO;

namespace SARAI.Modules
{
    public static class LicenseManager
    {
        private static string KeyFile => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SARAI", "license.dat");

        public static bool IsLicensed { get; private set; }
        public static int DemoDaysLeft { get; private set; }

        public static void Initialize()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(KeyFile)!);
            var cfgKey = AppConfig.Current.LicenseKey;

            if (!string.IsNullOrWhiteSpace(cfgKey) && cfgKey.StartsWith("SARAI-"))
            {
                IsLicensed = true;
                DemoDaysLeft = int.MaxValue;
                return;
            }

            var createDate = DateTime.Today;
            if (File.Exists(KeyFile))
            {
                var s = File.ReadAllText(KeyFile);
                if (DateTime.TryParse(s, out var d)) createDate = d;
            }
            else
            {
                File.WriteAllText(KeyFile, createDate.ToString("yyyy-MM-dd"));
            }

            var demoTotal = 30;
            var used = (DateTime.Today - createDate).Days;
            DemoDaysLeft = Math.Max(0, demoTotal - used);
            IsLicensed = DemoDaysLeft > 0;
        }
    }
}
