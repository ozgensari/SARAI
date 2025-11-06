using System;
using System.Data;
using Microsoft.Data.Sqlite;
using System.IO;

namespace SARAI.Modules
{
    public static class DbConnection
    {
        public static string ConnectionString => AppConfig.Current.ConnectionString;

        public static SqliteConnection Open()
        {
            var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        public static void EnsureDatabase()
        {
            var cs = new SqliteConnectionStringBuilder(ConnectionString);
            var dbPath = cs.DataSource;
            var dir = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

            using var conn = Open();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS Products(
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  Barcode TEXT UNIQUE,
  ProductName TEXT NOT NULL,
  UnitPrice REAL NOT NULL DEFAULT 0,
  Stock INTEGER NOT NULL DEFAULT 0,
  CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS Customers(
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  FullName TEXT NOT NULL,
  Phone TEXT,
  CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS Sales(
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  SaleDate TEXT NOT NULL DEFAULT (datetime('now')),
  ProductId INTEGER NOT NULL,
  Quantity INTEGER NOT NULL,
  UnitPrice REAL NOT NULL,
  FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

CREATE TABLE IF NOT EXISTS Invoices(
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  CustomerId INTEGER,
  Status TEXT NOT NULL DEFAULT 'Pending',
  Amount REAL NOT NULL DEFAULT 0,
  CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
  FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
);
";
            cmd.ExecuteNonQuery();
        }

        public static decimal GetTotalSales()
        {
            using var conn = Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT IFNULL(SUM(Quantity*UnitPrice),0) FROM Sales";
            var r = cmd.ExecuteScalar();
            return Convert.ToDecimal(r);
        }

        public static int GetPendingInvoiceCount()
        {
            using var conn = Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT COUNT(*) FROM Invoices WHERE Status='Pending'";
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public static int GetCriticalStockCount(int min = 3)
        {
            using var conn = Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT COUNT(*) FROM Products WHERE Stock <= $min";
            cmd.Parameters.AddWithValue("$min", min);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}
