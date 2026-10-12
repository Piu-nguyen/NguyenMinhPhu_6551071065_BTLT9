using Cau2.Models;
using Cau2.ViewModels;
using FastReport;

namespace Cau2.Services;

internal static class HoaDonReport
{
    public static Report Create(HoaDon hoaDon, List<ChiTietIn> chiTiet)
    {
        var report = new Report();
        try
        {
            report.Load(Path.Combine(AppContext.BaseDirectory, "Reports", "HoaDon.frx"));
            report.RegisterData(chiTiet, "ChiTiet");
            report.GetDataSource("ChiTiet").Enabled = true;
            report.SetParameterValue("TenQuan", "QUÁN CÀ PHÊ GÓC PHỐ");
            report.SetParameterValue("SoHoaDon", $"HD{hoaDon.MaHD:000000}");
            report.SetParameterValue("NgayLap", hoaDon.NgayLap.ToString("dd/MM/yyyy HH:mm"));
            report.SetParameterValue("SoBan", hoaDon.SoBan);
            report.SetParameterValue("HinhThucTT", hoaDon.HinhThucTT);
            report.Prepare();
            return report;
        }
        catch
        {
            report.Dispose();
            throw;
        }
    }
}
