namespace SARAI.Modules
{
    public static class AiEngine
    {
        public static string BuildDashboardHint(decimal weeklyTotal, int criticalStock, int pendingInvoice)
        {
            if (criticalStock > 5) return "⚠ Kritik stok sayısı yüksek. Depo ikmali planlayın.";
            if (pendingInvoice > 0 && weeklyTotal > 0) return "💡 Tahsilat hatırlat: Bekleyen faturalar var.";
            if (weeklyTotal <= 0) return "ℹ Henüz satış yok. Ürün girişlerini kontrol edin.";
            return "✅ Satışlar stabil görünüyor. Fırsatlar için raporları gözden geçirin.";
        }
    }
}
