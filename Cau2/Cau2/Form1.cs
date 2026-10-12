using System.Diagnostics;
using System.Drawing.Printing;
using Cau2.Data;
using Cau2.Models;
using Cau2.Services;
using Cau2.ViewModels;
using FastReport.Export.Pdf;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Cau2;

public partial class Form1 : Form
{
    private bool busy;
    private bool binding;
    private int? selectedId;

    public Form1()
    {
        InitializeComponent();
        Shown += async (_, _) => await RunAsync(InitializeAsync);
        gridHoaDon.SelectionChanged += async (_, _) =>
        {
            if (!binding && !busy && gridHoaDon.CurrentRow?.DataBoundItem is HoaDonRow row)
                await RunAsync(() => SelectInvoiceAsync(row));
        };
        gridChiTiet.SelectionChanged += (_, _) =>
        {
            if (gridChiTiet.CurrentRow?.DataBoundItem is ChiTietIn row)
            {
                cboMon.SelectedValue = row.MaMon;
                nudSoLuong.Value = Math.Clamp(row.SoLuong, 1, 50);
            }
        };
        btnTim.Click += async (_, _) => await RunAsync(() => ReloadAsync(selectedId));
        btnLamMoi.Click += async (_, _) => await RunAsync(async () =>
        {
            await ResetFiltersAsync();
            await ReloadAsync(selectedId);
        });
        txtTenMon.KeyDown += async (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                await RunAsync(() => ReloadAsync(selectedId));
            }
        };
        btnHoaDonMoi.Click += async (_, _) => await RunAsync(CreateInvoiceAsync);
        btnSuaHoaDon.Click += async (_, _) => await RunAsync(UpdateInvoiceAsync);
        btnXoaHoaDon.Click += async (_, _) => await RunAsync(DeleteInvoiceAsync);
        btnThemMon.Click += async (_, _) => await RunAsync(AddItemAsync);
        btnSuaSL.Click += async (_, _) => await RunAsync(UpdateQuantityAsync);
        btnXoaMon.Click += async (_, _) => await RunAsync(DeleteItemAsync);
        btnXem.Click += async (_, _) => await RunAsync(() => ReportAsync("preview"));
        btnPdf.Click += async (_, _) => await RunAsync(() => ReportAsync("pdf"));
        btnIn.Click += async (_, _) => await RunAsync(() => ReportAsync("print"));
        FormClosing += (_, e) => { if (busy) e.Cancel = true; };
    }

    // Một thao tác mỗi lần để tránh kết quả truy vấn cũ ghi đè hóa đơn đang chọn.
    private async Task RunAsync(Func<Task> action)
    {
        if (busy) return;
        busy = true;
        contentPanel.Enabled = false;
        UseWaitCursor = true;
        lblStatus.Text = "Đang xử lý...";
        try
        {
            await action();
            lblStatus.Text = "Sẵn sàng • SQL Server: localhost • QuanCaPhe";
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Thao tác chưa hoàn tất";
            string message = ex switch
            {
                SqlException => "Không thể thao tác với CSDL. Kiểm tra SQL Server và kết nối trong appsettings.json.",
                DbUpdateException => "Không thể lưu dữ liệu. Hãy làm mới danh sách và thử lại.",
                InvalidPrinterException => "Không tìm thấy máy in hợp lệ. Hãy cài máy in hoặc chọn Microsoft Print to PDF.",
                IOException => "Không thể đọc/ghi file. Nếu file PDF đang mở, hãy đóng file rồi thử lại.",
                UnauthorizedAccessException => "Không có quyền ghi vào thư mục này. Hãy chọn thư mục khác.",
                _ => "Không thể hoàn tất thao tác."
            };
            MessageBox.Show(this, $"{message}\n\nChi tiết: {ex.GetBaseException().Message}",
                "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            UseWaitCursor = false;
            contentPanel.Enabled = true;
            busy = false;
        }
    }

    private async Task InitializeAsync()
    {
        await using var db = Database.CreateContext();
        var menu = await db.Mons.AsNoTracking().OrderBy(m => m.MaMon)
            .Select(m => new { m.MaMon, m.TenMon, m.DonGia }).ToListAsync();
        cboMon.DisplayMember = "Display";
        cboMon.ValueMember = "MaMon";
        cboMon.DataSource = menu.Select(m => new
        {
            m.MaMon, Display = $"{m.TenMon} — {m.DonGia:N0} đ"
        }).ToList();
        await ResetFiltersAsync();
        await ReloadAsync();
    }

    private async Task ResetFiltersAsync()
    {
        await using var db = Database.CreateContext();
        var min = await db.HoaDons.MinAsync(h => (DateTime?)h.NgayLap);
        var max = await db.HoaDons.MaxAsync(h => (DateTime?)h.NgayLap);
        dtpTuNgay.Value = (min ?? DateTime.Today).Date;
        dtpDenNgay.Value = (max > DateTime.Today ? max.Value : DateTime.Today).Date;
        cboLocBan.SelectedIndex = 0;
        cboLocTT.SelectedIndex = 0;
        txtTenMon.Clear();
    }

    private async Task ReloadAsync(int? preferredId = null)
    {
        if (dtpTuNgay.Value.Date > dtpDenNgay.Value.Date)
        {
            Notify("Từ ngày phải nhỏ hơn hoặc bằng đến ngày.");
            return;
        }
        var from = dtpTuNgay.Value.Date;
        var until = dtpDenNgay.Value.Date.AddDays(1);
        string name = txtTenMon.Text.Trim();
        await using var db = Database.CreateContext();
        var query = db.HoaDons.AsNoTracking().Where(h => h.NgayLap >= from && h.NgayLap < until);
        if (cboLocBan.SelectedIndex > 0)
        {
            int table = cboLocBan.SelectedIndex;
            query = query.Where(h => h.SoBan == table);
        }
        if (cboLocTT.SelectedIndex > 0)
        {
            string payment = cboLocTT.Text;
            query = query.Where(h => h.HinhThucTT == payment);
        }
        if (name.Length > 0)
            query = query.Where(h => h.ChiTietHoaDons.Any(ct => ct.MaMonNavigation.TenMon.Contains(name)));

        var invoices = await query.OrderByDescending(h => h.NgayLap).ThenByDescending(h => h.MaHD)
            .Select(h => new HoaDonRow
            {
                MaHD = h.MaHD, NgayLap = h.NgayLap, SoBan = h.SoBan,
                HinhThucTT = h.HinhThucTT, GhiChu = h.GhiChu,
                TongTien = h.ChiTietHoaDons.Sum(ct => (decimal?)(ct.SoLuong * ct.DonGia)) ?? 0
            }).ToListAsync();
        binding = true;
        try
        {
            gridHoaDon.DataSource = invoices;
            gridHoaDon.ClearSelection();
            var selected = invoices.FirstOrDefault(h => h.MaHD == preferredId) ?? invoices.FirstOrDefault();
            if (selected != null)
            {
                int index = invoices.IndexOf(selected);
                gridHoaDon.CurrentCell = gridHoaDon.Rows[index].Cells[0];
                gridHoaDon.Rows[index].Selected = true;
                await SelectInvoiceAsync(selected);
            }
            else ClearInvoice();
            lblDoanhThu.Text = $"Tổng doanh thu đang hiển thị: {invoices.Sum(h => h.TongTien):N0} đ";
            lblCount.Text = $"{invoices.Count} hóa đơn";
        }
        finally { binding = false; }
    }

    private async Task SelectInvoiceAsync(HoaDonRow row)
    {
        // Chỉ cập nhật vùng chi tiết sau khi truy vấn hoàn tất.
        await using var db = Database.CreateContext();
        var details = await db.ChiTietHoaDons.AsNoTracking().Where(ct => ct.MaHD == row.MaHD)
            .OrderBy(ct => ct.MaCT).Select(ct => new ChiTietIn
            {
                MaCT = ct.MaCT, MaMon = ct.MaMon, TenMon = ct.MaMonNavigation.TenMon,
                SoLuong = ct.SoLuong, DonGia = ct.DonGia
            }).ToListAsync();
        for (int i = 0; i < details.Count; i++) details[i].STT = i + 1;
        selectedId = row.MaHD;
        groupChiTiet.Text = $"Chi tiết hóa đơn {row.SoHoaDon}";
        dtpNgayLap.Value = row.NgayLap;
        cboSoBan.SelectedItem = row.SoBan;
        rdoTienMat.Checked = row.HinhThucTT == "Tiền mặt";
        rdoChuyenKhoan.Checked = row.HinhThucTT == "Chuyển khoản";
        txtGhiChu.Text = row.GhiChu ?? "";
        gridChiTiet.DataSource = details;
        lblTongCong.Text = $"Tổng cộng: {details.Sum(ct => ct.ThanhTien):N0} đ";
    }

    private void ClearInvoice()
    {
        selectedId = null;
        groupChiTiet.Text = "Chi tiết hóa đơn • Chưa chọn hóa đơn";
        gridChiTiet.DataSource = new List<ChiTietIn>();
        dtpNgayLap.Value = DateTime.Now;
        cboSoBan.SelectedIndex = 0;
        rdoTienMat.Checked = true;
        txtGhiChu.Clear();
        nudSoLuong.Value = 1;
        lblTongCong.Text = "Tổng cộng: 0 đ";
    }

    private bool RequireInvoice()
    {
        if (selectedId.HasValue) return true;
        Notify("Vui lòng chọn một hóa đơn trước.");
        return false;
    }

    private ChiTietIn? RequireItem()
    {
        if (!RequireInvoice()) return null;
        if (gridChiTiet.CurrentRow?.DataBoundItem is ChiTietIn row) return row;
        Notify("Vui lòng chọn một dòng món trong hóa đơn.");
        return null;
    }

    private void FillInvoice(HoaDon invoice)
    {
        invoice.NgayLap = dtpNgayLap.Value;
        invoice.SoBan = (int)(cboSoBan.SelectedItem ?? 1);
        invoice.HinhThucTT = rdoChuyenKhoan.Checked ? "Chuyển khoản" : "Tiền mặt";
        invoice.GhiChu = string.IsNullOrWhiteSpace(txtGhiChu.Text) ? null : txtGhiChu.Text.Trim();
    }

    private async Task CreateInvoiceAsync()
    {
        await using var db = Database.CreateContext();
        var invoice = new HoaDon();
        FillInvoice(invoice);
        db.HoaDons.Add(invoice);
        await db.SaveChangesAsync();
        await ResetFiltersAsync();
        await ReloadAsync(invoice.MaHD);
    }

    private async Task UpdateInvoiceAsync()
    {
        if (!RequireInvoice()) return;
        await using var db = Database.CreateContext();
        var invoice = await db.HoaDons.FindAsync(selectedId!.Value)
            ?? throw new InvalidOperationException("Hóa đơn đã bị xóa. Hãy bấm Tìm để tải lại danh sách.");
        FillInvoice(invoice);
        await db.SaveChangesAsync();
        await ReloadAsync(invoice.MaHD);
    }

    private async Task DeleteInvoiceAsync()
    {
        if (!RequireInvoice()) return;
        int id = selectedId!.Value;
        if (!Confirm($"Xóa hóa đơn HD{id:000000} và toàn bộ món trong hóa đơn?")) return;
        await using var db = Database.CreateContext();
        await using var transaction = await db.Database.BeginTransactionAsync();
        await db.ChiTietHoaDons.Where(ct => ct.MaHD == id).ExecuteDeleteAsync();
        await db.HoaDons.Where(h => h.MaHD == id).ExecuteDeleteAsync();
        await transaction.CommitAsync();
        await ReloadAsync();
    }

    private async Task AddItemAsync()
    {
        if (!RequireInvoice()) return;
        if (cboMon.SelectedValue is not int monId)
        {
            Notify("Vui lòng chọn món cần thêm.");
            return;
        }
        await using var db = Database.CreateContext();
        var mon = await db.Mons.AsNoTracking().SingleAsync(m => m.MaMon == monId);
        // Luôn tạo dòng mới và lưu giá tại thời điểm bán, không dùng giá cũ trên ComboBox.
        db.ChiTietHoaDons.Add(new ChiTietHoaDon
        {
            MaHD = selectedId!.Value, MaMon = monId,
            SoLuong = (int)nudSoLuong.Value, DonGia = mon.DonGia
        });
        await db.SaveChangesAsync();
        await ReloadAsync(selectedId);
    }

    private async Task UpdateQuantityAsync()
    {
        var row = RequireItem();
        if (row == null) return;
        await using var db = Database.CreateContext();
        var detail = await db.ChiTietHoaDons.SingleAsync(ct => ct.MaCT == row.MaCT && ct.MaHD == selectedId);
        detail.SoLuong = (int)nudSoLuong.Value;
        await db.SaveChangesAsync();
        await ReloadAsync(selectedId);
    }

    private async Task DeleteItemAsync()
    {
        var row = RequireItem();
        if (row == null || !Confirm($"Xóa món “{row.TenMon}” khỏi hóa đơn?")) return;
        await using var db = Database.CreateContext();
        await db.ChiTietHoaDons.Where(ct => ct.MaCT == row.MaCT && ct.MaHD == selectedId).ExecuteDeleteAsync();
        await ReloadAsync(selectedId);
    }

    private async Task ReportAsync(string mode)
    {
        if (!RequireInvoice()) return;
        await using var db = Database.CreateContext();
        var invoice = await db.HoaDons.AsNoTracking().Include(h => h.ChiTietHoaDons)
            .ThenInclude(ct => ct.MaMonNavigation).SingleAsync(h => h.MaHD == selectedId);
        var details = invoice.ChiTietHoaDons.OrderBy(ct => ct.MaCT).Select((ct, i) => new ChiTietIn
        {
            STT = i + 1, TenMon = ct.MaMonNavigation.TenMon, SoLuong = ct.SoLuong, DonGia = ct.DonGia
        }).ToList();
        if (details.Count == 0)
        {
            Notify("Hóa đơn chưa có món. Vui lòng thêm món trước khi xem, xuất PDF hoặc in.");
            return;
        }
        using var report = HoaDonReport.Create(invoice, details);
        UseWaitCursor = false;
        if (mode == "preview") report.ShowPrepared(true, this);
        else if (mode == "print")
        {
            report.PrintSettings.ShowDialog = true;
            report.Print();
        }
        else
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "Tài liệu PDF (*.pdf)|*.pdf", DefaultExt = "pdf", AddExtension = true,
                FileName = $"HoaDon_HD{invoice.MaHD:000000}.pdf", Title = "Xuất hóa đơn PDF"
            };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            UseWaitCursor = true;
            await Task.Run(() =>
            {
                using var export = new PDFExport { EmbeddingFonts = true };
                report.Export(export, dialog.FileName);
            });
            UseWaitCursor = false;
            if (Confirm("Đã xuất PDF thành công. Bạn có muốn mở file không?"))
                Process.Start(new ProcessStartInfo(dialog.FileName) { UseShellExecute = true });
        }
    }

    private void Notify(string message) => MessageBox.Show(this, message, "Quán cà phê Góc Phố",
        MessageBoxButtons.OK, MessageBoxIcon.Information);

    private bool Confirm(string message) => MessageBox.Show(this, message, "Quán cà phê Góc Phố",
        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
}
