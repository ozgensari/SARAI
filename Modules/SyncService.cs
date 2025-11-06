using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using Microsoft.Data.Sqlite;

namespace SARAI.Modules
{
    public static class SyncService
    {
        public static string ExportJson()
        {
            var dto = new
            {
                CreatedAt = DateTime.Now,
                Products = Query("SELECT Id,Barcode,ProductName,UnitPrice,Stock FROM Products"),
                Customers = Query("SELECT Id,FullName,Phone FROM Customers"),
                Sales = Query("SELECT Id,SaleDate,ProductId,Quantity,UnitPrice FROM Sales"),
                Invoices = Query("SELECT Id,CustomerId,Status,Amount,CreatedAt FROM Invoices"),
            };

            var json = new JavaScriptSerializer().Serialize(dto);
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SARAI_Backups");
            Directory.CreateDirectory(folder);
            var path = Path.Combine(folder, $"backup_{DateTime.Now:yyyyMMdd_HHmm}.json");
            File.WriteAllText(path, json, Encoding.UTF8);
            return path;
        }

        private static DataTable Query(string sql)
        {
            using var conn = DbConnection.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            using var da = new SqliteDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
}
