using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Cau2.Data;

internal static class Database
{
    public static QuanCaPheContext CreateContext()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        using var settings = JsonDocument.Parse(File.ReadAllText(path));
        string connection = settings.RootElement.GetProperty("ConnectionStrings")
            .GetProperty("QuanCaPhe").GetString()
            ?? throw new InvalidOperationException("Chưa cấu hình kết nối QuanCaPhe.");
        var options = new DbContextOptionsBuilder<QuanCaPheContext>()
            .UseSqlServer(connection).Options;
        return new QuanCaPheContext(options);
    }
}
