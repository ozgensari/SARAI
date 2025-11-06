using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using SARAI.Modules;

namespace SARAI
{
    public class MainDashboard : Form
    {
        // Tema
        private readonly Color AppBg = Color.FromArgb(245, 247, 250);
        private readonly Color NavBlue = Color.FromArgb(30, 58, 138);
        private readonly Color CardStart = Color.FromArgb(0, 102, 204);
        private readonly Color CardEnd = Color.FromArgb(0, 51, 102);
        private readonly Color TextWhite = Color.White;
        private readonly Color TextDark = Color.FromArgb(15, 15, 15);
        private readonly Color AccentInfo = Color.FromArgb(37, 99, 235);
        private readonly Color AccentGreen = Color.FromArgb(46, 204, 113);

        // UI
        private Panel pnlHeader;
        private Label lblBrand, lblClock;
        private Button btnNavProducts, btnNavSales, btnNavStock, btnNavReports, btnNavCustomers, btnNavSettings;
        private Panel cardTotalSales, cardCriticalStock, cardPendingInvoice;
        private Label lblTotalSalesCaption, lblTotalSalesValue;
        private Label lblCriticalStockCaption, lblCriticalStockValue;
        private Label lblPendingInvoiceCaption, lblPendingInvoiceValue;
        private Chart chartSales;
        private Panel aiSuggestionPanel;
        private Label lblAiIcon, lblAiText;
        private Button btnWeeklyReport, btnBackup;

        public MainDashboard()
        {
            // Form
            Text = "SARAI — Ana Menü";
            BackColor = AppBg;
            DoubleBuffered = true;
            MinimumSize = new Size(1100, 650);
            StartPosition = FormStartPosition.CenterScreen;

            BuildHeader();
            BuildCards();
            BuildChart();
            BuildAiPanel();
            BuildActions();

            Resize += (_, __) => PositionAll();
            Load += MainDashboard_Load;
        }

        private void MainDashboard_Load(object sender, EventArgs e)
        {
            PositionAll();
            LoadDashboardData();
        }

        // Header
        private void BuildHeader()
        {
            pnlHeader = new Panel { Height = 52, Dock = DockStyle.Top, BackColor = NavBlue };
            Controls.Add(pnlHeader);

            lblBrand = new Label
            {
                Text = "SARAI",
                AutoSize = true,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(16, 12)
            };
            pnlHeader.Controls.Add(lblBrand);

            lblClock = new Label { AutoSize = true, Font = new Font("Segoe UI", 10f), ForeColor = Color.White };
            pnlHeader.Controls.Add(lblClock);
            var timer = new Timer { Interval = 1000 };
            timer.Tick += (_, __) => lblClock.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy  HH:mm");
            timer.Start();

            btnNavProducts = MakeNavButton("Ürünler");
            btnNavSales = MakeNavButton("Satış");
            btnNavStock = MakeNavButton("Stok");
            btnNavReports = MakeNavButton("Raporlar");
            btnNavCustomers = MakeNavButton("Müşteriler");
            btnNavSettings = MakeNavButton("Ayarlar");

            pnlHeader.Controls.AddRange(new Control[] {
                btnNavProducts, btnNavSales, btnNavStock, btnNavReports, btnNavCustomers, btnNavSettings
            });
        }

        private Button MakeNavButton(string text)
        {
            var b = new Button
            {
                Text = text,
                Size = new Size(90, 28),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        // Cards
        private Panel MakeCard()
        {
            var p = new Panel { Size = new Size(300, 100) };
            p.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var rect = p.ClientRectangle;
                using var path = RoundedRect(rect, 18);
                using var brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, CardStart, CardEnd, 45f);
                e.Graphics.FillPath(brush, path);
                using var borderPen = new Pen(Color.FromArgb(150, 0, 0, 0), 1);
                e.Graphics.DrawPath(borderPen, path);
            };
            return p;
        }

        private static System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private Label MakeCaption(string text) => new Label
        {
            Text = text,
            AutoSize = true,
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            ForeColor = TextWhite,
            Location = new Point(16, 14)
        };

        private Label MakeValue(string text) => new Label
        {
            Text = text,
            AutoSize = true,
            Font = new Font("Segoe UI", 13f, FontStyle.Bold),
            ForeColor = TextDark,                                   // SİYAH değer
            BackColor = Color.FromArgb(240, 240, 240, 240),          // hafif gri arka plan (okunurluk)
            Padding = new Padding(8, 3, 8, 3),
            Location = new Point(16, 46)
        };

        private void BuildCards()
        {
            cardTotalSales = MakeCard();
            cardCriticalStock = MakeCard();
            cardPendingInvoice = MakeCard();

            lblTotalSalesCaption = MakeCaption("Toplam Satış");
            lblTotalSalesValue = MakeValue("₺ 0,00");
            cardTotalSales.Controls.AddRange(new Control[] { lblTotalSalesCaption, lblTotalSalesValue });

            lblCriticalStockCaption = MakeCaption("Kritik Stok");
            lblCriticalStockValue = MakeValue("—");
            cardCriticalStock.Controls.AddRange(new Control[] { lblCriticalStockCaption, lblCriticalStockValue });

            lblPendingInvoiceCaption = MakeCaption("Bekleyen Fatura");
            lblPendingInvoiceValue = MakeValue("0");
            cardPendingInvoice.Controls.AddRange(new Control[] { lblPendingInvoiceCaption, lblPendingInvoiceValue });

            Controls.AddRange(new Control[] { cardTotalSales, cardCriticalStock, cardPendingInvoice });
        }

        // Chart
        private void BuildChart()
        {
            chartSales = new Chart { Size = new Size(740, 260), BackColor = Color.White };
            var area = new ChartArea("Area") { BackColor = Color.White };
            area.AxisX.Interval = 1;
            chartSales.ChartAreas.Add(area);

            var series = new Series("Satış")
            {
                ChartType = SeriesChartType.Area,
                Color = Color.FromArgb(120, 59, 130, 246),
                BorderWidth = 2
            };
            chartSales.Series.Add(series);
            Controls.Add(chartSales);
        }

        // AI panel
        private void BuildAiPanel()
        {
            aiSuggestionPanel = new Panel { Size = new Size(720, 56), BackColor = AccentInfo };
            lblAiIcon = new Label { Text = "🤖", Font = new Font("Segoe UI Emoji", 18f), ForeColor = Color.White, AutoSize = true, Location = new Point(14, 12) };
            lblAiText = new Label { Text = "AI Analiz: Sistem verilerini inceliyor...", Font = new Font("Segoe UI", 10f), ForeColor = Color.White, AutoSize = true, Location = new Point(52, 18) };
            aiSuggestionPanel.Controls.Add(lblAiIcon);
            aiSuggestionPanel.Controls.Add(lblAiText);
            Controls.Add(aiSuggestionPanel);
        }

        // Actions
        private void BuildActions()
        {
            btnWeeklyReport = new Button
            {
                Text = "📄 Haftalık Rapor Oluştur",
                Size = new Size(220, 40),
                BackColor = AccentGreen,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnWeeklyReport.FlatAppearance.BorderSize = 0;
            btnWeeklyReport.Click += (s, e) =>
            {
                var path = ReportsModule.CreateWeeklySalesReportPdf();
                MessageBox.Show("Rapor oluşturuldu:\n" + path, "SARAI", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            Controls.Add(btnWeeklyReport);

            btnBackup = new Button
            {
                Text = "💾 Yedek Al",
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnBackup.FlatAppearance.BorderSize = 0;
            btnBackup.Click += (s, e) =>
            {
                var path = SyncService.ExportJson();
                MessageBox.Show("Yedek alındı:\n" + path, "SARAI", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            Controls.Add(btnBackup);
        }

        private void PositionAll()
        {
            int x = lblBrand.Right + 20;
            int top = (pnlHeader.Height - 28) / 2;
            foreach (var btn in new[] { btnNavProducts, btnNavSales, btnNavStock, btnNavReports, btnNavCustomers, btnNavSettings })
            {
                btn.Location = new Point(x, top);
                x += btn.Width + 8;
            }
            lblClock.Location = new Point(pnlHeader.Width - lblClock.Width - 16, top + 2);

            int spacing = 22;
            int totalWidth = cardTotalSales.Width + cardCriticalStock.Width + cardPendingInvoice.Width + spacing * 2;
            int startX = Math.Max(20, (ClientSize.Width - totalWidth) / 2);
            int yCards = pnlHeader.Bottom + 70;

            cardTotalSales.Location = new Point(startX, yCards);
            cardCriticalStock.Location = new Point(cardTotalSales.Right + spacing, yCards);
            cardPendingInvoice.Location = new Point(cardCriticalStock.Right + spacing, yCards);

            chartSales.Width = Math.Min(900, ClientSize.Width - 260);
            chartSales.Location = new Point(Math.Max(20, (ClientSize.Width - chartSales.Width) / 2), cardTotalSales.Bottom + 42);

            aiSuggestionPanel.Location = new Point(Math.Max(20, (ClientSize.Width - aiSuggestionPanel.Width) / 2), chartSales.Bottom + 28);

            btnWeeklyReport.Location = new Point((ClientSize.Width - btnWeeklyReport.Width) / 2, aiSuggestionPanel.Bottom + 20);
            btnBackup.Location = new Point(btnWeeklyReport.Right + 12, aiSuggestionPanel.Bottom + 20);
        }

        private void LoadDashboardData()
        {
            try
            {
                // Kartlar
                lblTotalSalesValue.Text = DbConnection.GetTotalSales().ToString("C2");
                lblCriticalStockValue.Text = DbConnection.GetCriticalStockCount().ToString();
                lblPendingInvoiceValue.Text = DbConnection.GetPendingInvoiceCount().ToString();

                // Grafik (son 7 gün örnek)
                var s = chartSales.Series.First();
                s.Points.Clear();
                var labels = new[] { "Gün 1", "Gün 2", "Gün 3", "Gün 4", "Gün 5", "Gün 6", "Gün 7" };
                var values = new[] { 13500, 12000, 15000, 10000, 13000, 11500, 11800 };
                for (int i = 0; i < labels.Length; i++) s.Points.AddXY(labels[i], values[i]);

                // AI ipucu
                lblAiText.Text = "AI Analiz: " + AiEngine.BuildDashboardHint(
                    DbConnection.GetTotalSales(),
                    DbConnection.GetCriticalStockCount(),
                    DbConnection.GetPendingInvoiceCount());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dashboard yükleme hatası: " + ex.Message);
            }
        }
    }
}
