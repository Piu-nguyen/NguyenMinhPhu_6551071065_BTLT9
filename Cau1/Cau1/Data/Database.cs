using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Cau1.Data;

internal static class Database
{
    public static TiemTapHoaContext CreateContext()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        using var settings = JsonDocument.Parse(File.ReadAllText(path));
        var connection = settings.RootElement.GetProperty("ConnectionStrings")
            .GetProperty("TiemTapHoa").GetString();
        if (string.IsNullOrWhiteSpace(connection))
            throw new InvalidOperationException("Chưa cấu hình kết nối trong appsettings.json.");

        var options = new DbContextOptionsBuilder<TiemTapHoaContext>()
            .UseSqlServer(connection).Options;
        return new TiemTapHoaContext(options);
    }
}
