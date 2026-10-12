using System.Drawing.Printing;
using System.Globalization;
using Tao_Xuat_Report_Chap9.Models;

namespace Tao_Xuat_Report_Chap9.Services;

public sealed class HoaDonPrinter : IDisposable
{
    private const float CaoDong = 24f;
    private const float CaoTieuDeBang = 28f;
    private const float CaoDauTrang1 = 235f;
    private const float CaoDauTrangN = 45f;
    private const float CaoChanTrang = 30f;
    private const int SoDongDanhChoTong = 5;

    private static readonly float[] TyLeCot = { 0.07f, 0.45f, 0.10f, 0.18f, 0.20f };
    private static readonly string[] TenCot = { "STT", "Tên sản phẩm", "SL", "Đơn giá", "Thành tiền" };

    private static readonly StringFormat Trai = new()
    {
        LineAlignment = StringAlignment.Center,
        Trimming = StringTrimming.EllipsisCharacter,
        FormatFlags = StringFormatFlags.NoWrap
    };
    private static readonly StringFormat Phai = new(Trai) { Alignment = StringAlignment.Far };
    private static readonly StringFormat Giua = new(Trai) { Alignment = StringAlignment.Center };

    private readonly HoaDonDayDu _hd;
    private readonly PrintDocument _doc = new();
    private readonly CultureInfo _vi = AppConfig.Vi;

    private readonly Font _fTieuDe = new("Segoe UI", 18, FontStyle.Bold);
    private readonly Font _fDam = new("Segoe UI", 10, FontStyle.Bold);
    private readonly Font _fThuong = new("Segoe UI", 10);
    private readonly Font _fNho = new("Segoe UI", 8, FontStyle.Italic);

    private List<(int Bat, int SoLuong)> _trang = new();
    private int _chiSoTrang;

    public HoaDonPrinter(HoaDonDayDu hoaDon)
    {
        _hd = hoaDon;
        _doc.DocumentName = $"HoaDon_HD{hoaDon.Info.MaHoaDon:D6}";

        _doc.DefaultPageSettings.Margins = new Margins(60, 60, 60, 60);

        _doc.BeginPrint += OnBeginPrint;
        _doc.PrintPage += OnPrintPage;
    }

    public void XemTruoc(IWin32Window owner)
    {
        using var dlg = new PrintPreviewDialog
        {
            Document = _doc,
            UseAntiAlias = true,
            StartPosition = FormStartPosition.CenterParent,
            Size = new Size(1000, 760)
        };
        dlg.ShowDialog(owner);
    }

    public void InCoHopThoai(IWin32Window owner)
    {
        using var dlg = new PrintDialog
        {
            Document = _doc,
            UseEXDialog = true,
            AllowSomePages = false
        };
        if (dlg.ShowDialog(owner) == DialogResult.OK)
            _doc.Print();
    }

    public void CaiDatTrang(IWin32Window owner)
    {
        using var dlg = new PageSetupDialog
        {
            Document = _doc,
            AllowPrinter = true
        };
        dlg.ShowDialog(owner);
    }

    private void OnBeginPrint(object? sender, PrintEventArgs e)
    {
        _chiSoTrang = 0;

        var ps = _doc.DefaultPageSettings;
        float cao = ps.Bounds.Height - ps.Margins.Top - ps.Margins.Bottom;
        _trang = ChiaTrang(cao);
    }

    private List<(int Bat, int SoLuong)> ChiaTrang(float caoVung)
    {
        int capTrang1 = (int)((caoVung - CaoDauTrang1 - CaoTieuDeBang - CaoChanTrang) / CaoDong);
        int capTrangN = (int)((caoVung - CaoDauTrangN - CaoTieuDeBang - CaoChanTrang) / CaoDong);
        capTrang1 = Math.Max(capTrang1, SoDongDanhChoTong + 1);
        capTrangN = Math.Max(capTrangN, SoDongDanhChoTong + 1);

        var ds = new List<(int, int)>();
        int i = 0, n = _hd.Dong.Count;
        while (true)
        {
            int cap = ds.Count == 0 ? capTrang1 : capTrangN;
            int con = n - i;

            if (con <= cap - SoDongDanhChoTong)
            {
                ds.Add((i, con));
                break;
            }

            int lay = Math.Min(cap, con);
            ds.Add((i, lay));
            i += lay;
        }
        return ds;
    }

    private void OnPrintPage(object? sender, PrintPageEventArgs e)
    {
        var g = e.Graphics!;
        var v = e.MarginBounds;
        var (bat, soLuong) = _trang[_chiSoTrang];

        float y = v.Top;
        y = _chiSoTrang == 0 ? VeDauTrangDau(g, v, y) : VeDauTrangSau(g, v, y);
        y = VeTieuDeBang(g, v, y);

        for (int i = bat; i < bat + soLuong; i++)
            y = VeDong(g, v, y, _hd.Dong[i]);

        bool trangCuoi = _chiSoTrang == _trang.Count - 1;
        if (trangCuoi)
            VeTong(g, v, y);

        VeChanTrang(g, v);

        _chiSoTrang++;
        e.HasMorePages = _chiSoTrang < _trang.Count;
    }

    private string Tien(decimal d) => d.ToString("N0", _vi);

    private float VeDauTrangDau(Graphics g, Rectangle v, float y)
    {
        g.DrawString(AppConfig.TenCuaHang, _fDam, Brushes.Black, new RectangleF(v.Left, y, v.Width, 24), Trai);
        y += 24;
        g.DrawString(AppConfig.DiaChiCuaHang, _fThuong, Brushes.Black, new RectangleF(v.Left, y, v.Width, 20), Trai);
        y += 20 + 10;

        g.DrawString("HÓA ĐƠN BÁN HÀNG", _fTieuDe, Brushes.Black, new RectangleF(v.Left, y, v.Width, 40), Giua);
        y += 40 + 10;

        var i = _hd.Info;
        y = DongThongTin(g, v, y, $"Số HĐ: HD{i.MaHoaDon:D6}", $"Ngày lập: {i.NgayLap:dd/MM/yyyy HH:mm}");
        y = DongThongTin(g, v, y, $"Khách hàng: {i.TenKhachHang}");
        y = DongThongTin(g, v, y, $"Điện thoại: {i.DienThoai}");
        y = DongThongTin(g, v, y, $"Địa chỉ: {i.DiaChi}");
        y = DongThongTin(g, v, y, $"Ghi chú: {i.GhiChu}");
        return y + 6;
    }

    private float VeDauTrangSau(Graphics g, Rectangle v, float y)
    {
        g.DrawString($"Hóa đơn HD{_hd.Info.MaHoaDon:D6} (tiếp theo)", _fDam, Brushes.Black,
            new RectangleF(v.Left, y, v.Width, 24), Trai);
        return y + CaoDauTrangN;
    }

    private float DongThongTin(Graphics g, Rectangle v, float y, string trai, string? phai = null)
    {
        g.DrawString(trai, _fThuong, Brushes.Black, new RectangleF(v.Left, y, v.Width * 0.6f, 22), Trai);
        if (phai != null)
            g.DrawString(phai, _fThuong, Brushes.Black, new RectangleF(v.Left + v.Width * 0.6f, y, v.Width * 0.4f, 22), Phai);
        return y + 22;
    }

    private static (float[] X, float[] W) ViTriCot(Rectangle v)
    {
        var x = new float[TyLeCot.Length];
        var w = new float[TyLeCot.Length];
        float cur = v.Left;
        for (int i = 0; i < TyLeCot.Length; i++)
        {
            x[i] = cur;
            w[i] = v.Width * TyLeCot[i];
            cur += w[i];
        }
        return (x, w);
    }

    private float VeTieuDeBang(Graphics g, Rectangle v, float y)
    {
        g.FillRectangle(Brushes.LightGray, v.Left, y, v.Width, CaoTieuDeBang);
        g.DrawRectangle(Pens.Black, v.Left, y, v.Width, CaoTieuDeBang);

        var (x, w) = ViTriCot(v);
        for (int i = 0; i < TenCot.Length; i++)
        {
            var fmt = i >= 3 ? Phai : (i == 1 ? Trai : Giua);
            g.DrawString(TenCot[i], _fDam, Brushes.Black, new RectangleF(x[i] + 4, y, w[i] - 8, CaoTieuDeBang), fmt);
        }
        return y + CaoTieuDeBang;
    }

    private float VeDong(Graphics g, Rectangle v, float y, ChiTietDong d)
    {
        var (x, w) = ViTriCot(v);
        string[] giaTri = { d.STT.ToString(), d.TenSanPham, d.SoLuong.ToString(), Tien(d.DonGia), Tien(d.ThanhTien) };

        for (int i = 0; i < giaTri.Length; i++)
        {
            var fmt = i >= 3 ? Phai : (i == 1 ? Trai : Giua);
            g.DrawString(giaTri[i], _fThuong, Brushes.Black, new RectangleF(x[i] + 4, y, w[i] - 8, CaoDong), fmt);
        }
        g.DrawLine(Pens.Gray, v.Left, y + CaoDong, v.Right, y + CaoDong);
        return y + CaoDong;
    }

    private void VeTong(Graphics g, Rectangle v, float y)
    {
        g.DrawLine(Pens.Black, v.Left, y, v.Right, y);
        y += 6;

        float nua = v.Width / 2f;
        g.DrawString($"Số dòng: {_hd.Dong.Count}", _fThuong, Brushes.Black, new RectangleF(v.Left, y, nua, CaoDong), Trai);
        g.DrawString($"Tổng cộng: {Tien(_hd.TongTien)} đ", _fDam, Brushes.Black, new RectangleF(v.Left + nua, y, nua, CaoDong), Phai);
        y += CaoDong + 24;

        g.DrawString("Người lập phiếu", _fDam, Brushes.Black, new RectangleF(v.Left, y, nua, CaoDong), Giua);
        g.DrawString("Khách hàng", _fDam, Brushes.Black, new RectangleF(v.Left + nua, y, nua, CaoDong), Giua);
        y += CaoDong;
        g.DrawString("(Ký, ghi rõ họ tên)", _fNho, Brushes.Black, new RectangleF(v.Left, y, nua, 20), Giua);
        g.DrawString("(Ký, ghi rõ họ tên)", _fNho, Brushes.Black, new RectangleF(v.Left + nua, y, nua, 20), Giua);
    }

    private void VeChanTrang(Graphics g, Rectangle v)
    {
        float y = v.Bottom - 20;
        g.DrawString($"In lúc {DateTime.Now:dd/MM/yyyy HH:mm}", _fNho, Brushes.Gray,
            new RectangleF(v.Left, y, v.Width / 2f, 20), Trai);
        g.DrawString($"Trang {_chiSoTrang + 1}/{_trang.Count}", _fNho, Brushes.Gray,
            new RectangleF(v.Left + v.Width / 2f, y, v.Width / 2f, 20), Phai);
    }

    public void Dispose()
    {
        _doc.Dispose();
        _fTieuDe.Dispose();
        _fDam.Dispose();
        _fThuong.Dispose();
        _fNho.Dispose();
    }
}