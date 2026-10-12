using System.Drawing.Printing;
using Cau3.Data;
using Cau3.Models;
using Cau3.Printing;
using Cau3.Services;
using Microsoft.EntityFrameworkCore;

namespace Cau3;

public partial class Form1 : Form
{
    private AppSettings settings = new();
    private readonly PhieuDiemPrinter printer = new();
    private readonly System.Windows.Forms.Timer searchTimer;
    private string? selectedMaSV;
    private int? selectedMaDiem;
    private bool binding;
    private bool busy;
    private int studentRequest;
    private int gradeRequest;
    private bool loaded;

    private sealed record LopItem(string MaLop, string Display);
    private sealed record GradeRow(int MaDiem, int STT, string TenMon, int SoTinChi, decimal Diem);

    public Form1()
    {
        InitializeComponent();
        searchTimer = new System.Windows.Forms.Timer(components!) { Interval = 350 };
        searchTimer.Tick += async (_, _) =>
        {
            searchTimer.Stop();
            await RunAsync(() => LoadStudentsAsync(), false);
        };
        txtTim.TextChanged += (_, _) => { searchTimer.Stop(); if (loaded) searchTimer.Start(); };
        lstLop.SelectedIndexChanged += async (_, _) =>
        {
            if (loaded && !binding) await RunAsync(() => LoadStudentsAsync(), false);
        };
        dgvSinhVien.SelectionChanged += async (_, _) =>
        {
            if (!binding && loaded) await RunAsync(SelectStudentAsync, false);
        };
        dgvDiem.SelectionChanged += (_, _) =>
        {
            if (binding) return;
            if (dgvDiem.CurrentRow?.DataBoundItem is GradeRow row)
            {
                selectedMaDiem = row.MaDiem;
                txtTenMon.Text = row.TenMon;
                nudTinChi.Value = row.SoTinChi;
                nudDiem.Value = row.Diem;
            }
        };
        Shown += async (_, _) => await RunAsync(InitializeAsync);
    }

    private async Task RunAsync(Func<Task> action, bool showBusy = true)
    {
        if (showBusy && busy) return;
        try
        {
            if (showBusy)
            {
                busy = true;
                ++studentRequest;
                ++gradeRequest;
                searchTimer.Stop();
                mainLayout.Enabled = false;
                UseWaitCursor = true;
            }
            await action();
        }
        catch (InvalidPrinterException)
        {
            MessageBox.Show(this, "Không tìm thấy máy in hợp lệ. Hãy cài máy in hoặc bật Microsoft Print to PDF rồi thử lại.", "Lỗi máy in", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (IOException ex)
        {
            MessageBox.Show(this, $"Không thể đọc/ghi file. Nếu file Excel đang mở, hãy đóng file rồi thử lại.\n\n{ex.Message}", "Lỗi file", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Thao tác không thành công.\n\n{ex.GetBaseException().Message}", "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (showBusy)
            {
                busy = false;
                mainLayout.Enabled = true;
                UseWaitCursor = false;
            }
        }
    }

    private async Task InitializeAsync()
    {
        settings = AppSettings.Load();
        await using var db = settings.CreateContext();
        var classes = await db.Lops.AsNoTracking().OrderBy(x => x.MaLop).ToListAsync();
        binding = true;
        try
        {
            lstLop.DisplayMember = nameof(LopItem.Display);
            lstLop.DataSource = new[] { new LopItem("", "Tất cả") }
                .Concat(classes.Select(x => new LopItem(x.MaLop, $"{x.MaLop} - {x.TenLop}"))).ToList();
            cboLop.DisplayMember = nameof(Lop.MaLop);
            cboLop.ValueMember = nameof(Lop.MaLop);
            cboLop.DataSource = classes;
        }
        finally { binding = false; }
        loaded = true;
        await LoadStudentsAsync();
    }

    private async Task LoadStudentsAsync(string? preferMaSV = null)
    {
        int request = ++studentRequest;
        string classId = (lstLop.SelectedItem as LopItem)?.MaLop ?? "";
        string keyword = txtTim.Text.Trim();
        string? previous = preferMaSV ?? selectedMaSV;
        await using var db = settings.CreateContext();
        var query = db.SinhViens.AsNoTracking();
        if (classId.Length > 0) query = query.Where(x => x.MaLop == classId);
        if (keyword.Length > 0)
            query = query.Where(x => EF.Functions.Collate(x.MaSV, "Vietnamese_100_CI_AS").Contains(keyword)
                || EF.Functions.Collate(x.HoTen, "Vietnamese_100_CI_AS").Contains(keyword));
        var rows = await query.OrderBy(x => x.MaSV).ToListAsync();
        if (request != studentRequest || IsDisposed) return;
        binding = true;
        try
        {
            dgvSinhVien.DataSource = rows;
            dgvSinhVien.ClearSelection();
            var index = rows.FindIndex(x => x.MaSV == previous);
            if (index < 0 && rows.Count > 0) index = 0;
            if (index >= 0)
            {
                dgvSinhVien.CurrentCell = dgvSinhVien.Rows[index].Cells[0];
                dgvSinhVien.Rows[index].Selected = true;
            }
            else dgvSinhVien.CurrentCell = null;
        }
        finally { binding = false; }
        lblTrangThai.Text = $"Tìm thấy {rows.Count} sinh viên";
        await SelectStudentAsync();
    }

    private async Task SelectStudentAsync()
    {
        if (dgvSinhVien.CurrentRow?.DataBoundItem is not SinhVien student)
        {
            ClearStudent();
            return;
        }
        selectedMaSV = student.MaSV;
        txtMaSV.Text = student.MaSV;
        txtMaSV.ReadOnly = true;
        txtHoTen.Text = student.HoTen;
        dtpNgaySinh.Value = student.NgaySinh;
        radNam.Checked = student.GioiTinh == "Nam";
        radNu.Checked = !radNam.Checked;
        cboLop.SelectedValue = student.MaLop;
        grpDiem.Text = $"Điểm của sinh viên: {student.MaSV} - {student.HoTen}";
        binding = true;
        try { dgvDiem.DataSource = null; }
        finally { binding = false; }
        ClearGrade();
        ShowSummary(BangDiem.Tinh([]));
        await LoadGradesAsync(student.MaSV);
    }

    private async Task LoadGradesAsync(string maSV)
    {
        int request = ++gradeRequest;
        await using var db = settings.CreateContext();
        var grades = await db.DiemMons.AsNoTracking().Where(x => x.MaSV == maSV).OrderBy(x => x.MaDiem).ToListAsync();
        if (request != gradeRequest || selectedMaSV != maSV || IsDisposed) return;
        binding = true;
        try
        {
            dgvDiem.DataSource = grades.Select((x, i) => new GradeRow(x.MaDiem, i + 1, x.TenMon, x.SoTinChi, x.Diem)).ToList();
            dgvDiem.ClearSelection();
            dgvDiem.CurrentCell = null;
        }
        finally { binding = false; }
        ClearGrade();
        ShowSummary(BangDiem.Tinh(grades));
    }

    private void ShowSummary(KetQuaDiem result)
    {
        lblTinChi.Text = $"Tổng tín chỉ: {result.TongTinChi}";
        lblTrungBinh.Text = result.DiemTrungBinh.HasValue ? $"Điểm TB: {result.DiemTrungBinh:N2}" : "Điểm TB: —";
        lblXepLoai.Text = result.DiemTrungBinh.HasValue ? $"Xếp loại: {result.XepLoai}" : result.XepLoai;
    }

    private void ClearGrade()
    {
        selectedMaDiem = null;
        txtTenMon.Clear();
        nudTinChi.Value = 3;
        nudDiem.Value = 0;
    }

    private void ClearStudent()
    {
        ++gradeRequest;
        selectedMaSV = null;
        binding = true;
        try
        {
            dgvSinhVien.ClearSelection();
            dgvSinhVien.CurrentCell = null;
            dgvDiem.DataSource = null;
        }
        finally { binding = false; }
        txtMaSV.ReadOnly = false;
        txtMaSV.Clear(); txtHoTen.Clear();
        dtpNgaySinh.Value = new DateTime(2005, 1, 1);
        radNu.Checked = true;
        if (lstLop.SelectedItem is LopItem item && item.MaLop.Length > 0) cboLop.SelectedValue = item.MaLop;
        grpDiem.Text = "Điểm của sinh viên";
        ClearGrade(); ShowSummary(BangDiem.Tinh([]));
        txtMaSV.Focus();
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private void ValidateStudent()
    {
        Require(!string.IsNullOrWhiteSpace(txtMaSV.Text), "MSSV không được để trống.");
        Require(txtMaSV.Text.Trim().All(c => c <= 127 && (char.IsLetterOrDigit(c) || c == '_' || c == '-')), "MSSV chỉ gồm chữ không dấu, số, dấu gạch ngang hoặc gạch dưới.");
        Require(!string.IsNullOrWhiteSpace(txtHoTen.Text), "Họ tên không được để trống.");
        Require(cboLop.SelectedValue is string, "Hãy chọn lớp cho sinh viên.");
        Require(dtpNgaySinh.Value.Date <= DateTime.Today, "Ngày sinh không được ở tương lai.");
    }

    private void FillStudent(SinhVien student)
    {
        student.HoTen = txtHoTen.Text.Trim();
        student.NgaySinh = dtpNgaySinh.Value.Date;
        student.GioiTinh = radNam.Checked ? "Nam" : "Nữ";
        student.MaLop = (string)cboLop.SelectedValue!;
    }

    private async Task AddStudentAsync()
    {
        ValidateStudent();
        await using var db = settings.CreateContext();
        string id = txtMaSV.Text.Trim();
        Require(!await db.SinhViens.AnyAsync(x => x.MaSV == id), "MSSV đã tồn tại. Hãy bấm Làm mới và nhập MSSV khác.");
        var student = new SinhVien { MaSV = id };
        FillStudent(student);
        db.SinhViens.Add(student);
        await db.SaveChangesAsync();
        await LoadStudentsAsync(id);
    }

    private async Task EditStudentAsync()
    {
        Require(selectedMaSV != null, "Hãy chọn sinh viên cần sửa.");
        ValidateStudent();
        await using var db = settings.CreateContext();
        var student = await db.SinhViens.SingleAsync(x => x.MaSV == selectedMaSV);
        FillStudent(student);
        await db.SaveChangesAsync();
        await LoadStudentsAsync(student.MaSV);
    }

    private async Task DeleteStudentAsync()
    {
        Require(selectedMaSV != null, "Hãy chọn sinh viên cần xóa.");
        string id = selectedMaSV!;
        if (MessageBox.Show(this, $"Xóa sinh viên {id} và toàn bộ điểm của sinh viên này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        await using var db = settings.CreateContext();
        await using var transaction = await db.Database.BeginTransactionAsync();
        await db.DiemMons.Where(x => x.MaSV == id).ExecuteDeleteAsync();
        await db.SinhViens.Where(x => x.MaSV == id).ExecuteDeleteAsync();
        await transaction.CommitAsync();
        await LoadStudentsAsync();
    }

    private void ValidateGrade()
    {
        Require(selectedMaSV != null, "Hãy chọn sinh viên trước khi nhập điểm.");
        Require(!string.IsNullOrWhiteSpace(txtTenMon.Text), "Tên môn học không được để trống.");
    }

    private async Task AddGradeAsync()
    {
        ValidateGrade();
        string id = selectedMaSV!;
        await using var db = settings.CreateContext();
        db.DiemMons.Add(new DiemMon { MaSV = id, TenMon = txtTenMon.Text.Trim(), SoTinChi = (int)nudTinChi.Value, Diem = nudDiem.Value });
        await db.SaveChangesAsync();
        await LoadGradesAsync(id);
    }

    private async Task EditGradeAsync()
    {
        ValidateGrade();
        Require(selectedMaDiem != null, "Hãy chọn môn cần sửa trong bảng điểm.");
        string id = selectedMaSV!;
        await using var db = settings.CreateContext();
        var grade = await db.DiemMons.SingleAsync(x => x.MaDiem == selectedMaDiem && x.MaSV == id);
        grade.TenMon = txtTenMon.Text.Trim(); grade.SoTinChi = (int)nudTinChi.Value; grade.Diem = nudDiem.Value;
        await db.SaveChangesAsync();
        await LoadGradesAsync(id);
    }

    private async Task DeleteGradeAsync()
    {
        Require(selectedMaSV != null && selectedMaDiem != null, "Hãy chọn môn cần xóa trong bảng điểm.");
        if (MessageBox.Show(this, "Xóa môn học đang chọn?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        string id = selectedMaSV!;
        await using var db = settings.CreateContext();
        await db.DiemMons.Where(x => x.MaDiem == selectedMaDiem && x.MaSV == id).ExecuteDeleteAsync();
        await LoadGradesAsync(id);
    }

    private async Task PreparePrintAsync()
    {
        Require(selectedMaSV != null, "Hãy chọn sinh viên để in phiếu báo điểm.");
        await using var db = settings.CreateContext();
        var student = await db.SinhViens.AsNoTracking().Include(x => x.MaLopNavigation)
            .Include(x => x.DiemMons).SingleAsync(x => x.MaSV == selectedMaSV);
        Require(student.DiemMons.Count > 0, "Sinh viên chưa có điểm. Không thể in phiếu trống.");
        printer.SetData(settings.TenKhoa, student, student.DiemMons.OrderBy(x => x.MaDiem).ToList());
        Require(printer.Document.PrinterSettings.IsValid, "Không có máy in hợp lệ. Hãy cài máy in hoặc bật Microsoft Print to PDF.");
    }

    private async Task PreviewAsync()
    {
        await PreparePrintAsync();
        using var dialog = new PrintPreviewDialog { Document = printer.Document, Width = 1050, Height = 800, UseAntiAlias = true };
        dialog.Shown += (_, _) =>
        {
            var area = Screen.FromControl(this).WorkingArea;
            dialog.Bounds = new Rectangle(area.Left + 40, area.Top + 30, Math.Min(1100, area.Width - 80), Math.Min(900, area.Height - 60));
        };
        dialog.ShowDialog(this);
    }

    private async Task PrintAsync()
    {
        await PreparePrintAsync();
        using var dialog = new PrintDialog { Document = printer.Document, UseEXDialog = true, AllowSomePages = false };
        if (dialog.ShowDialog(this) == DialogResult.OK) printer.Document.Print();
    }

    private Task PageSetupAsync()
    {
        if (!printer.Document.PrinterSettings.IsValid) throw new InvalidPrinterException(printer.Document.PrinterSettings);
        using var dialog = new PageSetupDialog { Document = printer.Document, EnableMetric = true };
        dialog.ShowDialog(this);
        return Task.CompletedTask;
    }

    private async Task ExportAsync()
    {
        var selectedClass = lstLop.SelectedItem as LopItem;
        Require(selectedClass != null && selectedClass.MaLop.Length > 0, "Hãy chọn một lớp cụ thể ở danh sách Lớp để xuất Excel.");
        string classId = selectedClass!.MaLop;
        await using var db = settings.CreateContext();
        var rows = await (from sv in db.SinhViens.AsNoTracking()
                          join lop in db.Lops on sv.MaLop equals lop.MaLop
                          join diem in db.DiemMons on sv.MaSV equals diem.MaSV
                          where lop.MaLop == classId
                          orderby sv.MaSV, diem.MaDiem
                          select new DiemExcel(sv.MaSV, sv.HoTen, lop.MaLop, diem.TenMon, diem.SoTinChi, diem.Diem)).ToListAsync();
        Require(rows.Count > 0, "Lớp đang chọn chưa có điểm để xuất Excel.");
        using var dialog = new SaveFileDialog { Filter = "Excel Workbook (*.xlsx)|*.xlsx", FileName = $"BangDiem_{classId}.xlsx", DefaultExt = "xlsx" };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        await Task.Run(() => ExcelExporter.Save(dialog.FileName, rows));
        MessageBox.Show(this, $"Đã xuất {rows.Count} dòng điểm của lớp {classId}.\n{dialog.FileName}", "Xuất Excel thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
