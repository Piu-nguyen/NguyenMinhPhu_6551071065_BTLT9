using System.Globalization;

namespace Tao_Xuat_Report_Chap9;

/// <summary>Cấu hình dùng chung: chuỗi kết nối, culture, nơi lưu mẫu report.</summary>
public static class AppConfig
{
    // SQL Server localhost, đăng nhập bằng tài khoản Windows hiện tại.
    public const string ConnectionString =
        @"Server=localhost;Database=ReportDemoDb;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

    public const string TenCuaHang = "CỬA HÀNG DEMO CHƯƠNG 9";
    public const string DiaChiCuaHang = "123 Đường Lập Trình, Quận 1, TP.HCM";

    // Tên các file mẫu report (.frx) - sinh viên tự tạo trong Designer
    public const string FrxBangGia = "BangGia.frx";
    public const string FrxHoaDon = "HoaDon.frx";
    public const string FrxDoanhThu = "DoanhThu.frx";

    public static readonly CultureInfo Vi = new("vi-VN");

    /// <summary>
    /// Thư mục chứa các file .frx.
    /// Khi chạy từ Visual Studio, exe nằm ở bin\Debug\net9.0-windows\ nên ta đi ngược lên
    /// tới thư mục chứa file .csproj và dùng thư mục Reports\ ở đó.
    /// Nhờ vậy mẫu lưu từ Designer nằm trong source (không bị mất khi Clean/Rebuild).
    /// </summary>
    public static string ThuMucReport { get; } = TimThuMucReport();

    public static string DuongDanReport(string tenFile) => Path.Combine(ThuMucReport, tenFile);

    private static string TimThuMucReport()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (dir.GetFiles("*.csproj").Length > 0)
            {
                var p = Path.Combine(dir.FullName, "Reports");
                Directory.CreateDirectory(p);
                return p;
            }
            dir = dir.Parent;
        }

        var macDinh = Path.Combine(AppContext.BaseDirectory, "Reports");
        Directory.CreateDirectory(macDinh);
        return macDinh;
    }
}
