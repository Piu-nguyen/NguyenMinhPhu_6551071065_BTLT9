using Microsoft.Identity.Client;
using Tao_Xuat_Report_Chap9.Helpers;

using Tao_Xuat_Report_Chap9.Models;
using Tao_Xuat_Report_Chap9.Services;

using Report = FastReport.Report;

namespace Tao_Xuat_Report_Chap9.Forms;

public partial class FormSanPham : Form
{
    private readonly ReportDataService _svc = new();
    private readonly System.Windows.Forms.Timer _timerTim = new() { Interval = 350 };
    private List<SanPhamDong> _ds = new();

    public FormSanPham()
    {
        InitializeComponent();

        Load += (_, _) => UiHelper.ChayAsync(this, KhoiTaoAsync);

        txtTimKiem.TextChanged += (_, _) => { _timerTim.Stop(); _timerTim.Start(); };
        _timerTim.Tick += (_, _) => { _timerTim.Stop(); UiHelper.ChayAsync(this, TaiLaiAsync); };
        cboDanhMuc.SelectionChangeCommitted += (_, _) => UiHelper.ChayAsync(this, TaiLaiAsync);

        btnExcelLuoi.Click += (_, _) => XuatExcelTuLuoi();
        btnExcelEf.Click += (_, _) => UiHelper.ChayAsync(this, XuatExcelTuEfAsync);
        btnExcelNhanh.Click += (_, _) => XuatExcelNhanh();
        btnBangGia.Click += (_, _) => XemReportBangGia();
        btnThietKe.Click += (_, _) => ThietKeBangGia();

        FormClosed += (_, _) => _timerTim.Dispose();
    }

    private async Task KhoiTaoAsync()
    {
        var dm = await _svc.LayDanhMucAsync();
        dm.Insert(0, new DanhMuc { MaDanhMuc = 0, TenDanhMuc = "— Tất cả —" });

        cboDanhMuc.DisplayMember = nameof(DanhMuc.TenDanhMuc);
        cboDanhMuc.ValueMember = nameof(DanhMuc.MaDanhMuc);
        cboDanhMuc.DataSource = dm;

        await TaiLaiAsync();
    }

    private async Task TaiLaiAsync()
    {
        int maDm = cboDanhMuc.SelectedValue is int v ? v : 0;
        _ds = await _svc.LaySanPhamAsync(txtTimKiem.Text, maDm);

        grid.DataSource = _ds;
        lblTrangThai.Text = $"{_ds.Count} sản phẩm  |  Tổng giá trị tồn kho: {_ds.Sum(x => x.DonGia * x.SoLuongTon):N0} đ";
    }

    private void XuatExcelTuLuoi()
    {
        var path = UiHelper.ChonFileLuu("Excel (*.xlsx)|*.xlsx", $"SanPham_Luoi_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        if (path == null) return;

        UiHelper.Chay(this, () =>
        {
            ExcelExporter.XuatDataGridView(grid, path, "SanPham", nameof(SanPhamDong.SoLuongTon));
            UiHelper.HoiMoFile(this, path);
        });
    }

    private async Task XuatExcelTuEfAsync()
    {
        var path = UiHelper.ChonFileLuu("Excel (*.xlsx)|*.xlsx", $"SanPham_EF_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        if (path == null) return;

        var tatCa = await _svc.LaySanPhamAsync(null, null);

        var cot = new List<CotExcel>
        {
            new("Mã", "0"),
            new("Tên sản phẩm"),
            new("Danh mục"),
            new("Đơn giá", "#,##0"),
            new("Tồn kho", "#,##0", CongTong: true),
            new("Giá trị tồn", "#,##0", CongTong: true, CongThuc: "D{r}*E{r}")
        };

        var dong = tatCa.Select(x => new object?[] { x.MaSanPham, x.TenSanPham, x.TenDanhMuc, x.DonGia, x.SoLuongTon, null });

        ExcelExporter.Xuat(path, "SanPham", cot, dong);
        UiHelper.HoiMoFile(this, path);
    }

    private void XuatExcelNhanh()
    {
        var path = UiHelper.ChonFileLuu("Excel (*.xlsx)|*.xlsx", $"SanPham_Nhanh_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        if (path == null) return;

        UiHelper.Chay(this, () =>
        {
            var dt = DataTableHelper.ToDataTable(_ds, "SanPham");
            ExcelExporter.XuatNhanhDataTable(dt, path, "SanPham");
            UiHelper.HoiMoFile(this, path);
        });
    }

    private Report TaoReportBangGia()
    {
        var dt = DataTableHelper.ToDataTable(_ds, "SanPham");

        var r = ReportService.TaoReport(AppConfig.FrxBangGia);
        ReportService.DangKy(r, dt, "SanPham");
        ReportService.DatThamSo(r, "NgayIn", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
        return r;
    }

    private void XemReportBangGia() => UiHelper.Chay(this, () =>
    {
        if (!UiHelper.CoMauReport(this, AppConfig.FrxBangGia)) return;
        using var r = TaoReportBangGia();
        ReportService.XemTruoc(r);
    });

    private void ThietKeBangGia() => UiHelper.Chay(this, () =>
    {
        using var r = TaoReportBangGia();
        ReportService.ThietKe(r);
    });
}