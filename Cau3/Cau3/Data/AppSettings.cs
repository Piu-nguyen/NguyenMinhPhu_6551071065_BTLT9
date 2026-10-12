using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Cau3.Data;

internal sealed class AppSettings
{
    public string ConnectionString { get; set; } = "Server=localhost;Database=QuanLyDiem;Trusted_Connection=True;TrustServerCertificate=True;";
    public string TenKhoa { get; set; } = "KHOA CÔNG NGHỆ THÔNG TIN";

    public static AppSettings Load() => JsonSerializer.Deserialize<AppSettings>(
        File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "appsettings.json")))
        ?? throw new InvalidOperationException("Không đọc được cấu hình appsettings.json.");

    public QuanLyDiemContext CreateContext() => new(new DbContextOptionsBuilder<QuanLyDiemContext>()
        .UseSqlServer(ConnectionString).Options);
}
