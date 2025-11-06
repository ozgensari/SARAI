using System;
using System.Data;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Data.Sqlite;

namespace SARAI.Modules
{
    public static class ReportsModule
    {
        public static string CreateWeeklySalesReportPdf(string? outFolder = null)
        {
            outFolder ??= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SARAI_Reports");
            Directory.CreateDirectory(outFolder);
            var file = Path.Combine(outFolder, $"HaftalikSatis_{DateTime.Now:yyyyMMdd_HHmm}.pdf");

            using var fs = new FileStream(file, FileMode.Create, FileAccess.Write);
            using var doc = new Document(PageSize.A4, 36, 36, 48, 36);
            var writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            var title = new Paragraph("SARAI - Haftalık Satış Raporu", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14))
            { Alignment = Element.ALIGN_CENTER, SpacingAfter = 12f };
            doc.Add(title);

            doc.Add(new Paragraph(DateTime.Now.ToLongDateString(), FontFactory.GetFont(FontFactory.HELVETICA, 9))
            { SpacingAfter = 8f });

            var dt = FetchWeeklySales();

            var table = new PdfPTable(4) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 22, 42, 18, 18 });
            AddHeader(table, "Tarih", "Ürün", "Adet", "Tutar");

            foreach (DataRow r in dt.Rows)
            {
                table.AddCell(Cell(r["SaleDate"]));
                table.AddCell(Cell(r["ProductName"]));
                table.AddCell(Cell(r["Quantity"], Element.ALIGN_RIGHT));
                table.AddCell(Cell(r["TotalAmount"], Element.ALIGN_RIGHT));
            }
            doc.Add(table);

            var total = DbConnection.GetTotalSales();
            doc.Add(new Paragraph($"\nGenel Toplam: {total:C2}", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)));

            doc.Close();
            writer.Close();
            return file;
        }

        private static void AddHeader(PdfPTable t, params string[] headers)
        {
            foreach (var h in headers)
            {
                var cell = new PdfPCell(new Phrase(h, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9)))
                { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 6, BackgroundColor = new BaseColor(240, 240, 240) };
                t.AddCell(cell);
            }
        }

        private static PdfPCell Cell(object value, int align = Element.ALIGN_LEFT)
        {
            string s = value is IFormattable f ? f.ToString(null, System.Globalization.CultureInfo.CurrentCulture) : $"{value}";
            var cell = new PdfPCell(new Phrase(s, FontFactory.GetFont(FontFactory.HELVETICA, 9)))
            { HorizontalAlignment = align, Padding = 5 };
            return cell;
        }

        private static DataTable FetchWeeklySales()
        {
            using var conn = DbConnection.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT 
  substr(SaleDate,1,10) AS SaleDate,
  P.ProductName,
  S.Quantity,
  (S.Quantity*S.UnitPrice) AS TotalAmount
FROM Sales S
JOIN Products P ON P.Id = S.ProductId
WHERE datetime(SaleDate) >= datetime('now','-7 days')
ORDER BY SaleDate DESC";
            using var da = new SqliteDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
}
