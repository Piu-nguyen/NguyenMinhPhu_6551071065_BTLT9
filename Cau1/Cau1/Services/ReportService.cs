using System.Data;
using Cau1.Data;
using FastReport;
using Microsoft.EntityFrameworkCore;

namespace Cau1.Services;

internal static class ReportService
{
    public static async Task<Report> CreatePriceReportAsync()
    {
        await using var db = Database.CreateContext();
        var items = await db.HangHoas.AsNoTracking().Where(x => x.ConKinhDoanh)
            .OrderBy(x => x.MaHang).ToListAsync();
        if (items.Count == 0)
            throw new InvalidOperationException("Không có mặt hàng còn kinh doanh để lập bảng giá.");

        var table = new DataTable("HangHoa");
        table.Columns.Add("TenHang", typeof(string));
        table.Columns.Add("DonVi", typeof(string));
        table.Columns.Add("GiaBan", typeof(decimal));
        foreach (var item in items) table.Rows.Add(item.TenHang, item.DonVi, item.GiaBan);

        var report = new Report();
        try
        {
            report.Load(Path.Combine(AppContext.BaseDirectory, "Reports", "BangGia.frx"));
            report.RegisterData(table, "HangHoa");
            report.GetDataSource("HangHoa").Enabled = true;
            report.SetParameterValue("NgayIn", DateTime.Now.ToString("dd/MM/yyyy"));
            return report;
        }
        catch { report.Dispose(); throw; }
    }
}
