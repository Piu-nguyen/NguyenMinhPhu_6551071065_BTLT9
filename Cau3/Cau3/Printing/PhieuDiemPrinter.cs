using System.Drawing.Printing;
using System.Globalization;
using Cau3.Models;
using Cau3.Services;

namespace Cau3.Printing;

public sealed class PhieuDiemPrinter : IDisposable
{
    public PrintDocument Document { get; } = new();
    public int LastPageCount { get; private set; }
    private string tenKhoa = "";
    private string maSV = "";
    private string hoTen = "";
    private string maLop = "";
    private DateTime ngaySinh;
    private List<DiemMon> grades = [];
    private List<List<(int Index, float Height)>>? pages;
    private int pageIndex;
    private static readonly CultureInfo Vietnamese = CultureInfo.GetCultureInfo("vi-VN");
    private const float HeaderHeight = 190;
    private const float TableHeaderHeight = 30;
    private const float SummaryHeight = 135;
    private const float FooterHeight = 30;

    public PhieuDiemPrinter()
    {
        Document.DocumentName = "Phiếu báo điểm";
        Document.DefaultPageSettings.Margins = new Margins(60, 60, 50, 50);
        Document.BeginPrint += (_, _) =>
        {
            pageIndex = 0;
            pages = null;
            LastPageCount = 0;
        };
        Document.PrintPage += OnPrintPage;
    }

    public void SetData(string khoa, SinhVien student, IReadOnlyList<DiemMon> diem)
    {
        tenKhoa = khoa;
        maSV = student.MaSV;
        hoTen = student.HoTen;
        maLop = student.MaLop;
        ngaySinh = student.NgaySinh;
        grades = diem.Select(x => new DiemMon { TenMon = x.TenMon, Diem = x.Diem, SoTinChi = x.SoTinChi }).ToList();
        Document.DocumentName = $"PhieuDiem_{maSV}";
    }

    private void OnPrintPage(object? sender, PrintPageEventArgs e)
    {
        try
        {
            var g = e.Graphics ?? throw new InvalidOperationException("Không tạo được vùng vẽ cho máy in.");
            var bounds = e.MarginBounds;
            if (bounds.Width < 280 || bounds.Height < 430)
                throw new InvalidOperationException("Vùng in quá nhỏ. Hãy giảm lề hoặc chọn khổ giấy lớn hơn trong Cài đặt trang.");
            if (grades.Count == 0) throw new InvalidOperationException("Sinh viên chưa có điểm để in.");
            using var font = new Font("Arial", 11F);
            using var bold = new Font("Arial", 11F, FontStyle.Bold);
            using var title = new Font("Arial", 17F, FontStyle.Bold);
            using var small = new Font("Arial", 10F, FontStyle.Italic);
            using var center = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var left = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            float x = bounds.Left;
            float y = bounds.Top;
            float width = bounds.Width;
            float[] widths = [width * 0.09F, width * 0.61F, width * 0.15F, width * 0.15F];

            if (pages == null)
            {
                pages = [];
                float available = bounds.Height - HeaderHeight - TableHeaderHeight - SummaryHeight - FooterHeight;
                var current = new List<(int Index, float Height)>();
                float used = 0;
                for (int i = 0; i < grades.Count; i++)
                {
                    float height = Math.Max(30, g.MeasureString(grades[i].TenMon, font, (int)(widths[1] - 12)).Height + 8);
                    if (height > available) throw new InvalidOperationException("Tên môn quá dài so với vùng in. Hãy tăng kích thước vùng in.");
                    if (current.Count > 0 && used + height > available)
                    {
                        pages.Add(current);
                        current = [];
                        used = 0;
                    }
                    current.Add((i, height));
                    used += height;
                }
                if (current.Count > 0) pages.Add(current);
                LastPageCount = pages.Count;
            }

            g.DrawString(tenKhoa, bold, Brushes.Black, new RectangleF(x, y, width, 30), left);
            g.DrawString("PHIẾU BÁO ĐIỂM", title, Brushes.Black, new RectangleF(x, y + 38, width, 40), center);
            g.DrawString($"MSSV: {maSV}", font, Brushes.Black, new RectangleF(x, y + 88, width, 22), left);
            g.DrawString($"Họ tên: {hoTen}", bold, Brushes.Black, new RectangleF(x, y + 112, width, 24), left);
            g.DrawString($"Ngày sinh: {ngaySinh:dd/MM/yyyy}", font, Brushes.Black, new RectangleF(x, y + 140, width, 22), left);
            g.DrawString($"Lớp: {maLop}", font, Brushes.Black, new RectangleF(x, y + 164, width, 22), left);
            y += HeaderHeight;

            void DrawRow(string[] values, float height, bool isHeader)
            {
                float cellX = x;
                for (int col = 0; col < values.Length; col++)
                {
                    var rect = new RectangleF(cellX, y, widths[col], height);
                    if (isHeader) g.FillRectangle(Brushes.Gainsboro, rect);
                    g.DrawRectangle(Pens.Black, rect.X, rect.Y, rect.Width, rect.Height);
                    var content = new RectangleF(rect.X + 6, rect.Y + 3, rect.Width - 12, rect.Height - 6);
                    g.DrawString(values[col], isHeader ? bold : font, Brushes.Black, content, col == 1 && !isHeader ? left : center);
                    cellX += widths[col];
                }
                y += height;
            }

            DrawRow(["STT", "Môn học", "Số TC", "Điểm"], TableHeaderHeight, true);
            foreach (var row in pages[pageIndex])
            {
                var grade = grades[row.Index];
                DrawRow([(row.Index + 1).ToString(), grade.TenMon, grade.SoTinChi.ToString(), grade.Diem.ToString("N1", Vietnamese)], row.Height, false);
            }

            if (pageIndex == pages.Count - 1)
            {
                var result = BangDiem.Tinh(grades);
                y += 12;
                g.DrawString($"Điểm trung bình: {result.DiemTrungBinh!.Value.ToString("N2", Vietnamese)} – Xếp loại: {result.XepLoai}", bold, Brushes.Black,
                    new RectangleF(x, y, width, 30), center);
                y += 46;
                g.DrawString("Giáo vụ khoa", bold, Brushes.Black, new RectangleF(x, y, width / 2, 26), center);
                g.DrawString("Sinh viên", bold, Brushes.Black, new RectangleF(x + width / 2, y, width / 2, 26), center);
                g.DrawString("(Ký, ghi rõ họ tên)", small, Brushes.Black, new RectangleF(x, y + 28, width / 2, 24), center);
                g.DrawString("(Ký, ghi rõ họ tên)", small, Brushes.Black, new RectangleF(x + width / 2, y + 28, width / 2, 24), center);
            }

            g.DrawString($"Trang {pageIndex + 1}/{pages.Count}", font, Brushes.Black,
                new RectangleF(x, bounds.Bottom - FooterHeight, width, FooterHeight), center);
            pageIndex++;
            e.HasMorePages = pageIndex < pages.Count;
        }
        catch (Exception ex)
        {
            e.Cancel = true;
            e.HasMorePages = false;
            MessageBox.Show($"Không thể tạo phiếu báo điểm.\n{ex.Message}", "Lỗi in phiếu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void Dispose() => Document.Dispose();
}
