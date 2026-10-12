using Microsoft.Identity.Client;
using Tao_Xuat_Report_Chap9.Helpers;
using Tao_Xuat_Report_Chap9.Models;
using Tao_Xuat_Report_Chap9.Services;



using Report = FastReport.Report;

namespace Tao_Xuat_Report_Chap9.Forms;

public partial class FormDoanhThu : Form
{
    private readonly ReportDataService _svc = new();
    private List<DoanhThuDong> _ds = new();

    public FormDoanhThu()
    {
        InitializeComponent();
        CapNhatNut();

        Load += (_, _) => UiHelper.ChayAsync(this, TaiDuLieuAsync);
        btnTai.Click += (_, _) => UiHelper.ChayAsync(this, TaiDuLieuAsync);

        btnXem.Click += (_, _) => ChayVoiDuLieu(XemBaoCao);
        btnPdf.Click += (_, _) => ChayVoiDuLieu(XuatPdf);
        btnExcel.Click += (_, _) => UiHelper.ChayAsync(this, XuatExcelAsync);
        btnThietKe.Click += (_, _) => ChayVoiDuLieu(ThietKeMau);
    }

    private async Task TaiDuLieuAsync()
    {
        var tu = dtpTu.Value;
        var den = dtpDen.Value;

        _ds = await _svc.LayDoanhThuAsync(tu, den);
        var tongHop = await _svc.LayTongHopTheoDanhMucAsync(tu, den);

        gridChiTiet.DataSource = _ds;
        gridTongHop.DataSource = tongHop;

        lblTong.Text = $"{_ds.Count} dòng  —  Tổng doanh thu (EF Core): {_ds.Sum(x => x.ThanhTien):N0} đ  " +
                       "(con số này phải KHỚP với [Tổng cộng] trong report)";
        CapNhatNut();
    }

    private void CapNhatNut()
    {
        bool coDuLieu = _ds.Count > 0;
        btnXem.Enabled = coDuLieu;
        btnPdf.Enabled = coDuLieu;
        btnExcel.Enabled = coDuLieu;
        btnThietKe.Enabled = coDuLieu;
    }

    private void ChayVoiDuLieu(Action<List<DoanhThuDong>> tacVu)
    {
        var ds = _ds;
        if (ds.Count == 0) return;
        UiHelper.Chay(this, () => tacVu(ds));
    }

    private Report TaoReportDoanhThu(List<DoanhThuDong> ds)
    {
        var r = ReportService.TaoReport(AppConfig.FrxDoanhThu);
        ReportService.DangKy(r, ds, "DoanhThu");
        ReportService.DatThamSo(r, "TuNgay", dtpTu.Value.ToString("dd/MM/yyyy"));
        ReportService.DatThamSo(r, "DenNgay", dtpDen.Value.ToString("dd/MM/yyyy"));
        return r;
    }

    private void XemBaoCao(List<DoanhThuDong> ds)
    {
        if (!UiHelper.CoMauReport(this, AppConfig.FrxDoanhThu)) return;
        using var r = TaoReportDoanhThu(ds);
        ReportService.XemTruoc(r);
    }

    private void XuatPdf(List<DoanhThuDong> ds)
    {
        if (!UiHelper.CoMauReport(this, AppConfig.FrxDoanhThu)) return;

        var path = UiHelper.ChonFileLuu("PDF (*.pdf)|*.pdf", $"DoanhThu_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
        if (path == null) return;

        using var r = TaoReportDoanhThu(ds);
        ReportService.XuatPdf(r, path);
        UiHelper.HoiMoFile(this, path);
    }

    private void ThietKeMau(List<DoanhThuDong> ds)
    {
        using var r = TaoReportDoanhThu(ds);
        ReportService.ThietKe(r);
    }

    private async Task XuatExcelAsync()
    {
        var ds = _ds;
        if (ds.Count == 0) return;

        var path = UiHelper.ChonFileLuu("Excel (*.xlsx)|*.xlsx", $"DoanhThu_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        if (path == null) return;

        var cot = new List<CotExcel>
        {
            new("Danh mục"),
            new("Sản phẩm"),
            new("Mã HĐ", "0"),
            new("Ngày lập", "dd/MM/yyyy"),
            new("Số lượng", "#,##0", CongTong: true),
            new("Thành tiền", "#,##0", CongTong: true)
        };

        var dong = ds.Select(x => new object?[] { x.TenDanhMuc, x.TenSanPham, x.MaHoaDon, x.NgayLap, x.SoLuong, x.ThanhTien }).ToList();

        await Task.Run(() => ExcelExporter.Xuat(path, "DoanhThu", cot, dong));
        UiHelper.HoiMoFile(this, path);
    }
}