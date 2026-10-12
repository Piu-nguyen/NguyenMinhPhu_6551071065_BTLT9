namespace Tao_Xuat_Report_Chap9.Models;

// DTO (Data Transfer Object) = "hình dạng dữ liệu" dành riêng cho màn hình / report.
// Dùng record: ngắn gọn, bất biến, có sẵn các property public để DataGridView và FastReport đọc.
// FastReport chỉ cần RegisterData(List<DTO>, "Tên") là có field [Tên.TênProperty] trong Designer.

/// <summary>1 dòng sản phẩm kèm tên danh mục (JOIN SanPham + DanhMuc).</summary>
public record SanPhamDong(int MaSanPham, string TenSanPham, string TenDanhMuc, decimal DonGia, int SoLuongTon);

/// <summary>1 dòng trong danh sách hóa đơn (có tổng tiền tính bằng subquery).</summary>
public record HoaDonTomTat(int MaHoaDon, DateTime NgayLap, string TenKhachHang, int SoDong, decimal TongTien);

/// <summary>Phần "đầu phiếu" của hóa đơn - truyền vào report qua Parameters.</summary>
public record HoaDonInfo(int MaHoaDon, DateTime NgayLap, string TenKhachHang, string? DienThoai, string? DiaChi, string? GhiChu);

/// <summary>1 dòng chi tiết hóa đơn - đăng ký vào report như một nguồn dữ liệu List&lt;T&gt;.</summary>
public record ChiTietDong(int STT, string TenSanPham, int SoLuong, decimal DonGia, decimal ThanhTien);

/// <summary>Hóa đơn đầy đủ = đầu phiếu + các dòng. Dùng chung cho FastReport và PrintDocument.</summary>
public record HoaDonDayDu(HoaDonInfo Info, List<ChiTietDong> Dong)
{
    public decimal TongTien => Dong.Sum(d => d.ThanhTien);
}

/// <summary>1 dòng dữ liệu phẳng cho báo cáo doanh thu - FastReport sẽ tự Group theo TenDanhMuc.</summary>
public record DoanhThuDong(string TenDanhMuc, string TenSanPham, int MaHoaDon, DateTime NgayLap, int SoLuong, decimal ThanhTien);

/// <summary>Kết quả GROUP BY ngay trong EF Core - dùng để đối chiếu với tổng nhóm trong FastReport.</summary>
public record TongHopDanhMuc(string TenDanhMuc, int SoDong, int TongSoLuong, decimal TongTien);
