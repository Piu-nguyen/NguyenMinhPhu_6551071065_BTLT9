using System.Diagnostics;
using Cau1.Data;
using Cau1.Models;
using Cau1.Services;
using FastReport.Export.Pdf;
using Microsoft.EntityFrameworkCore;

namespace Cau1;

public partial class Form1 : Form
{
    private static readonly string[] LoaiHang = ["Đồ uống", "Thực phẩm khô", "Gia vị", "Đồ gia dụng"];
    private static readonly string[] DonVi = ["Chai", "Gói", "Hộp", "Túi"];
    private int? selectedId;
    private int loadVersion;
    private bool initializing = true;
    private bool busy;

    public Form1()
    {
        InitializeComponent();
        cboLoaiHang.Items.AddRange(LoaiHang);
        cboDonVi.Items.AddRange(DonVi);
        cboLocLoai.Items.Add("Tất cả");
        cboLocLoai.Items.AddRange(LoaiHang);
        cboLocLoai.SelectedIndex = 0;
        ClearInputs();
        dgvHangHoa.SelectionChanged += (_, _) => ShowSelectedRow();
        dgvHangHoa.CurrentCellChanged += (_, _) => ShowSelectedRow();
        txtTimKiem.TextChanged += (_, _) => ScheduleSearch();
        cboLocLoai.SelectedIndexChanged += (_, _) => ScheduleSearch();
        chkChiConKinhDoanh.CheckedChanged += (_, _) => ScheduleSearch();
        searchTimer.Tick += async (_, _) =>
        {
            searchTimer.Stop();
            await SearchAsync();
        };
        Shown += async (_, _) =>
        {
            initializing = false;
            await RunAsync(() => LoadRowsAsync());
        };
        FormClosing += (_, _) => { searchTimer.Stop(); loadVersion++; };
    }

    private void ScheduleSearch()
    {
        if (initializing) return;
        loadVersion++; // Không hiển thị kết quả truy vấn cũ sau khi đổi bộ lọc.
        searchTimer.Stop();
        searchTimer.Start();
    }

    private async Task SearchAsync()
    {
        try { await LoadRowsAsync(); }
        catch (Exception ex) { if (!IsDisposed) ShowError(ex); }
    }

    private async Task LoadRowsAsync(int? selectId = null)
    {
        int version = ++loadVersion;
        string keyword = txtTimKiem.Text.Trim();
        string? category = cboLocLoai.SelectedIndex > 0 ? cboLocLoai.Text : null;
        bool activeOnly = chkChiConKinhDoanh.Checked;
        await using var db = Database.CreateContext();
        var query = db.HangHoas.AsNoTracking();
        if (keyword.Length > 0) query = query.Where(x => x.TenHang.Contains(keyword));
        if (category != null) query = query.Where(x => x.LoaiHang == category);
        if (activeOnly) query = query.Where(x => x.ConKinhDoanh);
        var rows = await query.OrderBy(x => x.MaHang).ToListAsync();
        if (IsDisposed || Disposing || version != loadVersion) return;
        dgvHangHoa.DataSource = rows;
        dgvHangHoa.ClearSelection();
        dgvHangHoa.CurrentCell = null;
        selectedId = null;
        ClearInputs();
        if (selectId.HasValue)
        {
            foreach (DataGridViewRow row in dgvHangHoa.Rows)
                if (row.DataBoundItem is HangHoa item && item.MaHang == selectId)
                {
                    dgvHangHoa.CurrentCell = row.Cells[0];
                    row.Selected = true;
                    break;
                }
        }
        lblTrangThai.Text = $"Tìm thấy {rows.Count} mặt hàng";
    }

    private void ShowSelectedRow()
    {
        if (dgvHangHoa.CurrentRow?.DataBoundItem is not HangHoa item)
        {
            selectedId = null;
            return;
        }
        selectedId = item.MaHang;
        txtTenHang.Text = item.TenHang;
        SelectCombo(cboLoaiHang, item.LoaiHang);
        SelectCombo(cboDonVi, item.DonVi);
        nudGiaBan.Value = item.GiaBan;
        nudSoLuongTon.Value = item.SoLuongTon;
        dtpNgayNhap.Value = item.NgayNhap;
        chkConKinhDoanh.Checked = item.ConKinhDoanh;
    }

    private static void SelectCombo(ComboBox combo, string value)
    {
        if (!combo.Items.Contains(value)) combo.Items.Add(value);
        combo.SelectedItem = value;
    }

    private void ClearInputs()
    {
        selectedId = null;
        txtTenHang.Clear();
        cboLoaiHang.SelectedIndex = 0;
        cboDonVi.SelectedIndex = 0;
        nudGiaBan.Value = 0;
        nudSoLuongTon.Value = 0;
        dtpNgayNhap.Value = DateTime.Today;
        chkConKinhDoanh.Checked = true;
    }

    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(txtTenHang.Text))
        {
            MessageBox.Show(this, "Tên hàng không được để trống.", "Kiểm tra dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtTenHang.Focus();
            return false;
        }
        if (nudGiaBan.Value <= 0)
        {
            MessageBox.Show(this, "Giá bán phải lớn hơn 0.", "Kiểm tra dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            nudGiaBan.Focus();
            return false;
        }
        return true;
    }

    private void FillEntity(HangHoa item)
    {
        item.TenHang = txtTenHang.Text.Trim();
        item.LoaiHang = cboLoaiHang.Text;
        item.DonVi = cboDonVi.Text;
        item.GiaBan = nudGiaBan.Value;
        item.SoLuongTon = (int)nudSoLuongTon.Value;
        item.NgayNhap = dtpNgayNhap.Value;
        item.ConKinhDoanh = chkConKinhDoanh.Checked;
    }

    private async Task AddAsync()
    {
        if (!ValidateInputs()) return;
        await using var db = Database.CreateContext();
        var item = new HangHoa();
        FillEntity(item);
        db.HangHoas.Add(item);
        await db.SaveChangesAsync();
        await LoadRowsAsync(item.MaHang);
    }

    private int RequireSelection() => selectedId
        ?? throw new InvalidOperationException("Vui lòng chọn một mặt hàng trên lưới trước.");

    private async Task UpdateAsync()
    {
        int id = RequireSelection();
        if (!ValidateInputs()) return;
        await using var db = Database.CreateContext();
        var item = await db.HangHoas.FindAsync(id)
            ?? throw new InvalidOperationException("Mặt hàng này không còn tồn tại. Hãy bấm Làm mới.");
        FillEntity(item);
        await db.SaveChangesAsync();
        await LoadRowsAsync(id);
    }

    private async Task DeleteAsync()
    {
        int id = RequireSelection();
        if (MessageBox.Show(this, $"Bạn có chắc muốn xóa mặt hàng mã {id}?", "Xác nhận xóa",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        await using var db = Database.CreateContext();
        var item = await db.HangHoas.FindAsync(id)
            ?? throw new InvalidOperationException("Mặt hàng này không còn tồn tại. Hãy bấm Làm mới.");
        db.HangHoas.Remove(item);
        await db.SaveChangesAsync();
        await LoadRowsAsync();
    }

    private async Task ResetAsync()
    {
        initializing = true;
        txtTimKiem.Clear();
        cboLocLoai.SelectedIndex = 0;
        chkChiConKinhDoanh.Checked = false;
        initializing = false;
        searchTimer.Stop();
        ClearInputs();
        await LoadRowsAsync();
        txtTenHang.Focus();
    }

    private async Task PreviewAsync()
    {
        using var report = await ReportService.CreatePriceReportAsync();
        report.Show(true, this);
    }

    private async Task ExportPdfAsync()
    {
        using var dialog = new SaveFileDialog { Filter = "PDF (*.pdf)|*.pdf", FileName = $"BangGia_{DateTime.Now:yyyyMMdd}.pdf", DefaultExt = "pdf" };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        using var report = await ReportService.CreatePriceReportAsync();
        await Task.Run(() =>
        {
            report.Prepare();
            using var pdf = new PDFExport { EmbeddingFonts = true };
            report.Export(pdf, dialog.FileName);
        });
        AskOpenFile(dialog.FileName);
    }

    private async Task ExportGridAsync()
    {
        using var dialog = ExcelDialog("HangHoa_Luoi");
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        // Chụp đúng các dòng/cột đang hiển thị; không truy vấn lại CSDL.
        var columns = dgvHangHoa.Columns.Cast<DataGridViewColumn>()
            .Where(x => x.Visible).OrderBy(x => x.DisplayIndex)
            .Select(x => new GridExportColumn(x.Index, x.HeaderText, x.DataPropertyName)).ToArray();
        var rows = dgvHangHoa.Rows.Cast<DataGridViewRow>()
            .Where(x => x.Visible && !x.IsNewRow)
            .Select(row => columns.Select(col => row.Cells[col.Index].Value).ToArray()).ToArray();
        await Task.Run(() => ExcelService.ExportGrid(dialog.FileName, columns, rows));
        AskOpenFile(dialog.FileName);
    }

    private async Task ExportInventoryAsync()
    {
        using var dialog = ExcelDialog("TonKho");
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        await using var db = Database.CreateContext();
        var items = await db.HangHoas.AsNoTracking().Where(x => x.ConKinhDoanh)
            .OrderBy(x => x.MaHang).ToListAsync();
        await Task.Run(() => ExcelService.ExportInventory(dialog.FileName, items));
        AskOpenFile(dialog.FileName);
    }

    private static SaveFileDialog ExcelDialog(string name) => new()
    {
        Filter = "Excel (*.xlsx)|*.xlsx", DefaultExt = "xlsx",
        FileName = $"{name}_{DateTime.Now:yyyyMMdd}.xlsx"
    };

    private void AskOpenFile(string path)
    {
        if (MessageBox.Show(this, "Đã xuất file thành công. Bạn có muốn mở file không?", "Xuất tài liệu",
            MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
    }

    private async Task RunAsync(Func<Task> action)
    {
        if (busy) return;
        busy = true;
        pnlButtons.Enabled = false;
        UseWaitCursor = true;
        try { await action(); }
        catch (Exception ex) { if (!IsDisposed) ShowError(ex); }
        finally
        {
            busy = false;
            if (!IsDisposed) { pnlButtons.Enabled = true; UseWaitCursor = false; }
        }
    }

    private void ShowError(Exception ex)
    {
        string message = ex switch
        {
            IOException => "Không thể đọc hoặc ghi file. Nếu file đang mở trong Excel hoặc phần mềm đọc PDF, hãy đóng file rồi thử lại.\n\n" + ex.Message,
            UnauthorizedAccessException => "Không có quyền ghi file tại thư mục này. Hãy chọn thư mục khác.\n\n" + ex.Message,
            Microsoft.Data.SqlClient.SqlException => "Không thể truy cập SQL Server. Kiểm tra server localhost, CSDL TiemTapHoa và cấu hình appsettings.json.\n\n" + ex.Message,
            DbUpdateException => "Không thể lưu dữ liệu.\n\n" + ex.GetBaseException().Message,
            _ => ex.Message
        };
        MessageBox.Show(this, message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
