using Microsoft.EntityFrameworkCore;
using Tao_Xuat_Report_Chap9.Models;

namespace Tao_Xuat_Report_Chap9.Services;

public class ReportDataService
{
    public async Task<List<DanhMuc>> LayDanhMucAsync()
    {
        await using var db = new ReportDemoDbContext();
        return await db.DanhMucs.AsNoTracking().OrderBy(x => x.TenDanhMuc).ToListAsync();
    }

    public async Task<List<SanPhamDong>> LaySanPhamAsync(string? tuKhoa, int? maDanhMuc)
    {
        await using var db = new ReportDemoDbContext();

        var q = from sp in db.SanPhams.AsNoTracking()
                join dm in db.DanhMucs on sp.MaDanhMuc equals dm.MaDanhMuc
                select new { sp, dm };

        var kw = (tuKhoa ?? "").Trim();
        if (kw.Length > 0)
            q = q.Where(x => x.sp.TenSanPham.Contains(kw));

        if (maDanhMuc is > 0)
        {
            int ma = maDanhMuc.Value;
            q = q.Where(x => x.sp.MaDanhMuc == ma);
        }

        return await q
            .OrderBy(x => x.dm.TenDanhMuc).ThenBy(x => x.sp.TenSanPham)
            .Select(x => new SanPhamDong(x.sp.MaSanPham, x.sp.TenSanPham, x.dm.TenDanhMuc, x.sp.DonGia, x.sp.SoLuongTon))
            .ToListAsync();
    }

    public async Task<List<HoaDonTomTat>> LayHoaDonAsync(DateTime tuNgay, DateTime denNgay)
    {
        KiemTraKhoangNgay(tuNgay, denNgay);
        await using var db = new ReportDemoDbContext();

        var tu = tuNgay.Date;
        var den = denNgay.Date.AddDays(1);

        var q = from hd in db.HoaDons.AsNoTracking()
                join kh in db.KhachHangs on hd.MaKhachHang equals kh.MaKhachHang
                where hd.NgayLap >= tu && hd.NgayLap < den
                orderby hd.NgayLap descending
                select new HoaDonTomTat(
                    hd.MaHoaDon,
                    hd.NgayLap,
                    kh.TenKhachHang,
                    db.ChiTietHoaDons.Count(ct => ct.MaHoaDon == hd.MaHoaDon),
                    db.ChiTietHoaDons.Where(ct => ct.MaHoaDon == hd.MaHoaDon).Sum(ct => ct.SoLuong * ct.DonGia));

        return await q.ToListAsync();
    }

    public async Task<HoaDonDayDu?> LayHoaDonDayDuAsync(int maHoaDon)
    {
        await using var db = new ReportDemoDbContext();

        var info = await (from hd in db.HoaDons.AsNoTracking()
                          join kh in db.KhachHangs on hd.MaKhachHang equals kh.MaKhachHang
                          where hd.MaHoaDon == maHoaDon
                          select new HoaDonInfo(hd.MaHoaDon, hd.NgayLap, kh.TenKhachHang, kh.DienThoai, kh.DiaChi, hd.GhiChu))
                         .FirstOrDefaultAsync();

        if (info == null) return null;

        var raw = await (from ct in db.ChiTietHoaDons.AsNoTracking()
                         join sp in db.SanPhams on ct.MaSanPham equals sp.MaSanPham
                         where ct.MaHoaDon == maHoaDon
                         orderby ct.MaChiTiet
                         select new { sp.TenSanPham, ct.SoLuong, ct.DonGia })
                         .ToListAsync();

        var dong = raw
            .Select((x, i) => new ChiTietDong(i + 1, x.TenSanPham, x.SoLuong, x.DonGia, x.SoLuong * x.DonGia))
            .ToList();

        return new HoaDonDayDu(info, dong);
    }

    public async Task<List<DoanhThuDong>> LayDoanhThuAsync(DateTime tuNgay, DateTime denNgay)
    {
        KiemTraKhoangNgay(tuNgay, denNgay);
        await using var db = new ReportDemoDbContext();

        var tu = tuNgay.Date;
        var den = denNgay.Date.AddDays(1);

        var q = from ct in db.ChiTietHoaDons.AsNoTracking()
                join hd in db.HoaDons on ct.MaHoaDon equals hd.MaHoaDon
                join sp in db.SanPhams on ct.MaSanPham equals sp.MaSanPham
                join dm in db.DanhMucs on sp.MaDanhMuc equals dm.MaDanhMuc
                where hd.NgayLap >= tu && hd.NgayLap < den
                orderby dm.TenDanhMuc, hd.NgayLap, hd.MaHoaDon
                select new DoanhThuDong(dm.TenDanhMuc, sp.TenSanPham, hd.MaHoaDon, hd.NgayLap, ct.SoLuong, ct.SoLuong * ct.DonGia);

        return await q.ToListAsync();
    }

    public async Task<List<TongHopDanhMuc>> LayTongHopTheoDanhMucAsync(DateTime tuNgay, DateTime denNgay)
    {
        KiemTraKhoangNgay(tuNgay, denNgay);
        await using var db = new ReportDemoDbContext();

        var tu = tuNgay.Date;
        var den = denNgay.Date.AddDays(1);

        var q = from ct in db.ChiTietHoaDons.AsNoTracking()
                join hd in db.HoaDons on ct.MaHoaDon equals hd.MaHoaDon
                join sp in db.SanPhams on ct.MaSanPham equals sp.MaSanPham
                join dm in db.DanhMucs on sp.MaDanhMuc equals dm.MaDanhMuc
                where hd.NgayLap >= tu && hd.NgayLap < den
                select new { dm.TenDanhMuc, ct.SoLuong, ThanhTien = ct.SoLuong * ct.DonGia };

        return await q
            .GroupBy(x => x.TenDanhMuc)
            .OrderBy(g => g.Key)
            .Select(g => new TongHopDanhMuc(g.Key, g.Count(), g.Sum(x => x.SoLuong), g.Sum(x => x.ThanhTien)))
            .ToListAsync();
    }

    private static void KiemTraKhoangNgay(DateTime tuNgay, DateTime denNgay)
    {
        if (tuNgay.Date > denNgay.Date)
            throw new ArgumentException("Từ ngày phải nhỏ hơn hoặc bằng đến ngày.");
    }
}
