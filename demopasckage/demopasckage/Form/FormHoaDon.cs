using Microsoft.Identity.Client;
using Tao_Xuat_Report_Chap9.Helpers;
using Tao_Xuat_Report_Chap9.Models;
using Tao_Xuat_Report_Chap9.Services;

using Report = FastReport.Report;

namespace Tao_Xuat_Report_Chap9.Forms;

public partial class FormHoaDon : Form
{
    private readonly ReportDataService _svc = new();
    private List<HoaDonTomTat> _dsHoaDon = new();
    private HoaDonDayDu? _hdDangChon;
    private HoaDonPrinter? _printer;
    private int _lanTai;

    public FormHoaDon()
    {
        InitializeComponent();
        CapNhatNut();

        Load += (_, _) => UiHelper.ChayAsync(this, TaiDanhSachAsync);
        btnTim.Click += (_, _) => UiHelper.ChayAsync(this, TaiDanhSachAsync);
        gridHoaDon.SelectionChanged += (_, _) => UiHelper.ChayAsync(this, TaiChiTietAsync);

        btnXem.Click += (_, _) => ChayVoiHoaDon(XemHoaDon);
        btnPdf.Click += (_, _) => ChayVoiHoaDon(XuatPdf);
        btnInFr.Click += (_, _) => ChayVoiHoaDon(InBangFastReport);
        btnThietKe.Click += (_, _) => ChayVoiHoaDon(ThietKeMau);

        btnPreview.Click += (_, _) => ChayVoiHoaDon(XemTruocPrintDocument);
        btnInDoc.Click += (_, _) => ChayVoiHoaDon(InBangPrintDocument);
        btnPageSetup.Click += (_, _) => ChayVoiHoaDon(CaiDatTrangPrintDocument);
        FormClosed += (_, _) => _printer?.Dispose();
    }

    private async Task TaiDanhSachAsync()
    {
        _dsHoaDon = await _svc.LayHoaDonAsync(dtpTu.Value, dtpDen.Value);
        gridHoaDon.DataSource = _dsHoaDon;
        if (_dsHoaDon.Count == 0) await TaiChiTietAsync();
    }

    private async Task TaiChiTietAsync()
    {
        int lan = ++_lanTai;
        var item = gridHoaDon.CurrentRow?.DataBoundItem as HoaDonTomTat;

        if (item == null)
        {
            _printer?.Dispose();
            _printer = null;
            _hdDangChon = null;
            gridChiTiet.DataSource = null;
            lblTong.Text = "";
            CapNhatNut();
            return;
        }

        var hd = await _svc.LayHoaDonDayDuAsync(item.MaHoaDon);
        if (lan != _lanTai) return;

        _printer?.Dispose();
        _printer = null;
        _hdDangChon = hd;
        gridChiTiet.DataSource = hd?.Dong;
        lblTong.Text = hd == null ? "" : $"HD{hd.Info.MaHoaDon:D6}  —  {hd.Dong.Count} dòng  —  Tổng cộng: {hd.TongTien:N0} đ";
        CapNhatNut();
    }

    private void CapNhatNut()
    {
        bool coHoaDon = _hdDangChon != null;
        foreach (var b in new[] { btnXem, btnPdf, btnInFr, btnThietKe, btnPreview, btnInDoc, btnPageSetup })
            b.Enabled = coHoaDon;
    }

    private void ChayVoiHoaDon(Action<HoaDonDayDu> tacVu)
    {
        var hd = _hdDangChon;
        if (hd == null) return;
        UiHelper.Chay(this, () => tacVu(hd));
    }

    private static Report TaoReportHoaDon(HoaDonDayDu hd)
    {
        var r = ReportService.TaoReport(AppConfig.FrxHoaDon);

        ReportService.DangKy(r, hd.Dong, "ChiTiet");

        var i = hd.Info;
        ReportService.DatThamSo(r, "TenCuaHang", AppConfig.TenCuaHang);
        ReportService.DatThamSo(r, "SoHD", $"HD{i.MaHoaDon:D6}");
        ReportService.DatThamSo(r, "NgayLap", i.NgayLap.ToString("dd/MM/yyyy HH:mm"));
        ReportService.DatThamSo(r, "TenKhachHang", i.TenKhachHang);
        ReportService.DatThamSo(r, "DienThoai", i.DienThoai ?? "");
        ReportService.DatThamSo(r, "DiaChi", i.DiaChi ?? "");
        ReportService.DatThamSo(r, "GhiChu", i.GhiChu ?? "");
        return r;
    }

    private void XemHoaDon(HoaDonDayDu hd)
    {
        if (!UiHelper.CoMauReport(this, AppConfig.FrxHoaDon)) return;
        using var r = TaoReportHoaDon(hd);
        ReportService.XemTruoc(r);
    }

    private void XuatPdf(HoaDonDayDu hd)
    {
        if (!UiHelper.CoMauReport(this, AppConfig.FrxHoaDon)) return;

        var path = UiHelper.ChonFileLuu("PDF (*.pdf)|*.pdf", $"HoaDon_HD{hd.Info.MaHoaDon:D6}.pdf");
        if (path == null) return;

        using var r = TaoReportHoaDon(hd);
        ReportService.XuatPdf(r, path);
        UiHelper.HoiMoFile(this, path);
    }

    private void InBangFastReport(HoaDonDayDu hd)
    {
        if (!UiHelper.CoMauReport(this, AppConfig.FrxHoaDon)) return;
        using var r = TaoReportHoaDon(hd);
        ReportService.InThang(r, hienHopThoai: true);
    }

    private void ThietKeMau(HoaDonDayDu hd)
    {
        using var r = TaoReportHoaDon(hd);
        ReportService.ThietKe(r);
    }

    private void XemTruocPrintDocument(HoaDonDayDu hd)
    {
        var p = _printer ??= new HoaDonPrinter(hd);
        p.XemTruoc(this);
    }

    private void InBangPrintDocument(HoaDonDayDu hd)
    {
        var p = _printer ??= new HoaDonPrinter(hd);
        p.InCoHopThoai(this);
    }

    private void CaiDatTrangPrintDocument(HoaDonDayDu hd)
    {
        var p = _printer ??= new HoaDonPrinter(hd);
        p.CaiDatTrang(this);
    }
}
